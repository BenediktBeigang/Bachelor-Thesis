using System.Net;

public struct Received
{
    public IPEndPoint Sender;
    public string Message;
    public override string ToString() => $"IP: {Sender.Address.ToString()}\nPORT: {Sender.Port}\nMessage: {Message}";
}

public struct Message
{
    public string Text;
    public DateTime Time;
}

public struct Rotations
{
    public GyroSnapshot Left { get; set; }
    public GyroSnapshot Right { get; set; }
    public bool NodesFlipped;

    public Rotations(GyroSnapshot gyroOne, GyroSnapshot gyroTwo, bool nodesFlipped)
    {
        switch (nodesFlipped)
        {
            case false: Left = gyroOne; Right = gyroTwo; break;
            case true: Left = gyroTwo; Right = gyroOne; break;
        }
        NodesFlipped = nodesFlipped;
    }
}

