using System.Net;
using Websocket.Client;

public class Node
{
    // public
    public ConnectionType ConnectionType { get; set; }
    public int DataPerSecond { get; set; }
    public int DataCount { get; set; }
    public WebsocketClient? Client { get; set; }
    public int DisconnectionTime { get; set; } // in seconds

    // readonlys
    // MUST
    public readonly DeviceNumber DeviceNumber;
    // public static implicit operator string(DeviceNumber device)
    // {
    //     return device.ToString();
    // }
    // CAN
    public readonly Gyro? Gyro;
    public readonly IPEndPoint? EndPoint;
    public readonly string? WebSocketURI;

    public Node(DeviceNumber device)
    {
        DeviceNumber = device;
        ConnectionType = ConnectionType.NOTHING;
    }

    public Node(DeviceNumber device, Gyro gyro, ConnectionType connection, IPEndPoint endPoint)
    {
        DeviceNumber = device;
        Gyro = gyro;
        ConnectionType = connection;
        EndPoint = endPoint;
    }

    public (string Device, string Connection, string RawValue, string DegreePerSecond) ToStringTupel()
    {
        string device = DeviceNumber.ToString();
        string connection = ConnectionType.ToString();
        string raw = "";
        string degreePerSecond = "";

        if (ConnectionType is not ConnectionType.NOTHING)
        {
            raw = Gyro!.LastRawValue.ToString();
            degreePerSecond = Gyro!.DegreePerSecond().ToString();
        }

        return (device, connection, raw, degreePerSecond);
    }

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
}