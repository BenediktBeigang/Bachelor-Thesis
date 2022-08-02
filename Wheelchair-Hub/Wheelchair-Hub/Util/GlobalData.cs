using System.Collections.Concurrent;
using System.Collections.Generic;

public static class GlobalData
{
    private static ConcurrentBag<Message> MessageHistory { get; set; } = new();
    public static Queue<Received> Packages { get; set; } = new();

    public static Node Node_One { get; set; } = new Node(DeviceNumber.ONE);
    public static Node Node_Two { get; set; } = new Node(DeviceNumber.TWO);
    public static List<Node> Nodes { get; set; } = new List<Node>() { Node_One, Node_Two };
    public static GyroMode GyroMode { get; set; } = GyroMode.GYRO_250;
    public static WheelchairMode WheelchairMode { get; set; }

    public static bool NodesFlipped { get; set; }
    public static bool Recording { get; set; }
    public static string other { get; set; } = "";

    public static void Add_Message(string message)
    {
        MessageHistory.Add(new Message
        {
            Text = message,
            Time = DateTime.Now
        });
    }

    public static List<string> Get_MessageHistory()
    {
        List<Message> messages = MessageHistory.ToList();
        messages.Sort((x, y) => DateTime.Compare(x.Time, y.Time));
        return messages.Select(x => x.Text).ToList<string>();
    }

    public static void Reset_AllNodes()
    {
        Node_One.Reset();
        Node_Two.Reset();
    }

    public static (short RawLeft, short RawRight, double Left, double Right) Rotations()
    {
        short rawOne = Node_One.Gyro.RawValue_Last();
        short rawTwo = Node_Two.Gyro.RawValue_Last();
        double one = Node_One.Gyro.DegreePerSecond_Last();
        double two = Node_Two.Gyro.DegreePerSecond_Last();
        switch (NodesFlipped)
        {
            case false: return (RawLeft: rawOne, RawRight: rawTwo, Left: one, Right: two);
            case true: return (RawLeft: rawTwo, RawRight: rawOne, Left: two, Right: one);
        }
    }


    public static int GyroModeInterger()
    {
        switch (GyroMode)
        {
            case GyroMode.GYRO_250: return 250;
            case GyroMode.GYRO_500: return 250;
            case GyroMode.GYRO_1000: return 250;
            case GyroMode.GYRO_2000: return 250;
            default: return 250;
        }
    }

    public static Node? Node_ByDeviceNumber(DeviceNumber device)
    {
        switch (device)
        {
            case DeviceNumber.ONE: return Node_One;
            case DeviceNumber.TWO: return Node_Two;
            default: return null;
        }
    }
}