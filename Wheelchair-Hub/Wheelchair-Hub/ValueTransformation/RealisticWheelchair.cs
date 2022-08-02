public class RealisticWheelchair : ValueTransformation
{
    public RealisticWheelchair(double wheelRadius, double chairWidth) : base(wheelRadius, chairWidth) { }

    #region Calculations
    public override (double, double) TransformedValues_Next(short rawValue_One, short rawValue_Two, double value_One, double value_Two)
    {
        // (double Left, double Right) Rotation = GlobalData.Rotations();

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
    #endregion


    #region Wheel-Cases
    private (double distance, double rotation) SingleWheel(double wheelDistance, bool isForwardRotation)
    {
        double omega = wheelDistance / wheelchair.OuterTurningCircle;
        double speed = omega * wheelchair.InnerTurningCircle;
        speed *= isForwardRotation ? 1 : -1;
        double rotation = omega * 360;
        return (speed, rotation);
    }

    private (double distance, double rotation) DualWheel(double wheelDistance, bool isRotationAgainstEachOther)
    {
        if (isRotationAgainstEachOther)
            return (0, RatioToDegree(wheelDistance, wheelchair.InnerTurningCircle));
        else
            return (wheelDistance, 0);
    }
    #endregion

    private void PrintMovement(double rotationLeft, double rotationRight)
    {
        double rotationLeftAbs = Math.Abs(rotationLeft);
        double rotationRightAbs = Math.Abs(rotationRight);
        double subtraction = Math.Abs(rotationLeftAbs - rotationRightAbs);
        double minimum = Math.Min(rotationLeftAbs, rotationRightAbs);

        string print = $"RotationLeft: {rotationLeft} | RotationRigth: {rotationRight}\n"
        + $"AbsolutLeftRotation: {rotationLeftAbs} | AbsolutRightRotation: {rotationRightAbs}\n"
        + $"Subtraction: {subtraction} | Minimum: {minimum}\n"
        + "========================================";
        Console.WriteLine(print);
    }
}