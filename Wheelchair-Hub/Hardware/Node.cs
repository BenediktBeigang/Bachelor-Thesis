using System.Net;
using Websocket.Client;

public class Node
{
    // public
    public ConnectionType ConnectionType { get; set; }
    public int DataPerSecond { get; set; }
    public int DataCount { get; set; }

    // readonlys
    // MUST
    public readonly DeviceNumber DeviceNumber;
    // CAN
    public readonly Gyro? Gyro;
    public readonly IPEndPoint? EndPoint;
    public readonly string? WebSocketURL;

    public Node(DeviceNumber device)
    {
        DeviceNumber = device;
        ConnectionType = ConnectionType.NOTHING;
    }

    public Node(DeviceNumber device, Gyro gyro, ConnectionType connection, IPEndPoint endPoint)
    {
        Gyro = gyro;
        DeviceNumber = device;
        EndPoint = endPoint;
        ConnectionType = connection;
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
}