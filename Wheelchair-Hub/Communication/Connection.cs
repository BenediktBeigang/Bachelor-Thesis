using System.Net;
using System.Timers;

public abstract class Connection
{

    private const int MAXIMUM_DISCONNECTION_DURATION = 10;

    /// <summary>
    /// Checks that connection with nodes is still alive, 
    /// by counting up when PackagesPerSecond is 0, 
    /// until 
    /// 1. new data is received => counter is reset
    /// 2. reconnection take too long => Disconnection is called/announced
    /// /// </summary>
    public void Heartbeat()
    {
        Check_Node(GlobalData.Node_One);
        Check_Node(GlobalData.Node_Two);
    }

    /// <summary>
    /// Checks for disconnection. 
    /// When to often in a row zero packages per second are received, the Disconnectino function is called.
    /// The tolerated number of times with zero packages is defined by the constant MAXIMUM_DISCONNECTION_DURATION.
    /// </summary>
    /// <param name="node"></param>
    private void Check_Node(Node node)
    {
        if (node.ConnectionType is not ConnectionType.NOTHING)
        {
            if (node.DataPerSecond > 0)
                node.DisconnectionTime = 0;
            else
                node.DisconnectionTime++;
            if (node.DisconnectionTime > MAXIMUM_DISCONNECTION_DURATION)
            {
                Disconnect_Node(node);
            }
        }
    }

    protected void InitializeNode(ConnectionType connection, DeviceNumber device, GyroMode gyroMode, IPEndPoint? sender = null)
    {
        Node node = new Node(device);
        switch (connection)
        {
            case ConnectionType.WIFI: node = new Node(device, connection, sender!, gyroMode); break;
            case ConnectionType.ESP_NOW: node = new Node(device, connection, gyroMode); break;
            case ConnectionType.BLUETOOTH: break;
        }
        WriteNodeInGlobalData(device, node);
    }

    private void WriteNodeInGlobalData(DeviceNumber device, Node node)
    {
        switch (device)
        {
            case DeviceNumber.ONE: GlobalData.Node_One = node; break;
            case DeviceNumber.TWO: GlobalData.Node_Two = node; break;
        }
    }

    protected abstract void Disconnect_Node(Node node);
    public abstract void Disconnect_AllNodes();
    public abstract void ConnectToHost(GyroMode gyroMode);
}