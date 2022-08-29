public class SimpleWheelchair : Mapping
{
    public SimpleWheelchair(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold, int wheelMovementMax = 0)
    : base(MappingMode.Wheelchair_Simple, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth, wheelMovementMax) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        switch (StateDetection.Get_MovementState_SimpleWheelchair(rotations))
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
        return StateDetection.Get_MovementState_SimpleWheelchair(rotations);
    }

    private ControllerInput Handle_ViewAxisMotion(Rotations rotations)
    {
        double moveVector = AbsoluteInterpolation(rotations);
        moveVector = Wheelchair.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        return new ControllerInput()
        {
            LeftThumbY = Wheelchair.AngularVelocityToControllerAxis(moveVector)
        };
    }

    private ControllerInput Handle_DualWheelTurn(Rotations rotations)
    {
        double turnVector = AbsoluteInterpolation(rotations);
        turnVector = Wheelchair.RatioToDegree(turnVector, Wheelchair.InnerTurningCircle);
        turnVector = Wheelchair.Is_LeftRotation(rotations) ? -turnVector : turnVector;
        return new ControllerInput()
        {
            RightThumbX = Wheelchair.AngularVelocityToControllerAxis(turnVector)
        };
    }

    private ControllerInput Handle_SingleWheelTurn(Rotations rotations)
    {
        if (Wheelchair.Is_RotationUnderThreshold(rotations.Left.AngularVelocity, StateDetection.DualWheel_Threshold)) rotations.Left.Mute();
        if (Wheelchair.Is_RotationUnderThreshold(rotations.Right.AngularVelocity, StateDetection.DualWheel_Threshold)) rotations.Right.Mute();
        (double moveVector, double turnVector) vectors = DualWheel(rotations);
        return new ControllerInput()
        {
            LeftThumbY = Wheelchair.AngularVelocityToControllerAxis(vectors.moveVector),
            RightThumbX = Wheelchair.AngularVelocityToControllerAxis(vectors.turnVector)
        };
    }
}