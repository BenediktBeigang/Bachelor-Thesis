public class RealisticWheelchair : Mapping
{
    public RealisticWheelchair(double wheelRadius, double chairWidth, double threshold)
    : base(MappingMode.Wheelchair_Realistic, threshold, wheelRadius, chairWidth)
    {
    }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        double rotationLeftAbs = Math.Abs(rotations.AngularVelocityLeft);
        double rotationRightAbs = Math.Abs(rotations.AngularVelocityRight);
        double subtraction = Math.Abs(rotationLeftAbs - rotationRightAbs);
        double minimum = Math.Min(rotationLeftAbs, rotationRightAbs);

        double rotation = 0;
        double speed = 0;

        // Calculation of one-wheel movement
        (double distance, double rotation) singleWheelMovement = SingleWheel(subtraction, Are_BothRotationsForeward(rotations.AngularVelocityLeft, rotations.AngularVelocityRight));
        speed += singleWheelMovement.distance;
        rotation += Is_LeftRotation(rotations.AngularVelocityLeft, rotations.AngularVelocityRight) ? -singleWheelMovement.rotation : singleWheelMovement.rotation;

        // Calculation of dual-wheel movement
        (double distance, double rotation) dualWheelMovement = DualWheel(minimum, Is_RotationAgainstEachOther(rotations.AngularVelocityLeft, rotations.AngularVelocityRight));
        speed += Are_BothRotationsForeward(rotations.AngularVelocityLeft, rotations.AngularVelocityRight) ? dualWheelMovement.distance : -dualWheelMovement.distance;
        rotation += Is_LeftRotation(rotations.AngularVelocityLeft, rotations.AngularVelocityRight) ? -dualWheelMovement.rotation : dualWheelMovement.rotation;

        // Scales angualar velocity to 16-Bit Controller-Input
        return AngularVelocityToControllerInput(speed, rotation, false, false, false, false);
    }
}