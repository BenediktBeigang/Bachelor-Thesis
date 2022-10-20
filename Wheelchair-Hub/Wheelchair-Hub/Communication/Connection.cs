using System.Net;
using System.Timers;

public abstract class Connection
{
    private const int TIMEOUT = 10;
    private const string COM = "COM3";
    private const int BAUDRATE = 115200;
    public static Connection? _Connection { get; set; }

    public static Connection? SetConnection(ConnectionType connection)
    {
        switch (connection)
        {
            case ConnectionType.WIFI: return new WiFi();
            case ConnectionType.ESP_NOW: return new ESP_Now(COM, BAUDRATE);
            case ConnectionType.BLUETOOTH: return null;
            default: return null;
        }
    }

    /// <summary>
    /// Checks that connection with nodes is still alive, 
    /// by counting up when PackagesPerSecond is 0, 
    /// until 
    /// 1. new data is received => counter is reset
    /// 2. reconnection take too long => Disconnection is called/announced
    /// /// </summary>
    public static void Heartbeat(object sender, ElapsedEventArgs e)
    {
        if (_Connection is not null)
        {
            if (Node.Node_One.Check_Disconnect(TIMEOUT)) _Connection.Disconnect_Node(Node.Node_One);
            if (Node.Node_Two.Check_Disconnect(TIMEOUT)) _Connection.Disconnect_Node(Node.Node_Two);
        }
    }

    protected void Initialize_Node(ConnectionType connection, DeviceNumber device, bool wheelFlipped = false, IPEndPoint? sender = null)
    {
        Node node = new Node(device);
        switch (connection)
        {
            case ConnectionType.WIFI: node = new Node(device, connection, sender!, Gyro.Mode, wheelFlipped); break;
            case ConnectionType.ESP_NOW: node = new Node(device, connection, Gyro.Mode, wheelFlipped); break;
            case ConnectionType.BLUETOOTH: break;
        }
        WriteNodeInGlobalData(device, node);
    }

    private void WriteNodeInGlobalData(DeviceNumber device, Node node)
    {
        switch (device)
        {
            case DeviceNumber.ONE: Node.Node_One = node; break;
            case DeviceNumber.TWO: Node.Node_Two = node; break;
        }
    }

    protected abstract void Connect_ToHost();
    protected abstract void Disconnect_Node(Node node);
    public abstract void Disconnect_AllNodes();
    public abstract void Change_GyroMode(GyroMode mode);
}