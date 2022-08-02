using System.Timers;

public class Record
{
    private static bool Recording { get; set; }
    private static List<(short one, short two)> RecordedValues = new();
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
        RecordedValues.Clear();
        Recording = true;
        Start_Timer();
        GlobalData.Add_Message("Started recording.");
    }

    private static void Stop_Record()
    {
        Timer.Stop();
        Timer.Close();
        Recording = false;
        Save_Recording();
        GlobalData.Add_Message("Stopped recording.");
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
        short v1 = GlobalData.Node_One.Gyro.RawValue_Last();
        short v2 = GlobalData.Node_Two.Gyro.RawValue_Last();
        RecordedValues.Add((v1, v2));
    }

    private static void Save_Recording()
    {
        try
        {
            string fileString = "";
            for (int i = 0; i < RecordedValues.Count; i++)
            {
                short v1 = RecordedValues[i].one;
                short v2 = RecordedValues[i].two;
                fileString += $"{v1};{v2}\n";
            }

            string path = @"Files\";
            string time = DateTime.Now.ToLocalTime().ToLongTimeString();
            time = $"{time[0] + time[1]}_{time[3] + time[4]}_{time[6] + time[7]}";
            string filetype = ".csv";
            File.WriteAllText(path + time + filetype, fileString);
            new Plot(RecordedValues);
        }
        catch (Exception e)
        {
            GlobalData.other = e.ToString();
        }
    }
}