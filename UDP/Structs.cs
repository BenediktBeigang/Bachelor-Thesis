using System.Net;

public struct Received
{
    public IPEndPoint Sender;
    public string Message;
    public override string ToString() => $"IP: {Sender.Address.ToString()}\nPORT: {Sender.Port}\nMessage: {Message}\n";
}