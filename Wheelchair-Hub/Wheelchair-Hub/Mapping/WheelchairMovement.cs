public abstract class WheelchairMovement : Mapping
{
    public readonly Wheelchair wheelchair;

    protected WheelchairMovement(double wheelRadius, double chairWidth, MappingMode mode)
    : base(mode)
    {
        wheelchair = new Wheelchair(wheelRadius, chairWidth);
    }

    /// <summary>
    /// omega: ratio of wheelDistance covered to turning circle
    /// </summary>
    /// <param name="wheelDistance">Distance coverd by a wheel covered</param>
    /// <param name="isForwardRotation"></param>
    /// <returns></returns>
    protected (double, double) SingleWheel(double wheelDistance, bool isForwardRotation)
    {
        double omega = wheelDistance / wheelchair.OuterTurningCircle;
        double speed = omega * wheelchair.InnerTurningCircle;
        speed *= isForwardRotation ? 1 : -1;
        double rotation = omega * 360;
        return (speed, rotation);
    }

    protected (double distance, double rotation) DualWheel(double wheelDistance, bool isRotationAgainstEachOther)
    {
        if (isRotationAgainstEachOther)
            return DualWheel_Turn(wheelDistance);
        else
            return DualWheel_Move(wheelDistance);
    }

    protected (double, double) DualWheel_Turn(double wheelDistance)
    {
        return (0, RatioToDegree(wheelDistance, wheelchair.InnerTurningCircle));
    }

    protected (double, double) DualWheel_Move(double wheelDistance)
    {
        return (wheelDistance, 0);
    }

    /// <summary>
    /// The ratio of the covered distance, to his corresponding full circle, is calculated to a rotation in degree. 
    /// </summary>
    protected double RatioToDegree(double length, double circumference)
    {
        return (length / circumference) * 360;
    }
}