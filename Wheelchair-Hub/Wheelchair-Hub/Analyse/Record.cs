using System.Text.Json;
using System.Timers;

public class Record
{
    public static bool Is_Recording { get; set; }

    public static List<Sample> RecordedSamples { get; set; } = new();
    private const int TIME_BETWEEN_CALLS = 16;
    private static System.Timers.Timer Timer = new System.Timers.Timer(TIME_BETWEEN_CALLS);

    public static void Switch_Record()
    {
        Is_Recording = !Is_Recording;
        if (Is_Recording)
            Start_Record();
        else
            Stop_Record();
    }

    private static void Start_Record()
    {
        RecordedSamples.Clear();
        Is_Recording = true;
        Terminal.Add_Message("Started recording.");
    }

    private static void Stop_Record()
    {
        Is_Recording = false;
        Save_Recording();
        Terminal.Add_Message("Stopped recording.");
    }

    public static void TakeSample(GyroSnapshot nodeOne, GyroSnapshot nodeTwo, MovementState movementState)
    {
        RecordedSamples.Add(Sample.newSample(nodeOne, nodeTwo, movementState));
    }

    private static void Save_Recording()
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(RecordedSamples);
            string path = @"Files\";
            string time = DateTime.Now.ToLocalTime().ToLongTimeString();
            time = $"{time[0] + time[1]}_{time[3] + time[4]}_{time[6] + time[7]}";
            string filetype = ".json";
            File.WriteAllText(path + time + filetype, jsonString);
            new Plot(RecordedSamples);
        }
        catch (Exception e)
        {
            Terminal.Other = e.ToString();
        }
    }
}