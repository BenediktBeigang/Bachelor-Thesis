using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
// using WebSocketSharp;
// using WebSocket4Net;
// using esegece.sgcWebSockets;
using Websocket.Client;

public class WiFi
{
    private IPEndPoint LeftNodeEndPoint;
    private IPEndPoint RightNodeEndPoint;

    public bool LeftNodeConnected { get; set; }
    public bool RightNodeConnected { get; set; }

    // public WebSocket socket { get; set; }
    // public TsgcWebSocketClient socket;

    private UdpClient udpClient;
    public bool IsListening { get; set; }
    const int CLIENT_PORT = 11000;
    const int WEBSOCKET_PORT = 81;

    public string LastMessage { get; set; }
    public int GyroValue { get; set; }

    public struct Received
    {
        public IPEndPoint Sender;
        public string Message;
        public override string ToString() => $"IP: {Sender.Address.ToString()}\nPORT: {Sender.Port}\nMessage: {Message}\n";
    }

    public WiFi()
    {
        IsListening = false;
        udpClient = new UdpClient();
        IPEndPoint client = new IPEndPoint(IPAddress.Any, CLIENT_PORT); // <<<< notwendig?
        udpClient.Client.Bind(client);
    }

    public async void ConnectToHost()
    {
        IsListening = true;
        Console.WriteLine("UDP is listening!");
        while (IsListening)
        {
            var result = await udpClient.ReceiveAsync();
            Received received = new Received()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
            Console.WriteLine("Packet Received:");
            Console.WriteLine(received.ToString());
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
        Console.WriteLine($"Try to connect to Web-Socket:\n{url}\n");
        nodeEndPoint_variable = package.Sender;
        ConnectToWebSocketServer(url);
    }

    private void ConnectToWebSocketServer(string serverURL)
    {
        // socket = new WebSocket(serverURL);
        // socket.OnMessage += WebSocket_OnMessage;
        // socket.Connect();

        // socket.OnMessage = WebSocket_OnMessage;
        // socket.MessageReceived += WebSocket_OnMessage;
        // socket.Open();
        //----------------------------------------------------
        // TsgcWebSocketClient socket = new TsgcWebSocketClient();
        // // socket.Host = "ws://192.168.2.116";
        // // socket.Port = 81;
        // socket.URL = serverURL;
        // socket.OnConnect += WebSocket_OnConnect;
        // socket.OnMessage += WebSocket_OnMessage;

        // socket.Active = true;
        //----------------------------------------------------
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri(serverURL);

        using (var client = new WebsocketClient(url))
        {
            client.ReconnectTimeout = TimeSpan.FromSeconds(30);
            client.ReconnectionHappened.Subscribe(info =>
                Console.WriteLine($"Host>> Reconnection happened, type: {info.Type}\n"));

            client.MessageReceived.Subscribe(msg => WebSocket_OnMessage(msg.Text));
            client.Start();

            Task.Run(() => client.Send("{ message }"));

            exitEvent.WaitOne();
        }
    }

    // private void WebSocket_OnInfo(ReconnectionType info)
    // {
    //     switch (info)
    //     {
    //         case ReconnectionType.Lost:
    //             Console.WriteLine("Host>> Connection Lost\n");
    //             LeftNodeConnected = false;
    //             break;
    //     }
    // }

    private void WebSocket_OnMessage(string message)
    {
        switch (message)
        {
            case "L Connected":
                Console.WriteLine("Client>> Left Node Connected\n");
                LeftNodeConnected = true;
                break;
            case "R Connected":
                Console.WriteLine("Client>> Right Node Connected\n");
                RightNodeConnected = true;
                break;
            default:
                char wheelside = message[0];
                // char hi = message[1];
                // char lo = message[2];
                // int value = lo | hi << 8;

                string value = "";
                for (int i = 1; i < 7; i++)
                {
                    value = (message[i] == ' ') ? value : value + message[i];
                }

                LastMessage = message;
                GyroValue = int.Parse(value);

                // Console.WriteLine($"Host>> Message: {message}");
                // Console.WriteLine($"Host>> Wheel: {value}");
                // Console.WriteLine($"Message: {message}\nWheelside: {wheelside}\nHi: {(int)hi}\nLo: {(int)lo}");
                // Console.WriteLine($"Host>> Length: {message.Length}\n");
                break;

                // Console.WriteLine($"Host>> Received from the server: {message}\nTime: {DateTime.Now}\n");
                // break;
        }
    }

    private string WebSocketURL(Received package)
    {
        string address = package.Sender.Address.ToString();
        string port = package.Sender.Port.ToString();
        return $"ws://{address}:{WEBSOCKET_PORT}/";
    }
}