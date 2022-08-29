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

    public static Sample newSample(GyroSnapshot gyroOne, GyroSnapshot gyroTwo, MovementState movementState)
    {
        return new Sample()
        {
            NodeOne_RawValue_Clean = gyroOne.RawValue_Clean,
            NodeTwo_RawValue_Clean = gyroTwo.RawValue_Clean,
            NodeOne_RawValue = gyroOne.RawValue,
            NodeTwo_RawValue = gyroTwo.RawValue,
            NodeOne_Value = gyroOne.AngularVelocity,
            NodeTwo_Value = gyroTwo.AngularVelocity,
            NodeOne_SmoothedValue = gyroOne.AngularVelocity_Smoothed,
            NodeTwo_SmoothedValue = gyroTwo.AngularVelocity_Smoothed,
            NodeOne_Acceleration = gyroOne.Acceleration,
            NodeTwo_Acceleration = gyroTwo.Acceleration,
            NodeOne_Datarate = Node.Node_One.DataPerSecond,
            NodeTwo_Datarate = Node.Node_Two.DataPerSecond,
            MovementState = movementState
        };
    }
}