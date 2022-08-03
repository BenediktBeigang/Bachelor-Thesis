public class RealisticWheelchair : WheelchairMovement
{
    public RealisticWheelchair(double wheelRadius, double chairWidth)
    : base(wheelRadius, chairWidth, MappingMode.Wheelchair_Realistic)
    {
    }

    public override (double, double) Values_Next(short rawValue_One, short rawValue_Two, double value_One, double value_Two)
    {
        double rotationLeftAbs = Math.Abs(value_One);
        double rotationRightAbs = Math.Abs(value_Two);
        double subtraction = Math.Abs(rotationLeftAbs - rotationRightAbs);
        double minimum = Math.Min(rotationLeftAbs, rotationRightAbs);

        double rotation = 0;
        double speed = 0;

        // Calculation of one-wheel movement
        (double distance, double rotation) singleWheelMovement = SingleWheel(subtraction, IsForwardRotation(value_One, value_Two));
        speed += singleWheelMovement.distance;
        rotation += IsLeftRotation(value_One, value_Two) ? -singleWheelMovement.rotation : singleWheelMovement.rotation;

        // Calculation of dual-wheel movement
        (double distance, double rotation) dualWheelMovement = DualWheel(minimum, IsRotationAgainstEachOther(value_One, value_Two));
        speed += IsForwardRotation(value_One, value_Two) ? dualWheelMovement.distance : -dualWheelMovement.distance;
        rotation += IsLeftRotation(value_One, value_Two) ? -dualWheelMovement.rotation : dualWheelMovement.rotation;

        return (speed, rotation);
    }
}