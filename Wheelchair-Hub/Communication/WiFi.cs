using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

public class WiFi : Connection
{
    public bool Listening { get; set; }
    const int CLIENT_PORT = 11000;
    const int WEBSOCKET_PORT = 81;

    private UdpClient udpClient;
    private List<WebsocketClient> Clients;

    public WiFi()
    {
        Listening = false;
        Clients = new();
        udpClient = new UdpClient();
        IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, CLIENT_PORT);
        udpClient.Client.Bind(clientEndPoint);
    }

    /// <summary>
    /// Starts a Thread that waits for Broadcast of ESP to connect to it.
    /// </summary>
    public override void ConnectToHost()
    {
        Thread receiveThread = new Thread(new ThreadStart(this.Receive_UDP));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    /// <summary>
    /// Starts a UDP-Client that is listenting for a Broadcast. 
    /// When a Message is received a Handle-Function is called to react to the Message.
    /// </summary>
    private async void Receive_UDP()
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

    /// <summary>
    /// Handles an incoming message. 
    /// If the message comes from one of the Nodes (has correct message) a new Node is generated.
    /// </summary>
    /// <param name="package"></param>
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

    /// <summary>
    /// Generates a NewNode:
    /// 1. Initializes new Gyro
    /// 2. Initializes new Node and overrides old Node in GlobalData
    /// 3. Trys to connect to WebSocket with Information provided by package
    /// </summary>
    /// <param name="package"></param>
    /// <param name="device"></param>
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

        string uri = WebSocketURI(package);
        GlobalData.LastMessages.Add($"Try to connect to Web-Socket of Node {device.ToString()}: {uri}");
        Thread connectionThread = new Thread(() => ConnectToWebSocketServer(uri, device));
        connectionThread.IsBackground = true;
        connectionThread.Start();
    }

    /// <summary>
    /// Initializes new WebSocketClient and trys to connect to WebSocketServer.
    /// </summary>
    /// <param name="serverURL"></param>
    /// <param name="device"></param>
    /// <remarks>The client is given over to Node but if you try to get the client from somewhere else client is null.</remarks>
    private void ConnectToWebSocketServer(string serverURL, DeviceNumber device)
    {
        var exitEvent = new ManualResetEvent(false);
        var url = new Uri(serverURL);

        using (var client = new WebsocketClient(url))
        {
            Clients.Add(client);
            switch (device)
            {
                case DeviceNumber.ONE: GlobalData.Node_One.Client = client; break;
                case DeviceNumber.TWO: GlobalData.Node_Two.Client = client; break;
            }

            client.Name = device.ToString();
            client.IsReconnectionEnabled = false;
            client.ReconnectTimeout = TimeSpan.FromSeconds(5);
            client.ReconnectionHappened.Subscribe(info =>
                GlobalData.LastMessages.Add($"Host>> Reconnection happened, type: {info.Type}"));
            client.MessageReceived.Subscribe(msg => WebSocket_OnMessage(msg.Text, client));
            client.Start();
            exitEvent.WaitOne();
        }
    }

    /// <summary>
    /// Handles an incoming message from the WebSocketServer, based on the message.
    /// Two cases are expected:
    /// 1. ESP wants to confirm connection => starting Gyro-Calibration 
    /// 2. Other information (probably data) => Call HandleMessagePackage()
    /// </summary>
    /// <param name="message"></param>
    /// <param name="client"></param>
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

    /// <summary>
    /// Interprets message, based on who sends the message.
    /// If the message is coming from one of the Nodes, it is expected that it is data, 
    /// because all messages are intercepted befor by the WebSocket_OnMessage function.
    /// With data following things happen:
    /// 1. Message is parsed to int-value and written in Node
    /// 2. The dataCount for this second is incremented
    /// </summary>
    /// <param name="message"></param>
    /// <param name="client"></param>
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

    private static string WebSocketURI(Received package)
    {
        string address = package.Sender.Address.ToString();
        string port = package.Sender.Port.ToString();
        return $"ws://{address}:{WEBSOCKET_PORT}/";
    }

    /// <summary>
    /// Stops Node-Connection over WiFi.
    /// 1. All Connections with WebSocketServers are closed/stopped
    /// 2. The client-objects in Clients are removed
    /// 3. The Nodes are reset to there inital state
    /// </summary>
    public override void Disconnect_AllNodes()
    {
        Listening = false;
        foreach (WebsocketClient client in Clients)
        {
            client.IsReconnectionEnabled = false;
            Task.Run(() => client.Stop(WebSocketCloseStatus.NormalClosure, "Programm closing"));
            GlobalData.LastMessages.Add($"Disconnected from Node {client.Name}");
            // Thread.Sleep(100);
        }
        Clients.Clear();
        GlobalData.Reset_AllNodes();
    }

    /// <summary>
    /// Closes/Stops the connection with WebSocketServer.
    /// Resets the associated Node.
    /// </summary>
    /// <param name="node"></param>
    protected override void Disconnect_Node(Node node)
    {
        WebsocketClient client = Clients.First(c => c.Name == node.DeviceNumber.ToString());
        client.IsReconnectionEnabled = false;
        Task.Run(() => client!.Stop(WebSocketCloseStatus.EndpointUnavailable, "Lost Connection"));
        GlobalData.LastMessages.Add($"Lost connection to Node {node.DeviceNumber.ToString()}");
        Clients.Remove(client!);
        node.Reset();
    }

    // /// <summary>
    // /// Checks that connection with nodes is still alive, 
    // /// when Node is indicating that it is connected over WiFi.
    // /// When MessageReceived-Flag is true in Node the connection is still alive and set to false.
    // /// When MessageReceived-Flag is false connection is lost and Kill_Client() is called to announce that connection is lost.
    // /// /// </summary>
    // public void Heartbeat()
    // {
    //     foreach (Node node in GlobalData.Nodes)
    //     {
    //         if (node.ConnectionType is not ConnectionType.NOTHING)
    //         {
    //             GlobalData.LastMessages.Add($"HEARTBEAT: {node.DeviceNumber.ToString()}");
    //             if (node.MessageReceived)
    //                 node.MessageReceived = false;
    //             else
    //             {
    //                 node.MessageReceived = false;
    //                 Kill_Client(node);
    //             }
    //         }
    //     }
    // }

    // /// <summary>
    // /// Kills the connection with WebSocketServer.
    // /// Resets the associated Node.
    // /// </summary>
    // /// <param name="node"></param>
    // private void Kill_Client(Node node)
    // {
    //     WebsocketClient client = Clients.First(c => c.Name == node.DeviceNumber.ToString());
    //     GlobalData.LastMessages.Add($"Lost connection to Node {node.DeviceNumber.ToString()}");
    //     Task.Run(() => client!.Stop(WebSocketCloseStatus.EndpointUnavailable, "Lost Connection"));
    //     Clients.Remove(client!);
    //     node.Reset();
    // }
}