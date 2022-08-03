using System.Text.Json;

public class SavedOptions
{
    private const string FILEPATH = @"Files\SavedOptions.json";
    public static SavedOptions Options { get; set; } = new SavedOptions();

    public SavedOptions()
    {
        Update();
    }

    public void Load()
    {
        try
        {
            string json = File.ReadAllText(FILEPATH);
            SavedOptions? saved = JsonSerializer.Deserialize<SavedOptions>(json);

            if (saved is null) return;
            Gyro.Mode = saved.GyroMode;
            Node.NodesFlipped = saved.NodesFlipped;
            Node.Node_One.Gyro.RotationValueFlip = saved.WheelFlipped_One;
            Node.Node_Two.Gyro.RotationValueFlip = saved.WheelFlipped_Two;
            Mapping.Mode = saved.WheelchairMode;
        }
        catch (System.IO.FileNotFoundException)
        {
            Terminal.Add_Message("No saved options.");
        }

    }

    public void Save()
    {
        Update();
        SavedOptions saved = new SavedOptions();
        string json = JsonSerializer.Serialize(saved);
        File.WriteAllText(FILEPATH, json);
    }

    private void Update()
    {
        GyroMode = Gyro.Mode;
        NodesFlipped = Node.NodesFlipped;
        WheelFlipped_One = Node.Node_One.Gyro.RotationValueFlip;
        WheelFlipped_Two = Node.Node_Two.Gyro.RotationValueFlip;
        WheelchairMode = Mapping.Mode;
    }

    public GyroMode GyroMode { get; set; }
    public bool NodesFlipped { get; set; }
    public bool WheelFlipped_One { get; set; }
    public bool WheelFlipped_Two { get; set; }
    public MappingMode WheelchairMode { get; set; }
}