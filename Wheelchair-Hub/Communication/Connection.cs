using System.Timers;

public abstract class Connection : ICommunication
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

    protected abstract void Disconnect_Node(Node node);
    public abstract void Disconnect_AllNodes();
    public abstract void ConnectToHost();
}