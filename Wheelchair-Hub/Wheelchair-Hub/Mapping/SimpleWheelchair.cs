public class SimpleWheelchair : Mapping
{
    private const double MOVEMENT_THRESHOLD = 0.5;

    public SimpleWheelchair(double wheelRadius, double chairWidth, double threshold)
    : base(MappingMode.Wheelchair_Simple, threshold, wheelRadius, chairWidth) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        double valueInterpolation = Math.Abs((rotations.AngularVelocityLeft + rotations.AngularVelocityRight) / 2);
        (double, double) result = (0, 0);
        switch (Analyse_Movement(rotations.AngularVelocityLeft, rotations.AngularVelocityLeft))
        {
            case MovementState.ViewAxis_Motion: result = DualWheel_Move(valueInterpolation); break;
            case MovementState.DualWheel_Turn: result = DualWheel_Turn(valueInterpolation); break;
            case MovementState.SingleWheel_Turn:
                result =
                SingleWheel(MaxAbsolutRotation(rotations.AngularVelocityLeft, rotations.AngularVelocityRight),
                Are_BothRotationsForeward(rotations.AngularVelocityLeft, rotations.AngularVelocityRight));
                break;
        }
        return AngularVelocityToControllerInput(result.Item1, result.Item2, false, false, false, false);
    }
}