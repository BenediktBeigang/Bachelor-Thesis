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
    public readonly Gyro Gyro;
    public readonly IPEndPoint? EndPoint;
    public readonly string? WebSocketURI;
    #endregion

    #region Initialization
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

    public void Reset()
    {
        GlobalData.LastMessages.Add($"Reset Node: {DeviceNumber.ToString()}");
        ConnectionType = ConnectionType.NOTHING;
        Client = null;
    }
    #endregion
}