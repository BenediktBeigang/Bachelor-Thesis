public abstract class Mapping
{
    public static Mapping _Mapping { get; set; } = new GUI(30, 55.5, 200);
    public readonly double Button_Pressing_Threshold;
    public readonly MappingMode Mode;
    public readonly Wheelchair Wheelchair;

    public Mapping(MappingMode mode, double threshold, double wheelRadius, double chairWidth)
    {
        Mode = mode;
        Button_Pressing_Threshold = threshold;
        Wheelchair = new Wheelchair(wheelRadius, chairWidth);
    }

    public static void Change_Mapping(MappingMode mode, double wheelRadius, double chairWidth, double buttonPressingThreshold)
    {
        switch (mode)
        {
            case MappingMode.Wheelchair_Realistic: _Mapping = new RealisticWheelchair(wheelRadius, chairWidth, buttonPressingThreshold); break;
            case MappingMode.Wheelchair_Simple: _Mapping = new SimpleWheelchair(wheelRadius, chairWidth, buttonPressingThreshold); break;
            case MappingMode.GUI: _Mapping = new GUI(wheelRadius, chairWidth, buttonPressingThreshold); break;
        }
    }

    public abstract ControllerInput Values_Next(Rotations rotations);

    #region Checks
    protected bool Is_LeftRotation(double rotationLeft, double rotationRight)
    {
        return (rotationLeft < rotationRight);
    }

    protected bool Are_BothRotationsForeward(double rotationLeft, double rotationRight)
    {
        return (rotationLeft + rotationRight) >= 0;
    }

    protected bool Is_RotationAgainstEachOther(double rotationLeft, double rotationRight)
    {
        return (rotationLeft > 0 ^ rotationRight > 0);
    }

    protected static bool Is_OneRotationUnderThreshold(double value_One, double value_Two)
    {
        bool b1 = Math.Abs(value_One) < Get_ButtonPressingThreshold();
        bool b2 = Math.Abs(value_Two) < Get_ButtonPressingThreshold();
        return b1 ^ b2;
    }

    protected static bool Are_BothRotationsUnderThreshold(double value_One, double value_Two)
    {
        bool b1 = Math.Abs(value_One) < Get_ButtonPressingThreshold();
        bool b2 = Math.Abs(value_Two) < Get_ButtonPressingThreshold();
        return b1 && b2;
    }

    protected bool Is_RotationForward(double value)
    {
        return value > 0;
    }
    #endregion

    #region Wheel-Calculations 
    /// <summary>
    /// omega: ratio of wheelDistance covered to turning circle
    /// </summary>
    /// <param name="wheelDistance">Distance coverd by a wheel covered</param>
    /// <param name="isForwardRotation"></param>
    /// <returns></returns>
    protected (double, double) SingleWheel(double wheelDistance, bool isForwardRotation)
    {
        double omega = wheelDistance / Wheelchair.OuterTurningCircle;
        double speed = omega * Wheelchair.InnerTurningCircle;
        speed *= isForwardRotation ? 1 : -1;
        double rotation = omega * 360;
        return (speed, rotation);
    }

    protected (double distance, double rotation) DualWheel(double wheelDistance, bool isRotationAgainstEachOther)
    {
        if (isRotationAgainstEachOther)
            return DualWheel_Turn(wheelDistance);
        else
            return DualWheel_Move(wheelDistance);
    }

    protected (double, double) DualWheel_Turn(double wheelDistance)
    {
        return (0, RatioToDegree(wheelDistance, Wheelchair.InnerTurningCircle));
    }

    protected (double, double) DualWheel_Move(double wheelDistance)
    {
        return (wheelDistance, 0);
    }

    protected ControllerInput What_ButtonPressed(double left, double right)
    {
        bool leftPositive = false;
        bool leftNegative = false;
        bool rightPositive = false;
        bool rightNegative = false;
        if (Math.Abs(left) > Button_Pressing_Threshold)
        {
            if (Is_RotationForward(left))
                leftPositive = true;
            else
                leftNegative = true;
        }
        if (Math.Abs(right) > Button_Pressing_Threshold)
        {
            if (Is_RotationForward(right))
                rightPositive = true;
            else
                rightNegative = true;
        }
        return new ControllerInput(0, 0, leftPositive, leftNegative, rightPositive, rightNegative);
    }
    #endregion

    #region Conversion
    /// <summary>
    /// The ratio of the covered distance, to his corresponding full circle, is calculated to a rotation in degree. 
    /// </summary>
    protected double RatioToDegree(double length, double circumference)
    {
        return (length / circumference) * 360;
    }

    protected ControllerInput AngularVelocityToControllerInput(double v1, double v2, bool b1, bool b2, bool b3, bool b4)
    {
        return new ControllerInput((short)(v1 * Gyro.StepsPerDegree), (short)(v2 * Gyro.StepsPerDegree), b1, b2, b3, b4);
    }
    #endregion

    protected MovementState Analyse_Movement(double value_One, double value_Two)
    {
        if (Are_BothRotationsUnderThreshold(value_One, value_Two)) return MovementState.StandingStill;
        if (Is_OneRotationUnderThreshold(value_One, value_Two)) return MovementState.SingleWheel_Turn;
        if (Is_RotationAgainstEachOther(value_One, value_Two)) return MovementState.DualWheel_Turn;
        return MovementState.ViewAxis_Motion;
    }

    protected double MaxAbsolutRotation(double rotationOne, double rotationTwo)
    {
        return Math.Max(Math.Abs(rotationOne), Math.Abs(rotationTwo));
    }

    #region Getter
    public static MappingMode Get_Mode()
    {
        return Mapping._Mapping.Mode;
    }
    public static double Get_ButtonPressingThreshold()
    {
        return Mapping._Mapping.Button_Pressing_Threshold;
    }
    public static Wheelchair Get_Wheelchair()
    {
        return Mapping._Mapping.Wheelchair;
    }
    #endregion
}