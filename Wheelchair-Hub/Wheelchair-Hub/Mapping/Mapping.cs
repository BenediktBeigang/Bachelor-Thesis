public abstract class Mapping
{
    public static Mapping _Mapping { get; set; } = new GUI(30, 55.5, 200, 300);
    public readonly int WheelMovement_Max;
    public readonly int WheelMovement_Threshold;
    public readonly int ButtonPressing_Threshold;
    public readonly MappingMode Mode;
    public readonly Wheelchair Wheelchair;
    private long LastMovement_Timestamp;

    public Mapping(MappingMode mode, int wheelMovement_Threshold, int buttonPressing_Threshold, double wheelRadius, double chairWidth, int wheelMovementMax = 0)
    {
        Mode = mode;
        ButtonPressing_Threshold = buttonPressing_Threshold;
        WheelMovement_Threshold = wheelMovement_Threshold;
        Wheelchair = new Wheelchair(wheelRadius, chairWidth);
        LastMovement_Timestamp = 0;
        WheelMovement_Max = (wheelMovementMax is 0 || wheelMovementMax > Gyro.GyroModeInterger()) ? Gyro.GyroModeInterger() : wheelMovementMax;
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

    public void Change_Mapping(MappingMode mode, double wheelRadius, double chairWidth, int wheelMovementThreshold, int buttonPressingThreshold, int wheelMovementMax)
    {
        switch (mode)
        {
            case MappingMode.Wheelchair_Realistic: _Mapping = new RealisticWheelchair(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold); break;
            case MappingMode.Wheelchair_Simple: _Mapping = new SimpleWheelchair(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold); break;
            case MappingMode.Wheelchair_WithButtons: _Mapping = new WheelchairWithButtons(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold); break;
            case MappingMode.GUI: _Mapping = new GUI(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold); break;
        }
    }
    #endregion

    public abstract ControllerInput Values_Next(Rotations rotations);

    #region Movement-Components
    protected (double moveVector, double turningVector) SingleWheel(Rotations rotations)
    {
        // calculate movement
        double theta = WheelOvershoot(rotations) / Wheelchair.OuterTurningCircle;
        double moveVector = theta * Wheelchair.InnerTurningCircle;
        double turningVector = theta * 360;

        // set directions
        moveVector = Wheelchair.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        turningVector = Wheelchair.Is_LeftRotation(rotations) ? -turningVector : turningVector;

        return (moveVector, turningVector);
    }

    protected (double moveVector, double turningVector) DualWheel(Rotations rotations)
    {
        double moveVector = 0;
        double turningVector = 0;

        if (Wheelchair.Is_RotationAgainstEachOther(rotations))
        {
            turningVector = Wheelchair.RatioToDegree(WheelMinimum(rotations), Wheelchair.InnerTurningCircle);
            turningVector = Wheelchair.Is_LeftRotation(rotations) ? -turningVector : turningVector;
        }
        else
        {
            moveVector = WheelMinimum(rotations);
            moveVector = Wheelchair.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        }

        return (moveVector, turningVector);
    }

    private double WheelOvershoot(Rotations rotations)
    {
        return Math.Abs(Math.Abs(rotations.AngularVelocityLeft) - Math.Abs(rotations.AngularVelocityRight));
    }

    private double WheelMinimum(Rotations rotations)
    {
        return Math.Min(Math.Abs(rotations.AngularVelocityLeft), Math.Abs(rotations.AngularVelocityRight));
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
    protected MovementState Get_MovementState2(Rotations rotations)
    {
        if (Wheelchair.Are_BothRotationsUnderThreshold(rotations, WheelMovement_Threshold))
        {
            if (rotations.RawLeft is 0 && rotations.RawRight is 0) return MovementState.StandingStill;
            return MovementState.Tilt;
        }
        if (Wheelchair.Is_ExactlyOneRotationUnderThreshold(rotations, WheelMovement_Threshold)) return MovementState.SingleWheel_Turn;

        Reset_LastMovement_Timestamp();
        if (Wheelchair.Is_RotationAgainstEachOther(rotations)) return MovementState.DualWheel_Turn;
        return MovementState.ViewAxis_Motion;
    }

    protected MovementState Get_MovementState(Rotations rotations)
    {
        if (rotations.RawLeft is 0 && rotations.RawRight is 0) return MovementState.StandingStill;
        if (Wheelchair.Is_ExactlyOneRotationUnderThreshold(rotations, WheelMovement_Threshold)) return MovementState.SingleWheel_Turn;

        if (Wheelchair.Is_RotationAgainstEachOther(rotations))
        {
            Reset_LastMovement_Timestamp();
            return MovementState.DualWheel_Turn;
        }
        if (Wheelchair.Are_BothRotationsUnderThreshold(rotations, WheelMovement_Threshold)) return MovementState.Tilt;

        Reset_LastMovement_Timestamp();
        return MovementState.ViewAxis_Motion;
    }

    protected double AbsoluteInterpolation(Rotations rotations)
    {
        return (Math.Abs(rotations.AngularVelocityLeft) + Math.Abs(rotations.AngularVelocityRight)) / 2;
    }

    protected ControllerInput What_ButtonPressed(Rotations rotations)
    {
        if (Is_ButtonPressAllowed() is false) return new ControllerInput();
        bool leftPositive = false;
        bool leftNegative = false;
        bool rightPositive = false;
        bool rightNegative = false;
        if (Math.Abs(rotations.AngularVelocityLeft) > WheelMovement_Threshold)
        {
            if (Wheelchair.Is_RotationForward(rotations.AngularVelocityLeft))
                leftPositive = true;
            else
                leftNegative = true;
        }
        if (Math.Abs(rotations.AngularVelocityRight) > WheelMovement_Threshold)
        {
            if (Wheelchair.Is_RotationForward(rotations.AngularVelocityRight))
                rightPositive = true;
            else
                rightNegative = true;
        }
        return new ControllerInput()
        {
            A = rightPositive,
            B = rightNegative,
            X = leftPositive,
            Y = leftNegative
        };
    }

    #region Button_Delay
    protected bool Is_ButtonPressAllowed()
    {
        long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        long timeBetween = now - LastMovement_Timestamp;
        return (timeBetween > ButtonPressing_Threshold);
    }

    private void Reset_LastMovement_Timestamp()
    {
        LastMovement_Timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
    #endregion

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
    public static int Get_WheelMovement_Max()
    {
        return Mapping._Mapping.WheelMovement_Max;
    }
    #endregion
}