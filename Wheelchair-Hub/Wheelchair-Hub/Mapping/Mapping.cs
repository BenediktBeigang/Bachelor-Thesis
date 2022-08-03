public abstract class Mapping
{
    public static Mapping _Mapping { get; set; } = new GUI();
    public static MappingMode Mode { get; set; }

    public Mapping(MappingMode mode)
    {
        Mode = mode;
    }

    public abstract (double, double) Values_Next(short rawValue_One, short rawValue_Two, double value_One, double value_Two);

    protected bool IsLeftRotation(double rotationLeft, double rotationRight)
    {
        return (rotationLeft < rotationRight);
    }

    protected bool IsForwardRotation(double rotationLeft, double rotationRight)
    {
        return (rotationLeft + rotationRight) >= 0;
    }

    protected bool IsRotationAgainstEachOther(double rotationLeft, double rotationRight)
    {
        return (rotationLeft > 0 ^ rotationRight > 0);
    }
}