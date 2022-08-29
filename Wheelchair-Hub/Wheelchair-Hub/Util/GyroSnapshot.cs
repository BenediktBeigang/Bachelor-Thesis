public struct GyroSnapshot
{
    public short RawValue_Clean { get; set; }
    public short RawValue { get; set; }
    public double AngularVelocity { get; set; }
    public double AngularVelocity_Smoothed { get; set; }
    public double Acceleration { get; set; }

    public GyroSnapshot(short rawValue_Clean, short rawValue, double angularVelocity, double angularVelocity_Smoothed, double acceleration)
    {
        RawValue_Clean = rawValue_Clean;
        RawValue = rawValue;
        AngularVelocity = angularVelocity;
        AngularVelocity_Smoothed = angularVelocity_Smoothed;
        Acceleration = acceleration;
    }

    public void Mute()
    {
        RawValue_Clean = 0;
        RawValue = 0;
        AngularVelocity = 0;
        AngularVelocity_Smoothed = 0;
        Acceleration = 0;
    }
}
