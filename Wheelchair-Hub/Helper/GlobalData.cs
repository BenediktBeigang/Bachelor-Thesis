using System.Collections.Concurrent;

public static class GlobalData
{
    public static List<string> LastMessages { get; set; } = new();
    public static Queue<Received> Packages { get; set; } = new();

    public static Node Node_One { get; set; } = new Node(DeviceNumber.ONE);
    public static Node Node_Two { get; set; } = new Node(DeviceNumber.TWO);
    public static List<Node> Nodes { get; set; } = new List<Node>() { Node_One, Node_Two };

    public static string other { get; set; } = "";

    public static void ConnectionReset()
    {
        Node_One = new Node(DeviceNumber.ONE);
        Node_Two = new Node(DeviceNumber.TWO);
        Nodes.Clear();
        Nodes.Add(Node_One);
        Nodes.Add(Node_Two);
    }
}