using System.Net;
using Websocket.Client;

public class Node
{
    #region Fields
    // static
    public static Node Node_One { get; set; } = new Node(DeviceNumber.ONE);
    public static Node Node_Two { get; set; } = new Node(DeviceNumber.TWO);
    public static List<Node> Nodes { get; set; } = new List<Node>() { Node_One, Node_Two };
    public static bool NodesFlipped { get; set; }

    // public
    public ConnectionType ConnectionType { get; set; }
    public int DataPerSecond { get; set; }
    public int DataCount { get; set; }
    public WebsocketClient? Client { get; set; }
    public int DisconnectionTime { get; set; } // in seconds
    public Gyro Gyro { get; set; }

    // readonlys
    // MUST
    public readonly DeviceNumber DeviceNumber;

    // CAN
    public readonly IPEndPoint? EndPoint;
    public readonly string? WebSocketURI;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructor when no connection is established (default).
    /// </summary>
    /// <param name="device"></param>
    public Node(DeviceNumber device, GyroMode gyroMode = GyroMode.GYRO_250, bool wheelFlipped = false)
    {
        DeviceNumber = device;
        ConnectionType = ConnectionType.NOTHING;
        Gyro = new Gyro(gyroMode, device, wheelFlipped);
    }

    /// <summary>
    /// Constructor for ESP-Now/SerialPort-Connection
    /// </summary>
    /// <param name="device"></param>
    /// <param name="gyro"></param>
    /// <param name="connection"></param>
    public Node(DeviceNumber device, ConnectionType connection, GyroMode gyroMode = GyroMode.GYRO_250, bool wheelFlipped = false)
    {
        DeviceNumber = device;
        ConnectionType = connection;
        Gyro = Gyro = new Gyro(gyroMode, device, wheelFlipped);
    }

    /// <summary>
    /// Constructor for WiFi-Connection.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="gyro"></param>
    /// <param name="connection"></param>
    /// <param name="endPoint"></param>
    public Node(DeviceNumber device, ConnectionType connection, IPEndPoint endPoint, GyroMode gyroMode = GyroMode.GYRO_250, bool wheelFlipped = false)
    {
        DeviceNumber = device;
        ConnectionType = connection;
        EndPoint = endPoint;
        Gyro = new Gyro(gyroMode, device, wheelFlipped);
    }
    #endregion

    #region StateChange
    public void Update_DataRate(int timeBetweenCalls)
    {
        DataPerSecond = DataCount * (1000 / timeBetweenCalls);
        DataCount = 0;
    }
    #endregion

    #region Reset
    public static void Reset_AllNodes()
    {
        Node_One.Reset();
        Node_Two.Reset();
    }

    public void Reset()
    {
        Terminal.Add_Message($"Reset Node: {DeviceNumber.ToString()}");
        ConnectionType = ConnectionType.NOTHING;
        Client = null;
    }
    #endregion

    public static (short RawLeft, short RawRight, double Left, double Right) Rotations()
    {
        short rawOne = Node_One.Gyro.RawValue_Last();
        short rawTwo = Node_Two.Gyro.RawValue_Last();
        double one = Node_One.Gyro.DegreePerSecond_Last();
        double two = Node_Two.Gyro.DegreePerSecond_Last();
        switch (NodesFlipped)
        {
            case false: return (RawLeft: rawOne, RawRight: rawTwo, Left: one, Right: two);
            case true: return (RawLeft: rawTwo, RawRight: rawOne, Left: two, Right: one);
        }
    }

    public static Node? Get_Node(DeviceNumber device)
    {
        switch (device)
        {
            case DeviceNumber.ONE: return Node_One;
            case DeviceNumber.TWO: return Node_Two;
            default: return null;
        }
    }

    public bool Check_Disconnect(int timeout)
    {
        if (ConnectionType is not ConnectionType.NOTHING)
        {
            if (DataPerSecond > 0)
                DisconnectionTime = 0;
            else
                DisconnectionTime++;
            if (DisconnectionTime > timeout)
            {
                return true;
            }
        }
        return false;
    }
}