using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Enter Name of File:");
            string? input = Console.ReadLine();
            // string? input = "mitAcc";
            if (input == "q") break;

            string path = @"Files\";
            string name = input!;
            string filetype = ".json";

            try
            {
                Console.WriteLine("Try to Parse File");

                string jsonString = File.ReadAllText(path + name + filetype);
                List<Sample> record = JsonSerializer.Deserialize<List<Sample>>(jsonString) ?? new List<Sample>();

                string movementState = "button";

                // new Plot_GyroSignalNodeOne(record);
                // new Plot_GyroSignalNodeTwo(record);
                Stats_Datarate(record);
                // new Plot_Gyro(record, movementState, name);
                // new Plot_Datarate();
                // new Plot_GyroWithAcceleration(record, movementState, name);
                // new Plot_NodeOne(record);
                // new Plot_mitAcc(record, movementState, name);
                // new Plot_ohneAcc(record, movementState, name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    private static void Stats_Datarate(List<Sample> record)
    {
        List<double> datarates = record.Select(r => r.NodeOne_Datarate).ToList();
        datarates.AddRange(record.Select(r => r.NodeTwo_Datarate));
        double average = Math.Round(datarates.Average(), 2);
        double minimum = datarates.Min();
        double maximum = datarates.Max();
        Console.WriteLine($"Pakets per second\nAverage: {average}\nMinimum: {minimum}\nMaximum: {maximum}");
    }

    public static System.Drawing.Color GetMovementStateColor_Default(MovementState state)
    {
        switch (state)
        {
            case MovementState.StandingStill: return System.Drawing.Color.Transparent;
            case MovementState.ViewAxis_Motion: return Colors.Color_ViewAxis_Motion();//System.Drawing.Color.Purple;
            case MovementState.DualWheel_Turn: return System.Drawing.Color.Chocolate;
            case MovementState.SingleWheel_Turn: return System.Drawing.Color.Cyan;
            case MovementState.Tilt: return System.Drawing.Color.PeachPuff;
        }
        return System.Drawing.Color.Transparent;
    }

    public static System.Drawing.Color MovementStateColor_Tilt(MovementState state)
    {
        switch (state)
        {
            case MovementState.StandingStill: return System.Drawing.Color.LightGray;
            case MovementState.Tilt: return System.Drawing.Color.Red;
            case MovementState.ViewAxis_Motion: return System.Drawing.Color.LightGreen;
            case MovementState.DualWheel_Turn: return System.Drawing.Color.Transparent;
            case MovementState.SingleWheel_Turn: return System.Drawing.Color.Transparent;
        }
        return System.Drawing.Color.Transparent;
    }

    public static System.Drawing.Color MovementStateColor_Button(MovementState state)
    {
        switch (state)
        {
            case MovementState.StandingStill: return System.Drawing.Color.Transparent;
            case MovementState.Tilt: return System.Drawing.Color.Transparent;
            case MovementState.ViewAxis_Motion: return System.Drawing.Color.LightGreen;
            case MovementState.DualWheel_Turn: return System.Drawing.Color.Transparent;
            case MovementState.SingleWheel_Turn: return System.Drawing.Color.Red;
        }
        return System.Drawing.Color.Transparent;
    }

    public static void DrawMovementStates(List<Sample> record, ref ScottPlot.Plot plt, string drawingMode)
    {
        int span_start = 0;
        int span_end = 0;
        double scale = (double)1 / (double)60;
        for (int i = 1; i < record.Count; i++)
        {
            if (record[i].MovementState != record[i - 1].MovementState)
            {
                span_end = i - 1;
                plt.AddHorizontalSpan(span_start * scale, span_end * scale, GetColor(drawingMode, record[i - 1].MovementState));
                span_start = i;
            }
            else if (i + 1 == record.Count)
            {
                span_end = i;
                plt.AddHorizontalSpan(span_start * scale, span_end * scale, GetColor(drawingMode, record[i - 1].MovementState));
            }
        }
    }

    private static System.Drawing.Color GetColor(string drawindMode, MovementState state)
    {
        switch (drawindMode)
        {
            case "tilt": return MovementStateColor_Tilt(state);
            case "button": return MovementStateColor_Button(state);
            default: return GetMovementStateColor_Default(state);
        }
    }
}