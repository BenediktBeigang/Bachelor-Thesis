public class WheelchairWithButtons : Mapping
{
    public WheelchairWithButtons(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold, int wheelMovementMax = 0)
    : base(MappingMode.Wheelchair_WithButtons, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth, wheelMovementMax) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        switch (StateDetection.Get_MovementState_WheelchairWithButtons(rotations))
        {
            case MovementState.StandingStill: return new ControllerInput();
            case MovementState.ViewAxis_Motion: return Move(rotations);
            case MovementState.DualWheel_Turn: return Turn(rotations);
            case MovementState.SingleWheel_Turn: return What_ButtonPressed(rotations);
            case MovementState.Tilt: return Tilt(rotations);
        }
        return new ControllerInput();
    }

    public override MovementState Get_MovementState(Rotations rotations)
    {
        return StateDetection.Get_MovementState_WheelchairWithButtons(rotations);
    }

    private ControllerInput Move(Rotations rotations)
    {
        double moveVector = AbsoluteInterpolation(rotations);
        moveVector = (_Mapping.Is_RotationSumForeward(rotations)) ? moveVector : -moveVector;
        return new ControllerInput()
        {
            LeftThumbY = AngularVelocityToControllerAxis_Move(moveVector)
        };
    }

    private ControllerInput Turn(Rotations rotations)
    {
        double turnVector = AbsoluteInterpolation(rotations);
        turnVector = _Mapping.Is_LeftRotation(rotations) ? -turnVector : turnVector;
        turnVector *= 10;
        return new ControllerInput()
        {
            RightThumbX = AngularVelocityToControllerAxis(turnVector)
        };
    }

    private ControllerInput Tilt(Rotations rotations)
    {
        double tiltVector = AbsoluteInterpolation(rotations);
        tiltVector = _Mapping.Is_RotationSumForeward(rotations) ? tiltVector : -tiltVector;
        tiltVector *= 10;
        return new ControllerInput()
        {
            RightThumbY = AngularVelocityToControllerAxis(tiltVector)
        };
    }
}