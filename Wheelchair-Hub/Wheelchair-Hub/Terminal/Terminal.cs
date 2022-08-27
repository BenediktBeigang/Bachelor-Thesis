using System;
using System.Collections.Concurrent;
using System.Timers;

public static class Terminal
{
    public static string Other { get; set; } = "";
    private static ConcurrentBag<Message> MessageHistory { get; set; } = new();
    private static ConcurrentBag<Message> CommandHistory { get; set; } = new();
    private const int VISIBLE_MESSAGES = 5;
    private const string FORMAT_STRING_TABLELINE = "|{0,4}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|{6,17}|{7,17}|";
    private const string FORMAT_STRING_INFOHEAD = "|{0,-11}||{1,-24}||{2,-29}||{3,-32}||{4,-18}|";
    private const string FORMAT_STRING_INFO = "|{0,-11}||{1,-24}||{2,-7}|{3,-10}|{4,-10}||{5,-15}|{6,-16}||{7,-18}|";

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
        output += $"{Other}" + '\n' + '\n';
        output += "Last Messages: " + '\n';
        output += HistoryToString(Get_MessageHistory(), true) + '\n';
        output += "------------------------------------------" + '\n';
        output += "Last Commands: " + '\n';
        output += HistoryToString(Get_CommandHistory(), false) + '\n';
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
    #endregion

    #region Table
    private static string Generate_Table()
    {
        string table = "";
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        table += String.Format($"{FORMAT_STRING_TABLELINE}", "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond", "DisconnectionTime") + '\n';
        table += String.Format($"{FORMAT_STRING_TABLELINE}", "----", "", "----------", "----------", "----------------", "------------------", "-----------------", "-----------------") + '\n';

        table += Generate_TableLine(Node.Node_One) + '\n';
        table += Generate_TableLine(Node.Node_Two) + '\n';
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        table += "----------------------------------------------------------------------------------------------------------------------------" + "\n";
        table += Generate_InfoLine() + "\n";
        table += "----------------------------------------------------------------------------------------------------------------------------" + "\n";
        table += "--------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + "\n";
        table += LegendToStringFlat();
        table += "--------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
        return table;
    }

    private static string Generate_InfoLine()
    {
        string output = "";
        output += String.Format(FORMAT_STRING_INFOHEAD, "", "", "         Flipped", "          Threshold", "") + '\n';
        output += String.Format(FORMAT_STRING_INFO, " GYRO-MODE", " MAPPING-MODE", " Nodes", " Node-One", " Node-Two", " WheelMovement", " ButtonPressing", " WheelMovementMax") + '\n';
        output += String.Format(FORMAT_STRING_INFO,
        $" {Gyro.Mode}", $" {Mapping.Get_Mode().ToString()}",
        $" {Node.NodesFlipped}", $" {Node.Node_One.Gyro.RotationValueFlip}", $" {Node.Node_Two.Gyro.RotationValueFlip}",
        $" {Mapping.Get_WheelMovementThreshold()}", $" {Mapping.Get_ButtonPressingThreshold()}",
        $" {Mapping.Get_WheelMovement_Max()}");
        return output;
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
        ? String.Format($"{FORMAT_STRING_TABLELINE}",
        deviceNumber,
        "",
        "NOTHING",
        node.Gyro!.Peek_RawValue(),
        node.Gyro!.DegreePerSecond_Last().ToString("0.00"),
        "NO GYRO",
        "0",
        "0")
        : String.Format($"{FORMAT_STRING_TABLELINE}",
        deviceNumber,
        "",
        node.ConnectionType,
        node.Gyro!.Peek_RawValue(),
        node.Gyro!.DegreePerSecond_Last().ToString("0.00"),
        node.Gyro!.CalibrationStatus,
        node.DataPerSecond,
        node.DisconnectionTime);
    }
    #endregion

    #region String-Generator
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

    private static string LegendToStringFlat()
    {
        string output = "";
        output += "|| q - quit || c - calibrate gyros || f - flip                                  || m - mapping                                                                          ||\n";
        output += "||          ||                     || n - nodes | 1 - wheel one | 2 - wheel two || 1 - realisticWheelchair | 2 - simpleWheelchair | 3 - wheelchairWithButtons | 4 - gui ||\n";
        return output;
    }
    #endregion
}