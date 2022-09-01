public abstract class Mapping
{
    public static Mapping _Mapping { get; set; } = new GUI(30, 55.5, 200, 300);
    public readonly int WheelMovement_Max;
    // public readonly int DualWheel_Threshold;
    // public readonly int SingleWheel_Threshold;
    public readonly MappingMode Mode;
    public readonly Wheelchair Wheelchair;
    protected readonly MovementStateDetection StateDetection;

    public Mapping(MappingMode mode, int dualWheel_Threshold, int singleWheel_Threshold, double wheelRadius, double chairWidth, int wheelMovementMax = 0)
    {
        Mode = mode;
        // SingleWheel_Threshold = singleWheel_Threshold;
        // DualWheel_Threshold = dualWheel_Threshold;
        Wheelchair = new Wheelchair(wheelRadius, chairWidth);
        WheelMovement_Max = (wheelMovementMax is 0 || wheelMovementMax > Gyro.ModeAsInteger()) ? Gyro.ModeAsInteger() : wheelMovementMax;
        StateDetection = new MovementStateDetection(dualWheel_Threshold, singleWheel_Threshold);
    }

    #region Change_Mapping
    public void Change_Mapping(MappingMode mode)
    {
        switch (mode)
        {
            case MappingMode.Wheelchair_Realistic:
                _Mapping = new RealisticWheelchair(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, StateDetection.DualWheel_Threshold, StateDetection.SingleWheel_Threshold, WheelMovement_Max); break;
            case MappingMode.Wheelchair_Simple:
                _Mapping = new SimpleWheelchair(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, StateDetection.DualWheel_Threshold, StateDetection.SingleWheel_Threshold, WheelMovement_Max); break;
            case MappingMode.Wheelchair_WithButtons:
                _Mapping = new WheelchairWithButtons(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, StateDetection.DualWheel_Threshold, StateDetection.SingleWheel_Threshold, WheelMovement_Max); break;
            case MappingMode.GUI:
                _Mapping = new GUI(Wheelchair.Wheel_Radius, Wheelchair.Chair_Width, StateDetection.DualWheel_Threshold, StateDetection.SingleWheel_Threshold, WheelMovement_Max); break;
        }
    }

    public void Change_Mapping(MappingMode mode, double wheelRadius, double chairWidth, int wheelMovementThreshold, int buttonPressingThreshold, int wheelMovementMax)
    {
        switch (mode)
        {
            case MappingMode.Wheelchair_Realistic: _Mapping = new RealisticWheelchair(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold, wheelMovementMax); break;
            case MappingMode.Wheelchair_Simple: _Mapping = new SimpleWheelchair(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold, wheelMovementMax); break;
            case MappingMode.Wheelchair_WithButtons: _Mapping = new WheelchairWithButtons(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold, wheelMovementMax); break;
            case MappingMode.GUI: _Mapping = new GUI(wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold, wheelMovementMax); break;
        }
    }
    #endregion

    public abstract ControllerInput Values_Next(Rotations rotations);
    public abstract MovementState Get_MovementState(Rotations rotations);

    #region Movement-Components
    protected (double moveVector, double turningVector) SingleWheel(Rotations rotations)
    {
        // calculate movement
        double theta = WheelOvershoot(rotations) / Wheelchair.OuterTurningCircle;
        double moveVector = theta * Wheelchair.InnerTurningCircle;
        double turningVector = theta * 360;

        // set directions
        moveVector = _Mapping.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        turningVector = _Mapping.Is_LeftRotation(rotations) ? -turningVector : turningVector;

        return (moveVector, turningVector);
    }

    protected (double moveVector, double turningVector) DualWheel(Rotations rotations)
    {
        double moveVector = 0;
        double turningVector = 0;

        if (_Mapping.Is_RotationAgainstEachOther(rotations))
        {
            turningVector = Wheelchair.RatioToDegree(WheelMinimum(rotations), Wheelchair.InnerTurningCircle);
            turningVector = _Mapping.Is_LeftRotation(rotations) ? -turningVector : turningVector;
        }
        else
        {
            moveVector = WheelMinimum(rotations);
            moveVector = _Mapping.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        }

        return (moveVector, turningVector);
    }

    private double WheelOvershoot(Rotations rotations)
    {
        return Math.Abs(Math.Abs(rotations.Left.AngularVelocity) - Math.Abs(rotations.Right.AngularVelocity));
    }

    private double WheelMinimum(Rotations rotations)
    {
        return Math.Min(Math.Abs(rotations.Left.AngularVelocity), Math.Abs(rotations.Right.AngularVelocity));
    }
    #endregion

    protected double AbsoluteInterpolation(Rotations rotations)
    {
        return (Math.Abs(rotations.Left.AngularVelocity) + Math.Abs(rotations.Right.AngularVelocity)) / 2;
    }

    #region Conversion
    public short AngularVelocityToControllerAxis(double value)
    {
        if (Math.Abs(value) >= Gyro.ModeAsInteger()) return (value < 0) ? short.MinValue : short.MaxValue;
        return (short)(value * Gyro.StepsPerDegree);
    }

    public short AngularVelocityToControllerAxis_Move(double value)
    {
        if (Math.Abs(value) >= WheelMovement_Max || WheelMovement_Max == -1) return (value < 0) ? short.MinValue : short.MaxValue;
        double StepsPerDegree_Scaled = short.MaxValue / WheelMovement_Max;
        return (short)(value * StepsPerDegree_Scaled);
    }

    protected ControllerInput What_ButtonPressed(Rotations rotations)
    {
        return new ControllerInput()
        {
            A = StateDetection.Is_RightPositive(rotations),
            B = StateDetection.Is_RightNegative(rotations),
            X = StateDetection.Is_LeftPositive(rotations),
            Y = StateDetection.Is_LeftNegative(rotations)
        };
    }
    #endregion

    #region Getter
    public static MappingMode Get_Mode()
    {
        return Mapping._Mapping.Mode;
    }
    public static Wheelchair Get_Wheelchair()
    {
        return Mapping._Mapping.Wheelchair;
    }
    public static int Get_WheelMovement_Max()
    {
        return Mapping._Mapping.WheelMovement_Max;
    }
    public static MovementStateDetection Get_MovementStateDetection()
    {
        return Mapping._Mapping.StateDetection;
    }
    #endregion
}