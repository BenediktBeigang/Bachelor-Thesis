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
        wifiConnection!.IsListening = (GlobalData.LeftNodeConnected is false);

        CalcValuesToDegree();
    }

    private static void PrintConsole()
    {
        Console.Clear();

        if (wifiConnection is not null)
        {
            string leftNodeConnected = (GlobalData.LeftNodeConnected) ? "true" : "false";
            string rightNodeConnected = (GlobalData.RightNodeConnected) ? "true" : "false";
            string leftRaw = GlobalData.LeftValue.ToString();
            string rightRaw = GlobalData.RightValue.ToString();
            string leftValue = GlobalData.CalculatedLeftValue.ToString("0.00");
            string rightValue = GlobalData.CalculatedRightValue.ToString("0.00");

            string consoleString = "";
            Console.WriteLine("----------------------------------");
            consoleString += String.Format("|{0,10}|{1,10}|{2,10}|\n", " ", "L", "R");
            consoleString += String.Format("|{0,10}|{1,10}|{2,10}|\n", "Connected", leftNodeConnected, rightNodeConnected);
            consoleString += String.Format("|{0,10}|{1,10}|{2,10}|\n", "Raw Values", leftRaw, rightRaw);
            consoleString += String.Format("|{0,10}|{1,10}|{2,10}|", "Values", leftValue, rightValue);
            Console.WriteLine(consoleString);
            Console.WriteLine("----------------------------------\n");
            Console.WriteLine($"Last Messages:");
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
        timer!.Close();
        Thread.Sleep(200);
        PrintConsole();
    }

    private static void CalcValuesToDegree()
    {
        GlobalData.CalculatedLeftValue = (GlobalData.LeftValue / EnumFunctions.StepsPerDegree(GYRO_MODE));
        GlobalData.CalculatedRightValue = (GlobalData.RightValue / EnumFunctions.StepsPerDegree(GYRO_MODE));
    }
}
