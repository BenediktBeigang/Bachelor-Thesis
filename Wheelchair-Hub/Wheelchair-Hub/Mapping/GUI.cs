public class GUI : Mapping
{
    public GUI(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold, int wheelMovementMax = 0)
    : base(MappingMode.GUI, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth, wheelMovementMax) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        switch (Get_MovementState(rotations))
        {
            case MovementState.StandingStill: return new ControllerInput();
            case MovementState.ViewAxis_Motion: return YAxis(rotations);
            case MovementState.DualWheel_Turn: return XAxis(rotations);
            case MovementState.SingleWheel_Turn: return What_ButtonPressed(rotations);
        }
        return new ControllerInput();
    }

    private ControllerInput YAxis(Rotations rotations)
    {
        double moveVector = AbsoluteInterpolation(rotations);
        moveVector = Wheelchair.Is_RotationSumForeward(rotations) ? moveVector : -moveVector;
        return new ControllerInput()
        {
            LeftThumbY = Wheelchair.AngularVelocityToControllerAxis(moveVector)
        };
    }

    private ControllerInput XAxis(Rotations rotations)
    {
        double turnVector = AbsoluteInterpolation(rotations);
        turnVector = Wheelchair.Is_LeftRotation(rotations) ? -turnVector : turnVector;
        return new ControllerInput()
        {
            LeftThumbX = Wheelchair.AngularVelocityToControllerAxis(turnVector)
        };
    }
}