using System.Net;
using Websocket.Client;

public class Node
{
    // public
    public bool ConnectedWithWebsocket { get; set; }

    // readonlys
    // MUST
    public readonly DeviceNumber DeviceNumber;
    public readonly Gyro Gyro;
    public readonly ConnectionType ConnectionType;
    // CAN
    public readonly IPEndPoint? EndPoint;

    public Node(ConnectionType connection, Gyro gyro, DeviceNumber device, IPEndPoint endPoint)
    {
        Gyro = gyro;
        ConnectionType = connection;
        DeviceNumber = device;
        EndPoint = endPoint;
    }

    public
    (string Device,
    string Connection,
    string Connected,
    string RawValue,
    string DegreePerSecond)
    ToStringTupel()
    {
        return
        (DeviceNumber.ToString(),
        ConnectionType.ToString(),
        ConnectedWithWebsocket.ToString(),
        Gyro.RawValue.ToString(),
        Gyro.DegreePerSecond().ToString());
    }
}