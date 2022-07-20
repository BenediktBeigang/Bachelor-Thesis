using System.Timers;

public static class Program
{
    private static WiFi? wifiConnection;
    private static System.Timers.Timer? timer;
    private const int TIME_BETWEEN_CONSOLE_CALLS = 16;
    private const GyroMode GYRO_MODE = GyroMode.GYRO_2000;

    public static void Main(string[] args)
    {
        wifiConnection = new WiFi(); // "ws://ip:port/"
        wifiConnection.ConnectToHost();

        Loop();
        ExitCode();
    }

    private static void Loop()
    {
        timer = new System.Timers.Timer(TIME_BETWEEN_CONSOLE_CALLS);
        timer.Elapsed += Frame!;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private static void Frame(object sender, ElapsedEventArgs e)
    {
        ProgramStep();
        PrintConsole();
    }

    private static void PrintConsole()
    {
        if (wifiConnection is not null)
        {
            Console.Clear();
            Console.WriteLine("------------------------------------------------------------------");

            string table = "";
            table += String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|\n", "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status");
            table += String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|\n", "-----", "", "----------", "----------", "----------------", "------------------");
            table +=
            (GlobalData.Node_One.ConnectionType is ConnectionType.NOTHING)
            ? String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|\n", "ONE", "", "NOTHING", "", "", "NO GYRO")
            : String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|\n", "ONE", "", GlobalData.Node_One.ConnectionType, GlobalData.Node_One.Gyro!.RawValue, GlobalData.Node_One.Gyro!.DegreePerSecond().ToString("0.00"), GlobalData.Node_One.Gyro!.CalibrationStatus);
            table +=
            (GlobalData.Node_Two.ConnectionType is ConnectionType.NOTHING)
            ? String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|", "TWO", "", "NOTHING", "", "", "NO GYRO")
            : String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|", "TWO", "", GlobalData.Node_Two.ConnectionType, GlobalData.Node_Two.Gyro!.RawValue, GlobalData.Node_Two.Gyro!.DegreePerSecond().ToString("0.00"), GlobalData.Node_Two.Gyro!.CalibrationStatus);
            Console.WriteLine(table);

            Console.WriteLine("------------------------------------------------------------------\n");
            Console.WriteLine($"Last Messages: ");
            Console.WriteLine(LastMessagesString(10));
            Console.WriteLine($"\nPress 'q' to quit.\n");
            Console.WriteLine($"{GlobalData.other}");
        }
    }

    private static string LastMessagesString(int messageCount)
    {
        int listCount = GlobalData.LastMessages.Count;

        int messageCountMin = Math.Min(messageCount, listCount);
        int subListStart = listCount - messageCountMin;
        List<string> subList = GlobalData.LastMessages.GetRange(subListStart, messageCountMin);
        string output = "";
        for (int i = messageCountMin; i > 0; i--)
        {
            output += "> " + subList[i - 1] + "\n";
        }
        return output;
    }

    private static void ProgramStep()
    {
        wifiConnection!.Listening = (GlobalData.Node_One.ConnectionType is ConnectionType.NOTHING || GlobalData.Node_Two.ConnectionType is ConnectionType.NOTHING);
        CheckCalibration(GlobalData.Node_One!);
        CheckCalibration(GlobalData.Node_Two!);
    }

    private static void CheckCalibration(Node node)
    {
        if (node.ConnectionType is not ConnectionType.NOTHING && node.Gyro!.CalibrationStatus is CalibrationStatus.REQUESTED)
        {
            node.Gyro.Calibration(3);
        }
    }

    private static void ExitCode()
    {
        ConsoleKeyInfo k = Console.ReadKey();
        if (k.KeyChar is not 'q')
        {
            k = Console.ReadKey();
        }
        wifiConnection!.CloseWiFi();
        StopConsole();
    }

    // private static void ExitCode()
    // {
    //     int k = Console.Read();
    //     if (Convert.ToChar(k) is not 'q')
    //     {
    //         k = Console.Read();
    //     }
    //     wifiConnection!.CloseWiFi();
    //     StopConsole();
    // }

    private static void StopConsole()
    {
        GlobalData.LastMessages.Add("PROGRAM STOPPED");
        PrintConsole();
        Thread.Sleep(200);
        timer!.Stop();
        Environment.Exit(0);
    }
}
