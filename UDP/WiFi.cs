using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Websocket.Client;

public class WiFi
{
    private IPEndPoint LeftNodeEndPoint = new IPEndPoint(new IPAddress(0), 0);
    private IPEndPoint RightNodeEndPoint = new IPEndPoint(new IPAddress(0), 0);

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
        IPEndPoint client = new IPEndPoint(IPAddress.Any, CLIENT_PORT); // <<<< notwendig?
        udpClient.Client.Bind(client);
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
            case "I am the left Node":
                NewNode(ref LeftNodeEndPoint, package, "left");
                break;
            case "I am the right Node":
                NewNode(ref RightNodeEndPoint, package, "right");
                break;
        }
    }

    private void NewNode(ref IPEndPoint nodeEndPoint_variable, Received package, string wheelside)
    {
        string url = WebSocketURL(package);
        GlobalData.LastMessages.Add($"Try to connect to Web-Socket: {url}");
        nodeEndPoint_variable = package.Sender;
        ConnectToWebSocketServer(url, wheelside);
    }

    private void ConnectToWebSocketServer(string serverURL, string wheelside)
    {
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri(serverURL);

        using (var client = new WebsocketClient(url))
        {
            clients.Add(client);
            client.Name = wheelside;
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
            case "L Connected":
                GlobalData.LastMessages.Add("Client>> Left Node Connected");
                GlobalData.LeftNodeConnected = true;
                break;
            case "R Connected":
                GlobalData.LastMessages.Add("Client>> Right Node Connected");
                GlobalData.RightNodeConnected = true;
                break;
            default:
                HandleMessagePackage(message);
                break;
        }
    }

    private void HandleMessagePackage(string message)
    {
        char wheelside = message[0];
        string value = "";

        for (int i = 1; i < 7; i++)
        {
            value = (message[i] == ' ') ? value : value + message[i];
        }

        switch (wheelside)
        {
            case 'L':
                GlobalData.LeftValue = int.Parse(value);
                break;
            case 'R':
                GlobalData.RightValue = int.Parse(value);
                break;
            default:
                GlobalData.LastMessages.Add(message);
                break;
        }
    }

    private string WebSocketURL(Received package)
    {
        string address = package.Sender.Address.ToString();
        string port = package.Sender.Port.ToString();
        return $"ws://{address}:{WEBSOCKET_PORT}/";
    }

    public void CloseWiFi()
    {
        GlobalData.LastMessages.Add("Disconnected from ESP");
        foreach (WebsocketClient client in clients)
        {
            client.IsReconnectionEnabled = false;
            var exitEvent = new ManualResetEvent(false);
            Task.Run(() => client.Send("DISCONNECT"));
            exitEvent.WaitOne();
            client.Dispose();
        }
        clients.Clear();
        GlobalData.LastMessages.Add("PROGRAM STOPPED");
    }

}