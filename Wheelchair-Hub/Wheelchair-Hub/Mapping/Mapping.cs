public abstract class Mapping
{
    public static Mapping _Mapping { get; set; } = new GUI(30, 55.5, 200, 300);
    public readonly int WheelMovement_Threshold;
    public readonly int ButtonPressing_Threshold;
    public readonly MappingMode Mode;
    public readonly Wheelchair Wheelchair;
    private long LastMovement_Timestamp;

    public Mapping(MappingMode mode, int wheelMovement_Threshold, int buttonPressing_Threshold, double wheelRadius, double chairWidth)
    {
        Mode = mode;
        ButtonPressing_Threshold = buttonPressing_Threshold;
        WheelMovement_Threshold = wheelMovement_Threshold;
        Wheelchair = new Wheelchair(wheelRadius, chairWidth);
        LastMovement_Timestamp = 0;
    }

    #region Change_Mapping
    public void Change_Mapping(MappingMode mode)
    {
        switch (mode)
        {
            case MappingMode.Wheelchair_Realistic:
                _Mapping = new RealisticWheelchair(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, WheelMovement_Threshold, ButtonPressing_Threshold); break;
            case MappingMode.Wheelchair_Simple:
                _Mapping = new SimpleWheelchair(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, WheelMovement_Threshold, ButtonPressing_Threshold); break;
            case MappingMode.Wheelchair_WithButtons:
                _Mapping = new WheelchairWithButtons(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, WheelMovement_Threshold, ButtonPressing_Threshold); break;
            case MappingMode.GUI:
                _Mapping = new GUI(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, WheelMovement_Threshold, ButtonPressing_Threshold); break;
        }
    }

    public void Change_Mapping(MappingMode mode, double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold)
    {
        switch (mode)
        {
            case MappingMode.Wheelchair_Realistic: _Mapping = new RealisticWheelchair(wheelRadius, chairWidth, wheelMovement_Threshold, buttonPressingThreshold); break;
            case MappingMode.Wheelchair_Simple: _Mapping = new SimpleWheelchair(wheelRadius, chairWidth, wheelMovement_Threshold, buttonPressingThreshold); break;
            case MappingMode.Wheelchair_WithButtons: _Mapping = new WheelchairWithButtons(wheelRadius, chairWidth, wheelMovement_Threshold, buttonPressingThreshold); break;
            case MappingMode.GUI: _Mapping = new GUI(wheelRadius, chairWidth, wheelMovement_Threshold, buttonPressingThreshold); break;
        }
    }
    #endregion

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
        bool b1 = Math.Abs(value_One) < Get_WheelMovementThreshold();
        bool b2 = Math.Abs(value_Two) < Get_WheelMovementThreshold();
        return b1 ^ b2;
    }

    protected static bool Are_BothRotationsUnderThreshold(double value_One, double value_Two)
    {
        bool b1 = Math.Abs(value_One) < Get_WheelMovementThreshold();
        bool b2 = Math.Abs(value_Two) < Get_WheelMovementThreshold();
        return b1 && b2;
    }

    protected bool Is_RotationForward(double value)
    {
        return value > 0;
    }

    private bool Is_ButtonPressAllowed()
    {
        long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long timeBetween = now - LastMovement_Timestamp;
        return (timeBetween > ButtonPressing_Threshold);
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
        if (Is_ButtonPressAllowed() is false) return new ControllerInput();
        bool leftPositive = false;
        bool leftNegative = false;
        bool rightPositive = false;
        bool rightNegative = false;
        if (Math.Abs(left) > WheelMovement_Threshold)
        {
            if (Is_RotationForward(left))
                leftPositive = true;
            else
                leftNegative = true;
        }
        if (Math.Abs(right) > WheelMovement_Threshold)
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

    /// <summary>
    /// Returns the current state of movement, dependend by the rotation of the wheels. 
    /// If the wheelchair moves (for MappingType: Gui, WheelchairWithButtons), so both wheels are rotating, buttons can't be pressed.
    /// The LastMovement_Threshold is resetted, so when the state changes to a state where buttons can be pressed, the system must wait until the threshold-time is elapsed.
    /// </summary>
    /// <param name="value_One"></param>
    /// <param name="value_Two"></param>
    /// <returns></returns>
    protected MovementState Get_MovementState(double value_One, double value_Two)
    {
        // if (Are_BothRotationsUnderThreshold(value_One, value_Two))
        // {
        //     if (value_One is 0 && value_Two is 0) return MovementState.StandingStill;
        //     return MovementState.Tilt;
        // }
        if (Are_BothRotationsUnderThreshold(value_One, value_Two)) return MovementState.StandingStill;
        if (Is_OneRotationUnderThreshold(value_One, value_Two)) return MovementState.SingleWheel_Turn;

        Reset_LastMovement_Timestamp();
        if (Is_RotationAgainstEachOther(value_One, value_Two)) return MovementState.DualWheel_Turn;
        return MovementState.ViewAxis_Motion;
    }

    protected double MaxAbsolutRotation(double rotationOne, double rotationTwo)
    {
        return Math.Max(Math.Abs(rotationOne), Math.Abs(rotationTwo));
    }

    private void Reset_LastMovement_Timestamp()
    {
        LastMovement_Timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    #region Getter
    public static MappingMode Get_Mode()
    {
        return Mapping._Mapping.Mode;
    }
    public static int Get_ButtonPressingThreshold()
    {
        return Mapping._Mapping.ButtonPressing_Threshold;
    }
    public static int Get_WheelMovementThreshold()
    {
        return Mapping._Mapping.WheelMovement_Threshold;
    }
    public static Wheelchair Get_Wheelchair()
    {
        return Mapping._Mapping.Wheelchair;
    }
    #endregion
}