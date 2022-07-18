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

    private WebsocketClient client;

    public WiFi()
    {
        IsListening = false;
        udpClient = new UdpClient();
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
        GlobalData.LastMessage = "UDP is listening!";
        while (IsListening)
        {
            var result = await udpClient.ReceiveAsync();
            Received received = new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
            GlobalData.LastMessage = $"Packet Received:\n{received.ToString()}";
            HandleIncomingPackage(received);
        }
    }

    private void HandleIncomingPackage(Received package)
    {
        switch (package.Message)
        {
            case "I am the left Node":
                NewNode(ref LeftNodeEndPoint, package);
                break;
            case "I am the right Node":
                NewNode(ref RightNodeEndPoint, package);
                break;
        }
    }

    private void NewNode(ref IPEndPoint nodeEndPoint_variable, Received package)
    {
        string url = WebSocketURL(package);
        GlobalData.LastMessage = $"Try to connect to Web-Socket:\n{url}";
        nodeEndPoint_variable = package.Sender;
        ConnectToWebSocketServer(url);
    }

    private void ConnectToWebSocketServer(string serverURL)
    {
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri(serverURL);

        using (client = new WebsocketClient(url))
        {
            client.ReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ReconnectionHappened.Subscribe(info =>
                GlobalData.LastMessage = $"Host>> Reconnection happened, type: {info.Type}\n");

            client.MessageReceived.Subscribe(msg => WebSocket_OnMessage(msg.Text));
            client.Start();

            Task.Run(() => client.Send("{ message }"));

            exitEvent.WaitOne();
        }
    }

    private void WebSocket_OnMessage(string message)
    {
        switch (message)
        {
            case "L Connected":
                GlobalData.LastMessage = "Client>> Left Node Connected";
                GlobalData.LeftNodeConnected = true;
                break;
            case "R Connected":
                GlobalData.LastMessage = "Client>> Right Node Connected";
                GlobalData.RightNodeConnected = true;
                break;
            default:
                HandleDataPackage(message);
                break;
        }
    }

    private void HandleDataPackage(string message)
    {
        char wheelside = message[0];
        // char hi = message[1];
        // char lo = message[2];
        // int value = lo | hi << 8;

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
        }
        GlobalData.LastMessage = message;
    }

    private string WebSocketURL(Received package)
    {
        string address = package.Sender.Address.ToString();
        string port = package.Sender.Port.ToString();
        return $"ws://{address}:{WEBSOCKET_PORT}/";
    }

    public void CloseWiFi()
    {
        if (client is not null)
            client.Dispose();
        IsListening = false;
    }
}