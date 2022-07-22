using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;

public static class Program
{
    #region Fields
    private static WiFi? wifiConnection;
    private static Formatting? Formatting;
    private const GyroMode GYRO_MODE = GyroMode.GYRO_2000;

    private static System.Timers.Timer? ConsoleTimer;
    private static System.Timers.Timer? ProgrammTimer;
    private static System.Timers.Timer? HeartbeatTimer;
    private const int TIME_BETWEEN_CONSOLE_CALLS = 100;
    private const int TIME_BETWEEN_PROGRAM_CALLS = 1000;
    private const int TIME_BETWEEN_HEARTBEAT_CALLS = 1000;
    #endregion

    public static void Main(string[] args)
    {
        new Benchmark();
        wifiConnection = new WiFi(); // "ws://ip:port/"
        wifiConnection.ConnectToHost();

        Loop();
        Exit_Code();
    }

    #region Timer
    private static void Loop()
    {
        Set_Timer(ref ConsoleTimer!, TIME_BETWEEN_CONSOLE_CALLS, "PrintConsole");
        Set_Timer(ref ProgrammTimer!, TIME_BETWEEN_PROGRAM_CALLS, "ProgramStep");
        Set_Timer(ref HeartbeatTimer!, TIME_BETWEEN_HEARTBEAT_CALLS, "Heartbeat");
    }

    private static void Set_Timer(ref System.Timers.Timer timer, int timeBetween, string methodName)
    {
        timer = new System.Timers.Timer(timeBetween);
        switch (methodName)
        {
            case "PrintConsole":
                timer.Elapsed += PrintConsole!;
                break;
            case "ProgramStep":
                timer.Elapsed += ProgramStep!;
                break;
            case "Heartbeat":
                timer.Elapsed += Heartbeat!;
                break;
        }
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private static void StopTimers()
    {
        ConsoleTimer!.Stop();
        ProgrammTimer!.Stop();
        HeartbeatTimer!.Stop();
    }
    #endregion

    #region Console
    private static void PrintConsole(object sender, ElapsedEventArgs e)
    {
        if (wifiConnection is not null)
        {
            Terminal.Print();
        }
    }

    private static void Stop_Console()
    {
        GlobalData.LastMessages.Add("PROGRAM STOPPED");
        Thread.Sleep(500);
        File.WriteAllTextAsync("LastConsolePrint.txt", Terminal.Print());
        Environment.Exit(0);
    }
    #endregion

    #region Programm
    private static void ProgramStep(object sender, ElapsedEventArgs e)
    {
        wifiConnection!.Listening = (GlobalData.Node_One.ConnectionType is ConnectionType.NOTHING || GlobalData.Node_Two.ConnectionType is ConnectionType.NOTHING);
        Check_Calibration(GlobalData.Node_One!);
        Check_Calibration(GlobalData.Node_Two!);
    }

    private static void Exit_Code()
    {
        ConsoleKeyInfo k = Console.ReadKey();
        if (k.KeyChar is not 'q')
        {
            k = Console.ReadKey();
        }
        wifiConnection!.Disconnect_AllNodes();
        Stop_Console();
    }
    #endregion

    #region Node
    /// <summary>
    /// Calls the Heartbeatfunction in Connection to verify that the connection is still alive.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void Heartbeat(object sender, ElapsedEventArgs e)
    {
        wifiConnection!.Heartbeat();
    }

    /// <summary>
    /// Checks if Calibration is requested and starts the Calibration-Process if true.
    /// </summary>
    /// <param name="node"></param>
    private static void Check_Calibration(Node node)
    {
        if (node.ConnectionType is not ConnectionType.NOTHING && node.Gyro!.CalibrationStatus is CalibrationStatus.REQUESTED)
        {
            node.Gyro.Calibration(3);
        }
    }
    #endregion
}