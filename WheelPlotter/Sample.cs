public class Sample
{
    public double NodeOne_RawValue_Clean { get; set; }
    public double NodeTwo_RawValue_Clean { get; set; }
    public double NodeOne_RawValue { get; set; }
    public double NodeTwo_RawValue { get; set; }
    public double NodeOne_Value { get; set; }
    public double NodeTwo_Value { get; set; }
    public double NodeOne_SmoothedValue { get; set; }
    public double NodeTwo_SmoothedValue { get; set; }
    public double NodeOne_Acceleration { get; set; }
    public double NodeTwo_Acceleration { get; set; }
    public double NodeOne_Datarate { get; set; }
    public double NodeTwo_Datarate { get; set; }
    public MovementState MovementState { get; set; }
}

public enum MovementState
{
    NoStateMovement,
    StandingStill,
    ViewAxis_Motion,
    SingleWheel_Turn,
    DualWheel_Turn,
    Tilt
}