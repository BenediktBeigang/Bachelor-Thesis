public class GUI : Mapping
{
    public GUI(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold)
    : base(MappingMode.GUI, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        double valueInterpolation = Math.Abs((rotations.AngularVelocityLeft + rotations.AngularVelocityRight) / 2);
        (double, double) result = (0, 0);
        switch (Get_MovementState(rotations.AngularVelocityLeft, rotations.AngularVelocityRight))
        {
            case MovementState.StandingStill: return new ControllerInput();
            case MovementState.ViewAxis_Motion: result = DualWheel_Move(valueInterpolation); break;
            case MovementState.DualWheel_Turn: result = DualWheel_Turn(valueInterpolation); break;
            case MovementState.SingleWheel_Turn:
                return What_ButtonPressed(rotations.AngularVelocityLeft, rotations.AngularVelocityRight);
        }
        return AngularVelocityToControllerInput(result.Item1, result.Item2, false, false, false, false);
    }
}