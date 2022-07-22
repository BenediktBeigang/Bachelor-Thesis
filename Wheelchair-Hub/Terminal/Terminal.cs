using System;

public static class Terminal
{
    private static Formatting format = new Formatting(new string[] { "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond", "DisconnectionTime" });

    public static string Print()
    {
        Console.Clear();
        string output = "";
        output += Generate_Table() + '\n';
        output += $"\nPress 'q' to quit.\n" + '\n';
        output += $"Last Messages: " + '\n';
        output += LastMessagesString() + '\n';
        output += $"{GlobalData.other}" + '\n';
        Console.Write(output);
        return output;
    }

    #region Helper
    private static string Generate_Table()
    {
        string table = "";
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        table += String.Format($"{format.FormatString}", "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond", "DisconnectionTime") + '\n';
        table += String.Format($"{format.FormatString}", "----", "", "----------", "----------", "----------------", "------------------", "-----------------", "-----------------") + '\n';

        table += Generate_TableLine(GlobalData.Node_One) + '\n';
        table += Generate_TableLine(GlobalData.Node_Two) + '\n';
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
        node.Gyro!.DegreePerSecond().ToString("0.00"),
        node.Gyro!.CalibrationStatus,
        node.DataPerSecond,
        node.DisconnectionTime);
    }

    private static string LastMessagesString()
    {
        string output = "";
        string[] messages = GlobalData.LastMessages.ToArray();
        for (int i = 0; i < messages.Length; i++)
        {
            output += $"> {messages[i]}\n";
        }
        return output;
    }
    #endregion
}