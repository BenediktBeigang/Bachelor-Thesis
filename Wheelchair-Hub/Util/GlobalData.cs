using System.Collections.Concurrent;
using System.Collections.Generic;

public static class GlobalData
{
    public static ConcurrentBag<string> LastMessages { get; set; } = new();
    public static Queue<Received> Packages { get; set; } = new();
    public static Node Node_One { get; set; } = new Node(DeviceNumber.ONE);
    public static Node Node_Two { get; set; } = new Node(DeviceNumber.TWO);
    public static List<Node> Nodes { get; set; } = new List<Node>() { Node_One, Node_Two };
    public static bool NodesFlipped { get; set; }
    public static string other { get; set; } = "";

    public static void Reset_AllNodes()
    {
        Node_One.Reset();
        Node_Two.Reset();
    }

    public static (double Left, double Right) Rotations()
    {
        double one = 0;
        double two = 0;

        if (Node_One.Gyro is not null)
            one = Node_One.Gyro.DegreePerSecond();

        if (Node_Two.Gyro is not null)
            two = Node_Two.Gyro.DegreePerSecond();

        switch (NodesFlipped)
        {
            case false: return (Left: one, Right: two);
            case true: return (Left: two, Right: one);
        }
    }
}