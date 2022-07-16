using System.Timers;

public class Program
{
    private static WiFi? wifiConnection;
    private static System.Timers.Timer timer;

    public static void Main(string[] args)
    {
        wifiConnection = new WiFi(); // "ws://ip:port/"
        wifiConnection.ConnectToHost();
        ConsoleLoop();
        ExitCode();
    }

    public static void ConsoleLoop()
    {
        timer = new System.Timers.Timer(1000);
        timer.Elapsed += PrintConsole!;
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    public static void PrintConsole(object sender, ElapsedEventArgs e)
    {
        Console.Clear();

        if (wifiConnection is not null)
        {
            char leftNode = (wifiConnection!.LeftNodeConnected) ? 'x' : ' ';
            char rightNode = (wifiConnection!.RightNodeConnected) ? 'x' : ' ';
            Console.WriteLine("---Connected Nodes---");
            Console.WriteLine("Left | Right ");
            Console.WriteLine($"  {leftNode}  |   {rightNode}");
            Console.WriteLine();
            Console.WriteLine($"Host>> Message: {wifiConnection.LastMessage}");
            Console.WriteLine($"Host>> Wheel: {wifiConnection.GyroValue}");
        }
    }

    private static void ExitCode()
    {
        ConsoleKeyInfo k = Console.ReadKey();
        while (k.KeyChar is not 'q')
        {
            k = Console.ReadKey();
        }
        wifiConnection!.IsListening = false;
        timer.Enabled = false;
    }
}
