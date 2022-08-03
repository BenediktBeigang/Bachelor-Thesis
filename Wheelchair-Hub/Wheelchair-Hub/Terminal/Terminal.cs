using System;
using System.Collections.Concurrent;
using System.Timers;

public static class Terminal
{
    public static string Other { get; set; } = "";
    private static ConcurrentBag<Message> MessageHistory { get; set; } = new();
    private static Formatting format = new Formatting(new string[] { "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond", "DisconnectionTime" });
    private const int VISIBLE_MESSAGES = 5;

    public static void Print(object sender, ElapsedEventArgs e)
    {
        Console.Clear();
        string userInterface = InterfaceToString();
        Console.Write(userInterface);
    }

    public static string InterfaceToString()
    {
        string output = "";
        output += Generate_Table() + '\n';
        output += $"\n" + LegendToString() + '\n';
        output += $"Last Messages: " + '\n';
        output += MessageHistoryToString() + '\n';
        output += $"{Other}" + '\n';
        return output;
    }

    #region MessageHistory
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
    #endregion

    #region Table
    private static string Generate_Table()
    {
        string table = "";
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        table += String.Format($"{format.FormatString}", "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond", "DisconnectionTime") + '\n';
        table += String.Format($"{format.FormatString}", "----", "", "----------", "----------", "----------------", "------------------", "-----------------", "-----------------") + '\n';

        table += Generate_TableLine(Node.Node_One) + '\n';
        table += Generate_TableLine(Node.Node_Two) + '\n';
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        return table;
    }

    private static string Generate_TableLine(Node node)
    {
        string deviceNumber;
        switch (node.DeviceNumber)
        {
            case DeviceNumber.ONE: deviceNumber = "ONE"; break;
            case DeviceNumber.TWO: deviceNumber = "TWO"; break;
            default: return "";
        }
        return (node.ConnectionType is ConnectionType.NOTHING)
        ? String.Format($"{format.FormatString}", deviceNumber, "", "NOTHING", "", "", "NO GYRO", "", "")
        : String.Format($"{format.FormatString}",
        deviceNumber,
        "",
        node.ConnectionType,
        node.Gyro!.RawValue_Last(),
        node.Gyro!.DegreePerSecond_Last().ToString("0.00"),
        node.Gyro!.CalibrationStatus,
        node.DataPerSecond,
        node.DisconnectionTime);
    }
    #endregion

    #region String-Generator
    private static string MessageHistoryToString()
    {
        string output = "";
        List<string> messages = Get_MessageHistory();
        messages.Reverse();
        for (int i = 0; i < VISIBLE_MESSAGES; i++)
        {
            if (i == messages.Count) break;
            output += $"> {messages[i]}\n";
        }
        return output;
    }

    private static string LegendToString()
    {
        string output = "";
        output += $"'q'     - quit" + '\n';
        output += $"'c'     - calibrate gyros" + '\n';
        output += $"'f'     - flip:" + '\n';
        output += $" |==> n  - nodes" + '\n';
        output += $" |==> 1  - wheel one" + '\n';
        output += $" |==> 2  - wheel two" + '\n';
        output += $"'g'     - gyroMode" + '\n';
        output += $" |==> 0  - 250" + '\n';
        output += $" |==> 1  - 500" + '\n';
        output += $" |==> 2  - 1000" + '\n';
        output += $" |==> 3  - 2000" + '\n';
        output += $"'m'     - mapping" + '\n';
        output += $" |==> 1  - realisiticWheelchair" + '\n';
        output += $" |==> 2  - simpleWheelchair" + '\n';
        output += $" |==> 3  - gui" + '\n';
        return output;
    }
    #endregion
}