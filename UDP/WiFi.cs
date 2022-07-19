using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
        IPEndPoint client = new IPEndPoint(IPAddress.Any, CLIENT_PORT);
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
            case "I am Node ONE":
                NewNode(ref NodeEndPoint_One, package, "one");
                break;
            case "I am Node TWO":
                NewNode(ref NodeEndPoint_Two, package, "two");
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
            case "1 Connected":
                GlobalData.LastMessages.Add("Client>> Node One Connected");
                GlobalData.NodeConnected_One = true;
                break;
            case "2 Connected":
                GlobalData.LastMessages.Add("Client>> Node Two Connected");
                GlobalData.NodeConnected_Two = true;
                break;
            default:
                HandleMessagePackage(message, client);
                break;
        }
    }

    private void HandleMessagePackage(string message, WebsocketClient client)
    {
        switch (client.Name)
        {
            case "one":
                GlobalData.RawValue_One = int.Parse(message);
                break;
            case "two":
                GlobalData.RawValue_Two = int.Parse(message);
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
        foreach (WebsocketClient client in clients)
        {
            client.IsReconnectionEnabled = false;
            Task.Run(() => client.Send("DISCONNECT"));
            Thread.Sleep(100);
            client.Dispose();
        }
        GlobalData.LastMessages.Add("Disconnected from ESPs");
        clients.Clear();
        GlobalData.NodeConnected_One = false;
        GlobalData.NodeConnected_Two = false;
        GlobalData.LastMessages.Add("PROGRAM STOPPED");
    }
}