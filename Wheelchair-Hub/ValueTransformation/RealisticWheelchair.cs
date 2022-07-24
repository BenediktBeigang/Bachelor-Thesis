public class RealisticWheelchair : ValueTransformation
{
    public RealisticWheelchair(double wheelRadius, double chairWidth) : base(wheelRadius, chairWidth) { }

    #region Calculations
    public override (WheelchairMode, double, double) NextTransformedValues()
    {
        (double Left, double Right) Rotation = GlobalData.Rotations();

        double rotationLeftAbs = Math.Abs(Rotation.Left);
        double rotationRightAbs = Math.Abs(Rotation.Right);
        double subtraction = Math.Abs(rotationLeftAbs - rotationRightAbs);
        double minimum = Math.Min(rotationLeftAbs, rotationRightAbs);

        double rotation = 0;
        double speed = 0;

        // Calculation of one-wheel movement
        (double, double) singleWheelMovement = SingleWheel(subtraction, IsForwardRotation(Rotation.Left, Rotation.Right));
        speed += singleWheelMovement.Item1;
        rotation += IsLeftRotation(Rotation.Left, Rotation.Right) ? -singleWheelMovement.Item2 : singleWheelMovement.Item2;

        // Calculation of dual-wheel movement
        (double, double) dualWheelMovement = DualWheel(minimum, IsRotationAgainstEachOther(Rotation.Left, Rotation.Right));
        speed += IsForwardRotation(Rotation.Left, Rotation.Right) ? dualWheelMovement.Item1 : -dualWheelMovement.Item1;
        rotation += IsLeftRotation(Rotation.Left, Rotation.Right) ? -dualWheelMovement.Item2 : dualWheelMovement.Item2;

        return (WheelchairMode.Wheelchair_Realistic, speed, rotation);
    }

    private (double, double) SingleWheel(double wheelDistance, bool isForwardRotation)
    {
        double omega = wheelDistance / wheelchair.TurningCircle;
        double speed = omega * wheelchair.InnerTurningCircle;
        speed *= isForwardRotation ? 1 : -1;
        double rotation = omega * 360;
        return (speed, rotation);
    }

    private (double, double) DualWheel(double wheelDistance, bool isRotationAgainstEachOther)
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