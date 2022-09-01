public class SimpleWheelchair : Mapping
{
    public SimpleWheelchair(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold, int wheelMovementMax = 0)
    : base(MappingMode.Wheelchair_Simple, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth, wheelMovementMax) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        switch (StateDetection.Get_MovementState_SimpleWheelchair(ref rotations))
        {
            case MovementState.StandingStill: return new ControllerInput();
            case MovementState.ViewAxis_Motion: return Handle_ViewAxisMotion(rotations);
            case MovementState.DualWheel_Turn: return Handle_DualWheelTurn(rotations);
            case MovementState.SingleWheel_Turn: return Handle_SingleWheelTurn(rotations);
        }
        return new ControllerInput();
    }

    public override MovementState Get_MovementState(Rotations rotations)
    {
        return StateDetection.Get_MovementState_SimpleWheelchair(ref rotations);
    }

    private ControllerInput Handle_ViewAxisMotion(Rotations rotations)
    {
        double moveVector = AbsoluteInterpolation(rotations);
        moveVector = _Mapping.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        return new ControllerInput()
        {
            LeftThumbY = AngularVelocityToControllerAxis_Move(moveVector)
        };
    }

    private ControllerInput Handle_DualWheelTurn(Rotations rotations)
    {
        double turnVector = AbsoluteInterpolation(rotations);
        turnVector = Wheelchair.RatioToDegree(turnVector, Wheelchair.InnerTurningCircle);
        turnVector = _Mapping.Is_LeftRotation(rotations) ? -turnVector : turnVector;
        return new ControllerInput()
        {
            RightThumbX = AngularVelocityToControllerAxis(turnVector)
        };
    }

    private ControllerInput Handle_SingleWheelTurn(Rotations rotations)
    {
        (double moveVector, double turnVector) vectors = DualWheel(rotations);
        return new ControllerInput()
        {
            LeftThumbY = AngularVelocityToControllerAxis(vectors.moveVector),
            RightThumbX = AngularVelocityToControllerAxis(vectors.turnVector)
        };
    }
}