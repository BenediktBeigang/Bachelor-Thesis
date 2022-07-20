using System.Net;
using Websocket.Client;

public class Node
{
    // public
    public ConnectionType ConnectionType { get; set; }

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
            raw = Gyro!.RawValue.ToString();
            degreePerSecond = Gyro!.DegreePerSecond().ToString();
        }

        return (device, connection, raw, degreePerSecond);
    }
}