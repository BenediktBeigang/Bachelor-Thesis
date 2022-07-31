public class SavedOptions
{
    public SavedOptions()
    {
        GyroMode = GlobalData.GyroMode;
        NodesFlipped = GlobalData.NodesFlipped;
        WheelchairMode = GlobalData.WheelchairMode;
        WheelFlipped_One = GlobalData.Node_One.Gyro.RotationValueFlip;
        WheelFlipped_Two = GlobalData.Node_Two.Gyro.RotationValueFlip;
    }

    public void Load()
    {
        GlobalData.GyroMode = GyroMode;
        GlobalData.NodesFlipped = NodesFlipped;
        GlobalData.WheelchairMode = WheelchairMode;
        GlobalData.Node_One.Gyro.RotationValueFlip = WheelFlipped_One;
        GlobalData.Node_Two.Gyro.RotationValueFlip = WheelFlipped_Two;
    }

    public GyroMode GyroMode { get; set; }
    public bool NodesFlipped { get; set; }
    public WheelchairMode WheelchairMode { get; set; }
    public bool WheelFlipped_One { get; set; }
    public bool WheelFlipped_Two { get; set; }
}