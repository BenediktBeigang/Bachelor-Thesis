public class Sample
{
    public double NodeOne_Value { get; set; }
    public double NodeTwo_Value { get; set; }
    public double NodeOne_Datarate { get; set; }
    public double NodeTwo_Datarate { get; set; }
    public double NodeOne_Acceleration { get; set; }
    public double NodeTwo_Acceleration { get; set; }
    public MovementState MovementState { get; set; }
    // public DateTime Timestamp { get; set; }
}

public enum MovementState
{
    StandingStill,
    ViewAxis_Motion,
    SingleWheel_Turn,
    DualWheel_Turn,
    Tilt
}