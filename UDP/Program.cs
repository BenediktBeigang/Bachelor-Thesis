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

    private static void ProgramStep()
    {
        // TODO: muss für beide räder angepasst werden.
        wifiConnection!.IsListening = (GlobalData.NodeConnected_One is false);

        CalcValuesToDegree();
    }

    private static void PrintConsole()
    {
        if (wifiConnection is not null)
        {
            Console.Clear();
            string nodeConnected_One = (GlobalData.NodeConnected_One) ? "true" : "false";
            string nodeConnected_Two = (GlobalData.NodeConnected_Two) ? "true" : "false";
            string raw_One = GlobalData.RawValue_One.ToString();
            string raw_Two = GlobalData.RawValue_Two.ToString();
            string value_One = GlobalData.CalculatedValue_One.ToString("0.00");
            string value_Two = GlobalData.CalculatedValue_Two.ToString("0.00");

            string consoleString = "";
            Console.WriteLine("--------------------------------------------");
            consoleString += String.Format("|{0,20}|{1,10}|{2,10}|\n", "Node", "ONE", "TWO");
            consoleString += String.Format("|{0,20}|{1,10}|{2,10}|\n", "--------------------", "----------", "----------");
            consoleString += String.Format("|{0,20}|{1,10}|{2,10}|\n", "Connected", nodeConnected_One, nodeConnected_Two);
            consoleString += String.Format("|{0,20}|{1,10}|{2,10}|\n", "Raw Values", raw_One, raw_Two);
            consoleString += String.Format("|{0,20}|{1,10}|{2,10}|", "Calc Values", value_One, value_Two);
            Console.WriteLine(consoleString);
            Console.WriteLine("--------------------------------------------\n");
            Console.WriteLine($"Last Messages: ");
            Console.WriteLine(LastMessagesString(5));
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

    private static void CalcValuesToDegree()
    {
        GlobalData.CalculatedValue_One = (GlobalData.RawValue_One / Gyro.StepsPerDegree(GYRO_MODE));
        GlobalData.CalculatedValue_Two = (GlobalData.RawValue_Two / Gyro.StepsPerDegree(GYRO_MODE));
    }
}
