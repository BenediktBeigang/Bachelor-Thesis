using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

public class WiFi
{
    public bool Listening { get; set; }
    const int CLIENT_PORT = 11000;
    const int WEBSOCKET_PORT = 81;

    private UdpClient udpClient;
    private List<WebsocketClient> clients;

    public WiFi()
    {
        Listening = false;
        clients = new();
        udpClient = new UdpClient();
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, CLIENT_PORT);
        udpClient.Client.Bind(clientEndPoint);
    }

    public void ConnectToHost()
    {
        Thread receiveThread = new Thread(new ThreadStart(this.ReceiveThread));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private async void ReceiveThread()
    {
        Listening = true;
        GlobalData.LastMessages.Add("UDP is listening!");
        while (Listening)
        {
            var result = await udpClient.ReceiveAsync();
            Received received = new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
            HandleIncomingPackage(received);
            GlobalData.LastMessages.Add($"Packet Received:\n{received.ToString()}");
        }
        GlobalData.LastMessages.Add("UDP stopped listening!");
    }

    private void HandleIncomingPackage(Received package)
    {
        switch (package.Message)
        {
            case "I am Node ONE":
                NewNode(package, DeviceNumber.ONE);
                break;
            case "I am Node TWO":
                NewNode(package, DeviceNumber.TWO);
                break;
        }
    }

    private void NewNode(Received package, DeviceNumber device)
    {
        Gyro gyro = new Gyro(GyroMode.GYRO_2000, device);
        switch (device)
        {
            case DeviceNumber.ONE:
                GlobalData.Node_One = new Node(device, gyro, ConnectionType.WIFI, package.Sender);
                break;
            case DeviceNumber.TWO:
                GlobalData.Node_Two = new Node(device, gyro, ConnectionType.WIFI, package.Sender);
                break;
        }

        string url = WebSocketURL(package);
        GlobalData.LastMessages.Add($"Try to connect to Web-Socket of Node {device.ToString()}: {url}");

        Thread connectionThread = new Thread(() => ConnectToWebSocketServer(url, device));
        connectionThread.IsBackground = true;
        connectionThread.Start();
    }

    private void ConnectToWebSocketServer(string serverURL, DeviceNumber device)
    {
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri(serverURL);

        using (var client = new WebsocketClient(url))
        {
            clients.Add(client);
            client.Name = device.ToString();
            client.ReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ReconnectionHappened.Subscribe(info =>
                GlobalData.LastMessages.Add($"Host>> Reconnection happened, type: {info.Type}"));
            client.MessageReceived.Subscribe(msg => WebSocket_OnMessage(msg.Text, client));
            client.Start();
            exitEvent.WaitOne();
        }
    }

    private void WebSocket_OnMessage(string message, WebsocketClient client)
    {
        switch (message)
        {
            case "1 Connected":
                GlobalData.LastMessages.Add("Client>> Node One Connected");
                GlobalData.Node_One.Gyro!.CalibrationStatus = CalibrationStatus.REQUESTED;
                break;
            case "2 Connected":
                GlobalData.LastMessages.Add("Client>> Node Two Connected");
                GlobalData.Node_Two.Gyro!.CalibrationStatus = CalibrationStatus.REQUESTED;
                break;
            default:
                HandleMessagePackage(message, client);
                break;
        }
    }

    private void HandleMessagePackage(string message, WebsocketClient client)
    {
        try
        {
            switch (client.Name)
            {
                case "ONE":
                    if (GlobalData.Node_One.ConnectionType is ConnectionType.WIFI)
                    {
                        GlobalData.Node_One.DataCount++;
                        GlobalData.Node_One.Gyro!.LastRawValue = int.Parse(message);
                    }
                    break;
                case "TWO":
                    if (GlobalData.Node_Two.ConnectionType is ConnectionType.WIFI)
                    {
                        GlobalData.Node_Two.DataCount++;
                        GlobalData.Node_Two.Gyro!.LastRawValue = int.Parse(message);
                    }
                    break;
                default:
                    GlobalData.LastMessages.Add(message);
                    break;
            }
        }
        catch (FormatException)
        {
            GlobalData.LastMessages.Add(message);
        }
    }

    public static string WebSocketURL(Received package)
    {
        string address = package.Sender.Address.ToString();
        string port = package.Sender.Port.ToString();
        return $"ws://{address}:{WEBSOCKET_PORT}/";
    }

    public void CloseWiFi()
    {
        Listening = false;
        foreach (WebsocketClient client in clients)
        {
            client.IsReconnectionEnabled = false;
            Task.Run(() => client.Send("DISCONNECT"));
            GlobalData.LastMessages.Add($"Disconnected from Node {client.Name}");
            Thread.Sleep(100);
            client.Dispose();
        }
        clients.Clear();
        GlobalData.ConnectionReset();
    }
}