public static class WheelchairMovement
{
    public static (double, double) SingleWheel(
        double wheelDistance,
        double outerTurningCircle,
        double innerTurningCircle,
        bool isForwardRotation)
    {
        double omega = wheelDistance / outerTurningCircle;
        double speed = omega * innerTurningCircle;
        speed *= isForwardRotation ? 1 : -1;
        double rotation = omega * 360;
        return (speed, rotation);
    }

    public static (double, double) DualWheel_Turn(double wheelDistance, double innerTurningCircle)
    {
        return (0, RatioToDegree(wheelDistance, innerTurningCircle));
    }

    public static (double, double) DualWheel_Move(double wheelDistance)
    {
        return (wheelDistance, 0);
    }

    /// <summary> This function gets the sublength of a circumference and calculates the corresponding Degree of that circle </summary>
    private static double RatioToDegree(double length, double circumference)
    {
        return (length / circumference) * 360;
    }
}