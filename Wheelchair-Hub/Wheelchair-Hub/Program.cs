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
        Start_Loops();
        new Benchmark();
        Controller.Start();
        Connection._Connection = Connection.SetConnection(ConnectionType.WIFI);
        Load_Options();
        Exit_Code();
    }

    private static void Start_Loops()
    {
        new Loop(Loop.LOOP_DELAY_CONSOLE, Terminal.Print!);
        new Loop(Loop.LOOP_DELAY_CALIBRATION, Gyro.Check_Calibration!);
        new Loop(Loop.LOOP_DELAY_HEARTBEAT, Connection.Heartbeat!);
        new Loop(Loop.LOOP_DELAY_CONTROLLER, Controller.Refresh_Controller!);
    }

    private static void Load_Options()
    {
        bool optionsExisting = SavedOptions.Options.Read_JSON();
        if (optionsExisting)
            SavedOptions.Options.Load();
        else
            UserInput.Change_Mapping();
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