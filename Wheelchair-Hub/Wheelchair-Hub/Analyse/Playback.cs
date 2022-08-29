using System.Text.Json;

public class Playback
{
    public static bool Is_PlaybackRunning { get; set; }
    private static short[] Buffer_One = new short[0];
    private static short[] Buffer_Two = new short[0];
    private static int Buffer_Length = 0;
    private static int Buffer_Pointer = 1;
    private static readonly string FOLDER = @"Playback/";

    public static void Update_Gyro()
    {
        if (Buffer_One.Length == 0 || Buffer_Two.Length == 0 || Buffer_Pointer == Buffer_Length)
        {
            Stop_Playback();
            return;
        }
        Node.Node_One.Gyro.Push_RawValue(Buffer_One[Buffer_Pointer], ValueSource.PLAYBACK);
        Node.Node_Two.Gyro.Push_RawValue(Buffer_Two[Buffer_Pointer], ValueSource.PLAYBACK);
        Buffer_Pointer++;
    }

    private static void Stop_Playback()
    {
        Is_PlaybackRunning = false;
        Buffer_One = new short[0];
        Buffer_Two = new short[0];
        Buffer_Length = 0;
        Buffer_Pointer = 1;
        Terminal.Add_Message("Playback stopped!");
    }

    public static void Start_Playback()
    {
        try
        {
            string? filepath = Directory.GetFiles(FOLDER, "*.json").FirstOrDefault();
            string jsonString = File.ReadAllText(filepath!);
            List<Sample> record = JsonSerializer.Deserialize<List<Sample>>(jsonString) ?? new List<Sample>();

            (short[] bufferOne, short[] bufferTwo) buffer = Extract_Buffer(record);
            if (buffer.bufferOne.Length != buffer.bufferTwo.Length) return;
            Buffer_Length = buffer.bufferOne.Length;
            Buffer_One = buffer.bufferOne;
            Buffer_Two = buffer.bufferTwo;

            Is_PlaybackRunning = true;
            Terminal.Add_Message("Playback started!");
        }
        catch (Exception e)
        {
            Is_PlaybackRunning = false;
            Terminal.Add_Message("Playback could NOT be started, because: " + e.Message);
        }
    }

    private static (short[] bufferOne, short[] bufferTwo) Extract_Buffer(List<Sample> record)
    {
        short[] one = new short[record.Count];
        short[] two = new short[record.Count];
        for (int i = 0; i < record.Count; i++)
        {
            one[i] = (short)record[i].NodeOne_RawValue_Clean;
            two[i] = (short)record[i].NodeTwo_RawValue_Clean;
        }
        return (one, two);
    }
}