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
    public short RawLeft { get; set; }
    public short RawRight { get; set; }
    public double AngularVelocityLeft { get; set; }
    public double AngularVelocityRight { get; set; }

    public Rotations(short rL, short rR, double aVL, double aVR)
    {
        RawLeft = rL;
        RawRight = rR;
        AngularVelocityLeft = aVL;
        AngularVelocityRight = aVR;
    }

    public void MuteLeft()
    {
        RawLeft = 0;
        AngularVelocityLeft = 0;
    }

    public void MuteRight()
    {
        RawRight = 0;
        AngularVelocityRight = 0;
    }
}

