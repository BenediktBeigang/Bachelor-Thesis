public abstract class ValueTransformation
{
    public readonly Wheelchair wheelchair;

    public ValueTransformation(double wheelRadius, double chairWidth)
    {
        wheelchair = new Wheelchair(wheelRadius, chairWidth);
    }

    public abstract (double, double) TransformedValues_Next(short rawValue_One, short rawValue_Two, double value_One, double value_Two);

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

    /// <summary> This function gets the sublength of a circumference and calculates the corresponding Degree of that circle </summary>
    protected double RatioToDegree(double length, double circumference)
    {
        return (length / circumference) * 360;
    }
}