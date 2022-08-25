using System.Text.Json;
using System.Timers;

public class Record
{
    private static bool Recording { get; set; }
    private static List<Sample> RecordedSamples = new();
    private const int TIME_BETWEEN_CALLS = 16;
    private static System.Timers.Timer Timer = new System.Timers.Timer(TIME_BETWEEN_CALLS);

    public static bool Is_Recording()
    {
        return Recording;
    }

    public static void Switch_Record()
    {
        Recording = !Recording;
        if (Recording)
            Start_Record();
        else
            Stop_Record();
    }

    private static void Start_Record()
    {
        RecordedSamples.Clear();
        Recording = true;
        Start_Timer();
        Terminal.Add_Message("Started recording.");
    }

    private static void Stop_Record()
    {
        Timer.Stop();
        Timer.Close();
        Recording = false;
        Save_Recording();
        Terminal.Add_Message("Stopped recording.");
    }

    private static void Start_Timer()
    {
        Timer = new System.Timers.Timer(TIME_BETWEEN_CALLS);
        Timer.Elapsed += TakeSample!;
        Timer.AutoReset = true;
        Timer.Enabled = true;
    }

    private static void TakeSample(object sender, ElapsedEventArgs e)
    {
        RecordedSamples.Add(new Sample
        {
            NodeOne_Value = Node.Node_One.Gyro.RawValue_Last() / Gyro.StepsPerDegree,
            NodeTwo_Value = Node.Node_Two.Gyro.RawValue_Last() / Gyro.StepsPerDegree,
            NodeOne_Datarate = Node.Node_One.DataPerSecond,
            NodeTwo_Datarate = Node.Node_Two.DataPerSecond,
            NodeOne_Acceleration = Node.Node_One.Gyro.Acceleration(),
            NodeTwo_Acceleration = Node.Node_Two.Gyro.Acceleration(),
            MovementState = Mapping._Mapping.Get_MovementState(Node.Rotations())
            // , Timestamp = DateTime.Now
        });
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