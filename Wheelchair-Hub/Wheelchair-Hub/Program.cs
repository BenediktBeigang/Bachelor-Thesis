using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Timers;

public static class Program
{
    public static void Main(string[] args)
    {
        new Benchmark();
        Mapping._Mapping = new RealisticWheelchair(30, 55.5);
        Controller.Start();
        SavedOptions.Options.Load();
        Connection._Connection = Connection.SetConnection(ConnectionType.WIFI);

        Start_Loops();
        Exit_Code();
    }

    private static void Start_Loops()
    {
        new Loop(Loop.LOOP_DELAY_CONSOLE, Terminal.Print!);
        new Loop(Loop.LOOP_DELAY_CALIBRATION, Gyro.Check_Calibration!);
        new Loop(Loop.LOOP_DELAY_HEARTBEAT, Connection.Heartbeat!);
        new Loop(Loop.LOOP_DELAY_CONTROLLER, Controller.Refresh_Controller!);
    }

    private static void Exit_Code()
    {
        UserInput.Input();
        SavedOptions.Options.Save();
        Connection._Connection!.Disconnect_AllNodes();
        Terminal.Add_Message("PROGRAM STOPPED");
        Loop.Close_AllLoops();
        Console.Clear();
        Console.WriteLine(Terminal.InterfaceToString());
        Environment.Exit(0);
    }
}