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
        Controller.Start();
        if (args.Length == 0) Connection._Connection = Connection.SetConnection(ConnectionType.WIFI);
        if (args.Length == 1 && args[0] == "wifi") Connection._Connection = Connection.SetConnection(ConnectionType.WIFI);
        if (args.Length == 2 && args[0] == "espnow") Connection._Connection = Connection.SetConnection(ConnectionType.ESP_NOW, args[1]);
        Exit_Code();
    }

    private static void Start_Loops()
    {
        new Loop(Loop.LOOP_DELAY_CONSOLE, Terminal.Print!);
        new Loop(Loop.LOOP_DELAY_CALIBRATION, Gyro.Check_Calibration!);
        new Loop(Loop.LOOP_DELAY_HEARTBEAT, Connection.Heartbeat!);
        new Loop(Loop.LOOP_DELAY_CONTROLLER, Controller.Refresh_Controller!);
        new Loop(Loop.LOOP_DELAY_MESSAGEBENCHMARK, Node.Update_Datarate_AllNodes!);
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
        if (Connection._Connection is not null)
        {
            UserInput.Input();
            SavedOptions.Options.Save();
            Connection._Connection!.Disconnect_AllNodes();
            Terminal.Add_Message("PROGRAM STOPPED");
        }
        Loop.Close_AllLoops();
        Console.Clear();
        Console.WriteLine(Terminal.InterfaceToString());
        Environment.Exit(0);
    }
}