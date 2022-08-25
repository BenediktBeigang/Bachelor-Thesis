public class Wheelchair
{
    public readonly double Wheel_Radius;
    public readonly double Chair_Width;
    public readonly double Wheel_Diameter;
    public readonly double Wheel_Circumference;
    public readonly double Chair_HalfWidth;
    public readonly double OuterTurningCircle;
    public readonly double InnerTurningCircle;
    public readonly double LengthOfOneDegree;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="wheelRadius">In meter</param>
    /// <param name="chairWidth">In meter</param>
    public Wheelchair(double wheelRadius, double chairWidth)
    {
        Wheel_Radius = wheelRadius;
        Wheel_Diameter = wheelRadius * 2;
        Wheel_Circumference = (double)Math.PI * Wheel_Diameter;
        Chair_Width = chairWidth;
        Chair_HalfWidth = chairWidth / 2;
        OuterTurningCircle = (double)Math.PI * chairWidth * 2;
        InnerTurningCircle = (double)Math.PI * chairWidth;
        LengthOfOneDegree = Wheel_Circumference / 360;
    }

    #region Checks
    public static bool Is_LeftRotation(Rotations rotations)
    {
        return (rotations.RawLeft < rotations.RawRight);
    }

    public static bool Is_RotationSumForeward(Rotations rotations)
    {
        return (rotations.RawLeft + rotations.RawRight) >= 0;
    }

    public static bool Is_RotationAgainstEachOther(Rotations rotations)
    {
        return ((rotations.RawLeft > 0) ^ (rotations.RawRight > 0));
    }

    public static bool Is_ExactlyOneRotationUnderThreshold(Rotations rotations, int threshold)
    {
        bool b1 = Math.Abs(rotations.AngularVelocityLeft) < threshold;
        bool b2 = Math.Abs(rotations.AngularVelocityRight) < threshold;
        return b1 ^ b2;
    }

    public static bool Are_BothRotationsUnderThreshold(Rotations rotations, int threshold)
    {
        bool b1 = Math.Abs(rotations.AngularVelocityLeft) < threshold;
        bool b2 = Math.Abs(rotations.AngularVelocityRight) < threshold;
        return b1 && b2;
    }

    public static bool Is_RotationForward(double rotation)
    {
        return rotation > 0;
    }

    public static bool Is_RotationUnderThreshold(double rotation, int threshold)
    {
        return Math.Abs(rotation) < threshold;
    }

    public static bool Is_MovementStateChanging()
    {
        return false;
        (double, double) acceleration = Gyro.Acceleration_BothNodes();
        return acceleration.Item1 + acceleration.Item2 >= 15;
    }
    #endregion

    #region Conversions
    /// <summary>
    /// The ratio of the covered distance, to his corresponding full circle, is calculated to a rotation in degree. 
    /// </summary>
    public static double RatioToDegree(double length, double circumference)
    {
        return (length / circumference) * 360;
    }

    public static short AngularVelocityToControllerAxis(double value)
    {
        // const int CONTROLLER_MAX = 35767;
        // double StepsPerDegree = CONTROLLER_MAX / Mapping.Get_WheelMovement_Max();
        // return (short)(value * StepsPerDegree);
        int sign = (value < 0) ? -1 : 1;
        return (short)((value > Gyro.ModeAsInteger()) ? sign * Gyro.ModeAsInteger() : value * Gyro.StepsPerDegree);
    }

    public override string ToString()
    => "========================================\n"
    + $"Radius|Diameter: {Wheel_Radius} | {Wheel_Diameter}\n"
    + $"Circumference: {Wheel_Circumference}\n"
    + $"ChairWidth(Half): {Chair_Width} | {Chair_HalfWidth}\n"
    + $"(Inner)TurningCircle: {InnerTurningCircle} | {OuterTurningCircle}"
    + "\n========================================";
    #endregion
}