using System.Net;
using Websocket.Client;

public class Node
{
    #region Fields
    // public
    public ConnectionType ConnectionType { get; set; }
    public int DataPerSecond { get; set; }
    public int DataCount { get; set; }
    public WebsocketClient? Client { get; set; }
    public int DisconnectionTime { get; set; } // in seconds

    // readonlys
    // MUST
    public readonly DeviceNumber DeviceNumber;

    // CAN
    public readonly Gyro? Gyro;
    public readonly IPEndPoint? EndPoint;
    public readonly string? WebSocketURI;
    #endregion

    #region Initialization
    /// <summary>
    /// Constructor when no connection is established (default).
    /// </summary>
    /// <param name="device"></param>
    public Node(DeviceNumber device)
    {
        DeviceNumber = device;
        ConnectionType = ConnectionType.NOTHING;
    }

    /// <summary>
    /// Constructor for WiFi-Connection.
    /// </summary>
    /// <param name="device"></param>
    /// <param name="gyro"></param>
    /// <param name="connection"></param>
    /// <param name="endPoint"></param>
    public Node(DeviceNumber device, Gyro gyro, ConnectionType connection, IPEndPoint endPoint)
    {
        DeviceNumber = device;
        Gyro = gyro;
        ConnectionType = connection;
        EndPoint = endPoint;
    }
    #endregion

    #region StateChange
    public void Update_DataRate(int timeBetweenCalls)
    {
        DataPerSecond = DataCount * (1000 / timeBetweenCalls);
        DataCount = 0;
    }

    public void Reset()
    {
        GlobalData.LastMessages.Add($"Reset Node: {DeviceNumber.ToString()}");
        ConnectionType = ConnectionType.NOTHING;
        Client = null;
    }
    #endregion
}