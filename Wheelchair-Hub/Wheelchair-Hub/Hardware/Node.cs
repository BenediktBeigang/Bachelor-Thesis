using System.Net;
using System.Timers;
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

    #region Datarate
    public static void Update_Datarate_AllNodes(object sender, ElapsedEventArgs e)
    {
        if (Node.Node_One.ConnectionType is not ConnectionType.NOTHING)
            Node.Node_One.Update_Datarate(Loop.LOOP_DELAY_MESSAGEBENCHMARK);
        if (Node.Node_Two.ConnectionType is not ConnectionType.NOTHING)
            Node.Node_Two.Update_Datarate(Loop.LOOP_DELAY_MESSAGEBENCHMARK);
    }

    public void Update_Datarate(int timeBetweenCalls)
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

    public static Rotations Rotations()
    {
        short rawOne = Node_One.Gyro.Peek_RawValue();
        short rawTwo = Node_Two.Gyro.Peek_RawValue();
        double one = Node_One.Gyro.DegreePerSecond_Last();
        double two = Node_Two.Gyro.DegreePerSecond_Last();
        // double one = Node_One.Gyro.SmoothedDegreePerSecond_Last();
        // double two = Node_Two.Gyro.SmoothedDegreePerSecond_Last();
        switch (NodesFlipped)
        {
            case false: return new Rotations(rawOne, rawTwo, one, two);
            case true: return new Rotations(rawTwo, rawOne, two, one);
        }

        // short[] rawOne = Node_One.Gyro.Peek_RawValue(2);
        // short[] rawTwo = Node_Two.Gyro.Peek_RawValue(2);
        // // double one = Node_One.Gyro.SmoothedDegreePerSecond_Last();
        // // double two = Node_Two.Gyro.SmoothedDegreePerSecond_Last();
        // switch (NodesFlipped)
        // {
        //     case false: return new Rotations(rawOne, rawTwo, Gyro.StepsPerDegree);
        //     case true: return new Rotations(rawTwo, rawOne, Gyro.StepsPerDegree);
        // }
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