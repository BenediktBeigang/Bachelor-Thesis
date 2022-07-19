using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Websocket.Client;

public class WiFi
{
    private IPEndPoint NodeEndPoint_One = new IPEndPoint(new IPAddress(0), 0);
    private IPEndPoint NodeEndPoint_Two = new IPEndPoint(new IPAddress(0), 0);

    private UdpClient udpClient;
    public bool IsListening { get; set; }
    const int CLIENT_PORT = 11000;
    const int WEBSOCKET_PORT = 81;

    private List<WebsocketClient> clients;

    public WiFi()
    {
        IsListening = false;
        udpClient = new UdpClient();
        clients = new();
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
        IsListening = true;
        GlobalData.LastMessages.Add("UDP is listening!");
        while (IsListening)
        {
            var result = await udpClient.ReceiveAsync();
            Received received = new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
            GlobalData.LastMessages.Add($"Packet Received:\n{received.ToString()}");
            HandleIncomingPackage(received);
        }
        GlobalData.LastMessages.Add("UDP stopped listening!");
    }

    private void HandleIncomingPackage(Received package)
    {
        switch (package.Message)
        {
            case "I am Node ONE":
                NewNode(ref NodeEndPoint_One, package, DeviceNumber.ONE);
                break;
            case "I am Node TWO":
                NewNode(ref NodeEndPoint_Two, package, DeviceNumber.TWO);
                break;
        }
    }

    private void NewNode(ref IPEndPoint nodeEndPoint_variable, Received package, DeviceNumber device)
    {
        Gyro gyro = new Gyro(GyroMode.Gyro_2000, device);
        switch (device)
        {
            case DeviceNumber.ONE:
                GlobalData.Node_One = new Node(ConnectionType.WIFI, gyro, device, package.Sender);
                break;
            case DeviceNumber.TWO:
                GlobalData.Node_Two = new Node(ConnectionType.WIFI, gyro, device, package.Sender);
                break;
        }

        string url = WebSocketURL(package);
        GlobalData.LastMessages.Add($"Try to connect to Web-Socket of Node {device.ToString()}: {url}");
        ConnectToWebSocketServer(url, device);
    }

    private WebsocketClient ConnectToWebSocketServer(string serverURL, DeviceNumber device)
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
            return client;
        }
    }

    private void WebSocket_OnMessage(string message, WebsocketClient client)
    {
        switch (message)
        {
            case "1 Connected":
                GlobalData.LastMessages.Add("Client>> Node One Connected");
                if (GlobalData.Node_One is not null)
                    GlobalData.Node_One!.ConnectedWithWebsocket = true;
                break;
            case "2 Connected":
                GlobalData.LastMessages.Add("Client>> Node Two Connected");
                if (GlobalData.Node_Two is not null)
                    GlobalData.Node_Two!.ConnectedWithWebsocket = true;
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
                    if (GlobalData.Node_One is not null)
                        GlobalData.Node_One.Gyro.RawValue = int.Parse(message);
                    break;
                case "TWO":
                    if (GlobalData.Node_Two is not null)
                        GlobalData.Node_Two.Gyro.RawValue = int.Parse(message);
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
        IsListening = false;
        foreach (WebsocketClient client in clients)
        {
            client.IsReconnectionEnabled = false;
            Task.Run(() => client.Send("DISCONNECT"));
            GlobalData.LastMessages.Add($"Disconnected from Node {client.Name}");
            Thread.Sleep(100);
            client.Dispose();
        }
        clients.Clear();
        GlobalData.LastMessages.Add("PROGRAM STOPPED");
    }
}