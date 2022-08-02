using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Timers;

public static class Program
{
    #region Fields
    private static Connection? connection;
    private static ValueTransformation? ValueTransformation;
    private static Controller? controllerInput;

    private static System.Timers.Timer? ConsoleTimer;
    private static System.Timers.Timer? CalibrationTimer;
    private static System.Timers.Timer? HeartbeatTimer;
    private static System.Timers.Timer? ControllerTimer;
    private const int TIME_BETWEEN_CONSOLE_CALLS = 100;
    private const int TIME_BETWEEN_CALIBRATION_CALLS = 1000;
    private const int TIME_BETWEEN_HEARTBEAT_CALLS = 1000;
    private const int TIME_BETWEEN_CONTROLLER_CALLS = 16;

    private const string COM = "COM4";
    private const int BAUDRATE = 115200;
    private const WheelchairMode WHEELCHAIR_MODE = WheelchairMode.Wheelchair_Realistic;
    #endregion

    public static void Main(string[] args)
    {
        new Benchmark();
        GlobalData.GyroMode = GyroMode.GYRO_1000;
        GlobalData.WheelchairMode = WheelchairMode.Wheelchair_Realistic;
        ValueTransformation = new RealisticWheelchair(30, 55.5);
        controllerInput = new Controller();
        LoadOptions();

        // connection = SetConnection(ConnectionType.ESP_NOW);
        // connection!.Connect_ToHost();
        connection = SetConnection(ConnectionType.WIFI);
        connection!.Connect_ToHost();

        Loop();
        Exit_Code();
    }

    private static Connection? SetConnection(ConnectionType connection)
    {
        switch (connection)
        {
            case ConnectionType.WIFI: return new WiFi();
            case ConnectionType.ESP_NOW: return new ESP_Now(COM, BAUDRATE);
            case ConnectionType.BLUETOOTH: return null;
            default: return null;
        }
    }

    #region Timer
    private static void Loop()
    {
        Set_Timer(ref ConsoleTimer!, TIME_BETWEEN_CONSOLE_CALLS, "PrintConsole");
        Set_Timer(ref CalibrationTimer!, TIME_BETWEEN_CALIBRATION_CALLS, "CalibrationCheck");
        Set_Timer(ref HeartbeatTimer!, TIME_BETWEEN_HEARTBEAT_CALLS, "Heartbeat");
        Set_Timer(ref ControllerTimer!, TIME_BETWEEN_CONTROLLER_CALLS, "ControllerRefresh");
    }

    private static void Set_Timer(ref System.Timers.Timer timer, int timeBetween, string methodName)
    {
        timer = new System.Timers.Timer(timeBetween);
        switch (methodName)
        {
            case "PrintConsole":
                timer.Elapsed += PrintConsole!;
                break;
            case "CalibrationCheck":
                timer.Elapsed += CalibrationCheck!;
                break;
            case "Heartbeat":
                timer.Elapsed += Heartbeat!;
                break;
            case "ControllerRefresh":
                timer.Elapsed += Refresh_Controller!;
                break;
        }
        timer.AutoReset = true;
        timer.Enabled = true;
    }

    private static void StopTimers()
    {
        ConsoleTimer!.Stop();
        CalibrationTimer!.Stop();
        HeartbeatTimer!.Stop();
        ControllerTimer!.Stop();
    }
    #endregion

    #region Console
    private static void PrintConsole(object sender, ElapsedEventArgs e)
    {
        Terminal.Print();
    }

    private static void Stop_Console()
    {
        GlobalData.Add_Message("PROGRAM STOPPED");
        Thread.Sleep(500);
        File.WriteAllTextAsync("LastConsolePrint.txt", Terminal.Print());
        Environment.Exit(0);
    }
    #endregion

    #region Program
    private static void Exit_Code()
    {
        UserInput.Input(connection!);
        SaveOptions();
        connection!.Disconnect_AllNodes();
        Stop_Console();
    }

    private static void SaveOptions()
    {
        SavedOptions saved = new SavedOptions();
        string json = JsonSerializer.Serialize(saved);
        File.WriteAllText(@"Files\SavedOptions.json", json);
    }

    private static void LoadOptions()
    {
        string json = File.ReadAllText(@"Files\SavedOptions.json");
        SavedOptions? saved = JsonSerializer.Deserialize<SavedOptions>(json);
        if (saved is not null) saved.Load();
    }
    #endregion

    #region Controller
    private static void Refresh_Controller(object sender, ElapsedEventArgs e)
    {
        (short RawLeft, short RawRight, double Left, double Right) rawValues = GlobalData.Rotations();
        (double, double) controllerValues = ValueTransformation!.TransformedValues_Next(rawValues.RawLeft, rawValues.RawRight, rawValues.Left, rawValues.Right);
        controllerInput!.ValuesToController(controllerValues);
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
        // wifiConnection!.Heartbeat();
        connection!.Heartbeat();
    }

    private static void CalibrationCheck(object sender, ElapsedEventArgs e)
    {
        Check_Calibration(GlobalData.Node_One!);
        Check_Calibration(GlobalData.Node_Two!);
    }

    /// <summary>
    /// Checks if Calibration is requested and starts the Calibration-Process if true.
    /// </summary>
    /// <param name="node"></param>
    private static void Check_Calibration(Node node)
    {
        if (node.Gyro.CalibrationStatus is CalibrationStatus.REQUESTED)
        {
            node.Gyro.Calibration(3);
        }
    }

    private static void Request_Calibration()
    {
        if (GlobalData.Node_One.ConnectionType is not ConnectionType.NOTHING) GlobalData.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
        if (GlobalData.Node_Two.ConnectionType is not ConnectionType.NOTHING) GlobalData.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
    }
    #endregion
}