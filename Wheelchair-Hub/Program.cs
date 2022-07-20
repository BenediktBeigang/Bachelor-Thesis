using System.Timers;

public static class Program
{
    private static WiFi? wifiConnection;
    private static System.Timers.Timer? timer;
    private static Formatting Formatting;
    private const int TIME_BETWEEN_CONSOLE_CALLS = 16;
    private const GyroMode GYRO_MODE = GyroMode.GYRO_2000;

    public static void Main(string[] args)
    {
        Formatting = new Formatting(new string[] { "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond" });
        new Benchmark();
        wifiConnection = new WiFi(); // "ws://ip:port/"
        wifiConnection.ConnectToHost();

        Loop();
        Exit_Code();
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
            Console.WriteLine("-----------------------------------------------------------------------------------");

            string table = "";
            table += String.Format($"{Formatting.FormatString}", "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond") + '\n';
            table += String.Format($"{Formatting.FormatString}", "----", "", "----------", "----------", "----------------", "------------------", "-----------------") + '\n';

            table += (GlobalData.Node_One.ConnectionType is ConnectionType.NOTHING)
            ? String.Format($"{Formatting.FormatString}", "ONE", "", "NOTHING", "", "", "NO GYRO", "") + '\n'
            : String.Format($"{Formatting.FormatString}",
            "ONE", "",
            GlobalData.Node_One.ConnectionType,
            GlobalData.Node_One.Gyro!.LastRawValue,
            GlobalData.Node_One.Gyro!.DegreePerSecond().ToString("0.00"),
            GlobalData.Node_One.Gyro!.CalibrationStatus,
            GlobalData.Node_One.DataPerSecond) + '\n';

            table += (GlobalData.Node_Two.ConnectionType is ConnectionType.NOTHING)
            ? String.Format($"{Formatting.FormatString}", "TWO", "", "NOTHING", "", "", "NO GYRO", "")
            : String.Format($"{Formatting.FormatString}",
            "TWO", "",
            GlobalData.Node_Two.ConnectionType,
            GlobalData.Node_Two.Gyro!.LastRawValue,
            GlobalData.Node_Two.Gyro!.DegreePerSecond().ToString("0.00"),
            GlobalData.Node_Two.Gyro!.CalibrationStatus,
            GlobalData.Node_Two.DataPerSecond);
            Console.WriteLine(table);

            Console.WriteLine("-----------------------------------------------------------------------------------\n");
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
        Check_Calibration(GlobalData.Node_One!);
        Check_Calibration(GlobalData.Node_Two!);
    }

    private static void Check_Calibration(Node node)
    {
        if (node.ConnectionType is not ConnectionType.NOTHING && node.Gyro!.CalibrationStatus is CalibrationStatus.REQUESTED)
        {
            node.Gyro.Calibration(3);
        }
    }

    private static void Exit_Code()
    {
        ConsoleKeyInfo k = Console.ReadKey();
        if (k.KeyChar is not 'q')
        {
            k = Console.ReadKey();
        }
        wifiConnection!.CloseWiFi();
        Stop_Console();
    }

    private static void Stop_Console()
    {
        GlobalData.LastMessages.Add("PROGRAM STOPPED");
        timer!.Stop();
        Thread.Sleep(500);
        PrintConsole();
        Environment.Exit(0);
    }
}
