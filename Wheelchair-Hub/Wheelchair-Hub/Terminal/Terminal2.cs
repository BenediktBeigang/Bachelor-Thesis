using System;
using System.Collections.Concurrent;
using System.Timers;

public static class Terminal2
{
    public static string Other { get; set; } = "";
    private static ConcurrentBag<Message> MessageHistory { get; set; } = new();
    private static ConcurrentBag<Message> CommandHistory { get; set; } = new();
    private const int VISIBLE_MESSAGES = 5;

    public static void Print(object sender, ElapsedEventArgs e)
    {
        Print_Other();
        Print_TableLines(3, Node.Node_One);
        Print_TableLines(4, Node.Node_Two);
        Print_InfoLine();
        Reset_Cursor();
    }

    private static void Print_InfoLine()
    {
        string[] InfoLineInput = Get_InfoLineInput();
        for (int i = 0; i < InfoLineInput.Length; i++)
        {
            Console.SetCursorPosition(Templates.InfoLineValuePositions[i], 9);
            Console.Write(" " + InfoLineInput[i]);
        }
    }

    private static void Print_TableLines(int line, Node node)
    {
        string[] TableLineInput = Get_TableLineInput(node);
        for (int i = 0; i < TableLineInput.Length; i++)
        {
            Console.SetCursorPosition(Templates.TableLineValuePositions[i], line);
            Console.Write(" " + TableLineInput[i]);
        }
    }

    private static string[] Get_InfoLineInput()
    {
        return new string[]
        {
            Gyro.Mode.ToString(),
            Mapping.Get_Mode().ToString(),
            Node.NodesFlipped.ToString(),
            Node.Node_One.Gyro.RotationValueFlip.ToString(),
            Node.Node_Two.Gyro.RotationValueFlip.ToString(),
            Mapping.Get_WheelMovementThreshold().ToString(),
            Mapping.Get_ButtonPressingThreshold().ToString(),
            Mapping.Get_WheelMovement_Max().ToString()
        };
    }

    private static string[] Get_TableLineInput(Node node)
    {
        if (node.ConnectionType is ConnectionType.NOTHING)
            return new string[] { node.DeviceNumber.ToString(), "", "NOTHING", "", "", "NO GYRO", "", "" };
        return new string[]
        {
            node.DeviceNumber.ToString(),
            node.ConnectionType.ToString(),
            node.Gyro.Peek_RawValue().ToString(),
            node.Gyro.DegreePerSecond_Last().ToString(),
            node.Gyro.CalibrationStatus.ToString(),
            node.DataPerSecond.ToString(),
            node.DisconnectionTime.ToString()
        };
    }

    private static void Print_Other()
    {
        Console.SetCursorPosition(0, 15);
        Console.Write(Other + "                                     ");
    }

    private static void Reset_Cursor()
    {
        Console.SetCursorPosition(0, 40);
    }

    public static void PrintTemplate()
    {
        Templates.Template();
    }

    public static void Add_Message(string message)
    {
        MessageHistory.Add(new Message
        {
            Text = message,
            Time = DateTime.Now
        });
    }

    public static void Add_Command(string message)
    {
        CommandHistory.Add(new Message
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

    public static List<string> Get_CommandHistory()
    {
        List<Message> messages = CommandHistory.ToList();
        messages.Sort((x, y) => DateTime.Compare(x.Time, y.Time));
        return messages.Select(x => x.Text).ToList<string>();
    }

    private static string HistoryToString(List<string> messages, bool flip)
    {
        string output = "";
        if (flip) messages.Reverse();
        for (int i = 0; i < VISIBLE_MESSAGES; i++)
        {
            if (i == messages.Count) break;
            output += $"> {messages[i]}\n";
        }
        return output;
    }
}