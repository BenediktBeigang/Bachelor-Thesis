public class WheelchairWithButtons : Mapping
{
    public WheelchairWithButtons(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold)
    : base(MappingMode.Wheelchair_WithButtons, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth) { }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        (double movement, double rotation) result = (0, 0);
        switch (Get_MovementState(rotations.AngularVelocityLeft, rotations.AngularVelocityRight))
        {
            case MovementState.StandingStill: return new ControllerInput();
            case MovementState.ViewAxis_Motion:
                result.movement = (Is_RotationForward(rotations.AngularVelocityLeft)) ? Gyro.GyroModeInterger() : -Gyro.GyroModeInterger(); break;
            case MovementState.DualWheel_Turn:
                double interpolation = (Math.Abs(rotations.AngularVelocityLeft) + Math.Abs(rotations.AngularVelocityRight)) / 2;
                result = DualWheel_Turn(interpolation);
                result.rotation *= Is_LeftRotation(rotations.AngularVelocityLeft, rotations.AngularVelocityRight) ? -1 : 1;
                break;
            case MovementState.SingleWheel_Turn:
                return What_ButtonPressed(rotations.AngularVelocityLeft, rotations.AngularVelocityRight);
        }
        return AngularVelocityToControllerInput(result.Item1, result.Item2, false, false, false, false);
    }
}