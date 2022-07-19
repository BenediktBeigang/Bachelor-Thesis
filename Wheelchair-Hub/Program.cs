using System.Timers;

public static class Program
{
    private static WiFi? wifiConnection;
    private static System.Timers.Timer? timer;
    private const int TIME_BETWEEN_CONSOLE_CALLS = 16;
    private const GyroMode GYRO_MODE = GyroMode.Gyro_2000;

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
            Console.WriteLine("---------------------------------------------------------------");

            string table = "";
            table += String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,15}|\n", "Node", "", "Connected", "Raw Values", "DegreesPerSecond", "Gyro Calibrated");
            table += String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,15}|\n", "-----", "", "----------", "----------", "----------------", "---------------");
            table +=
            (GlobalData.Node_One is null)
            ? String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,15}|\n", "ONE", "", "False", "", "", "False")
            : String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,15}|\n", "ONE", "", GlobalData.Node_One.ConnectedWithWebsocket, GlobalData.Node_One.Gyro.RawValue, GlobalData.Node_One.Gyro.DegreePerSecond().ToString("0.00"), GlobalData.Node_One.Gyro.Calibrated);
            table +=
            (GlobalData.Node_Two is null)
            ? String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,15}|", "TWO", "", "False", "", "", "False")
            : String.Format("|{0,5}|{1,0}|{2,10}|{3,10}|{4,16}|{5,15}|", "TWO", "", GlobalData.Node_Two.ConnectedWithWebsocket, GlobalData.Node_Two.Gyro.RawValue, GlobalData.Node_Two.Gyro.DegreePerSecond().ToString("0.00"), GlobalData.Node_Two.Gyro.Calibrated);
            Console.WriteLine(table);

            Console.WriteLine("---------------------------------------------------------------\n");
            Console.WriteLine($"Last Messages: ");
            Console.WriteLine(LastMessagesString(10));
            Console.WriteLine($"\nPress 'q' to quit.");
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
        wifiConnection!.IsListening = (GlobalData.Node_One is not null && GlobalData.Node_One.ConnectedWithWebsocket is false);
        CheckCalibration(GlobalData.Node_One);
        CheckCalibration(GlobalData.Node_Two);
    }

    private static void CheckCalibration(Node? node)
    {
        if (node is not null && node.Gyro.Calibrated is false)
        {
            node.Gyro.Calibration(2);
            node.Gyro.Calibrated = true;
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

    private static void StopConsole()
    {
        timer!.Stop();
        PrintConsole();
        Thread.Sleep(200);
        Environment.Exit(0);
    }
}
