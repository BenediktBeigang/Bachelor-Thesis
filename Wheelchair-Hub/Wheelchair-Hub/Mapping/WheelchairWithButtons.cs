public class WheelchairWithButtons : Mapping
{
    public WheelchairWithButtons(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold, int wheelMovementMax = 0)
    : base(MappingMode.Wheelchair_WithButtons, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth, wheelMovementMax) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        switch (Get_MovementState(rotations))
        {
            case MovementState.StandingStill: return new ControllerInput();
            case MovementState.ViewAxis_Motion: return Move(rotations);
            case MovementState.DualWheel_Turn: return Turn(rotations);
            case MovementState.SingleWheel_Turn: return What_ButtonPressed(rotations);
            case MovementState.Tilt: return Tilt(rotations);
        }
        return new ControllerInput();
    }

    /// <summary>
    /// The Output is the max- or min-value of the angularVelocity, so the controller-ThumbStick is at max deflection.
    /// </summary>
    /// <param name="movement"></param>
    /// <param name="angularVelocity"></param>
    /// <returns></returns>
    private ControllerInput Move(Rotations rotations)
    {
        double resultVector = (Wheelchair.Is_RotationForward(rotations.AngularVelocityLeft)) ? Gyro.ModeAsInteger() : -Gyro.ModeAsInteger();
        short controllerVector = Wheelchair.AngularVelocityToControllerAxis(resultVector);
        return new ControllerInput(0, controllerVector, 0, 0, false, false, false, false);
    }

    private ControllerInput Turn(Rotations rotations)
    {
        double turnVector = AbsoluteInterpolation(rotations);
        // turnVector = Wheelchair.RatioToDegree(turnVector, Wheelchair.InnerTurningCircle);
        turnVector = Wheelchair.Is_LeftRotation(rotations) ? -turnVector : turnVector;
        turnVector *= 10;
        return new ControllerInput()
        {
            RightThumbX = Wheelchair.AngularVelocityToControllerAxis(turnVector)
        };
    }

    private ControllerInput Tilt(Rotations rotations)
    {
        double tiltVector = AbsoluteInterpolation(rotations);
        tiltVector = Wheelchair.Is_RotationSumForeward(rotations) ? tiltVector : -tiltVector;
        tiltVector *= 10;
        return new ControllerInput()
        {
            RightThumbY = Wheelchair.AngularVelocityToControllerAxis(tiltVector)
        };
    }
}