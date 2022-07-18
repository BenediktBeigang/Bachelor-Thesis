using System.Timers;

public static class Program
{
    private static WiFi? wifiConnection;
    private static System.Timers.Timer? timer;
    private const int TIME_BETWEEN_CONSOLE_CALLS = 500;

    public static void Main(string[] args)
    {
        wifiConnection = new WiFi(); // "ws://ip:port/"
        wifiConnection.ConnectToHost();
        ConsoleLoop();
        ExitCode();
    }

    public static void ConsoleLoop()
    {
        timer = new System.Timers.Timer(TIME_BETWEEN_CONSOLE_CALLS);
        timer.Elapsed += PrintConsole!;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    public static void PrintConsole(object sender, ElapsedEventArgs e)
    {
        // Console.Clear();

        if (wifiConnection is not null)
        {
            char leftNodeConnected = (GlobalData.LeftNodeConnected) ? 'x' : ' ';
            char rightNodeConnected = (GlobalData.RightNodeConnected) ? 'x' : ' ';
            string leftValue = CenterValue(GlobalData.LeftValue, 7);
            string rightValue = CenterValue(GlobalData.RightValue, 7);
            Console.WriteLine("-------Nodes-------");
            Console.WriteLine("   L   ||   R   ");
            Console.WriteLine($"   {leftNodeConnected}   ||   {rightNodeConnected}   ");
            Console.WriteLine($"{GlobalData.LeftValue}||{GlobalData.LeftValue}");
            Console.WriteLine("------------------");
            Console.WriteLine($"Last Message: {GlobalData.LastMessage}");
            Console.WriteLine($"Press 'q' to quit.");
        }
    }

    private static string CenterValue(int value, int maxLength)
    {
        string valueAsString = value.ToString();
        int valueLength = valueAsString.Length;

        int emptyCount = maxLength - valueLength;
        int padding = (int)emptyCount / 2;

        valueAsString.PadLeft(padding);
        valueAsString.PadRight(padding);

        while (valueAsString.Length < maxLength)
        {
            valueAsString.PadRight(1);
        }
        return valueAsString;
    }

    private static void ExitCode()
    {
        char k = Convert.ToChar(Console.Read());
        while (k is not 'q')
        {
            k = Convert.ToChar(Console.Read()); ;
        }
        wifiConnection!.CloseWiFi();
        timer!.Enabled = false;
    }
}
