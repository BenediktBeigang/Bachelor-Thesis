using System.Text.Json;

public class SavedOptions
{
    private const string FILEPATH = @"Files\SavedOptions.json";
    public static SavedOptions Options { get; set; } = new();
    private static string? Json = null;

    public bool Read_JSON()
    {
        try
        {
            string json = File.ReadAllText(FILEPATH);
            Options = JsonSerializer.Deserialize<SavedOptions>(json) ?? Options;
            return true;
        }
        catch (System.IO.FileNotFoundException)
        {
            return false;
        }
    }

    public void Load()
    {
        try
        {
            if (Options is null) return;
            Gyro.Mode = Options.GyroMode;
            Node.NodesFlipped = Options.NodesFlipped;
            Node.Node_One.Gyro.RotationValueFlip = Options.WheelFlipped_One;
            Node.Node_Two.Gyro.RotationValueFlip = Options.WheelFlipped_Two;
            Mapping.Change_Mapping(Options.WheelchairMode, Options.Wheel_Radius, Options.Chair_Width, Options.Button_Pressing_Threshold);
        }
        catch (System.IO.FileNotFoundException)
        {
            Terminal.Add_Message("No saved options.");
        }

    }

    public void Save()
    {
        Update();
        string json = JsonSerializer.Serialize(Options);
        File.WriteAllText(FILEPATH, json);
    }

    private void Update()
    {
        Options.GyroMode = Gyro.Mode;
        Options.NodesFlipped = Node.NodesFlipped;
        Options.WheelFlipped_One = Node.Node_One.Gyro.RotationValueFlip;
        Options.WheelFlipped_Two = Node.Node_Two.Gyro.RotationValueFlip;
        Options.WheelchairMode = Mapping.Get_Mode();
        Options.Wheel_Radius = Mapping.Get_Wheelchair().Wheel_Radius;
        Options.Chair_Width = Mapping.Get_Wheelchair().Chair_Width;
        Options.Button_Pressing_Threshold = Mapping.Get_ButtonPressingThreshold();
    }

    public GyroMode GyroMode { get; set; }
    public bool NodesFlipped { get; set; }
    public bool WheelFlipped_One { get; set; }
    public bool WheelFlipped_Two { get; set; }
    public MappingMode WheelchairMode { get; set; }
    public double Wheel_Radius { get; set; }
    public double Chair_Width { get; set; }
    public double Button_Pressing_Threshold { get; set; }
}