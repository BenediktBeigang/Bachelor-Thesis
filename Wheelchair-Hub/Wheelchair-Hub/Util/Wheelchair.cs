public class Wheelchair
{
    public readonly double Wheel_Radius;
    public readonly double Chair_Width;
    public readonly double Wheel_Diameter;
    public readonly double Wheel_Circumference;
    public readonly double Chair_HalfWidth;
    public readonly double OuterTurningCircle;
    public readonly double InnerTurningCircle;
    public readonly double LengthOfOneDegree;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="wheelRadius">In meter</param>
    /// <param name="chairWidth">In meter</param>
    public Wheelchair(double wheelRadius, double chairWidth)
    {
        Wheel_Radius = wheelRadius;
        Wheel_Diameter = wheelRadius * 2;
        Wheel_Circumference = (double)Math.PI * Wheel_Diameter;
        Chair_Width = chairWidth;
        Chair_HalfWidth = chairWidth / 2;
        OuterTurningCircle = (double)Math.PI * chairWidth * 2;
        InnerTurningCircle = (double)Math.PI * chairWidth;
        LengthOfOneDegree = Wheel_Circumference / 360;
    }

    /// <summary>
    /// The ratio of the covered distance, to his corresponding full circle, is calculated to a rotation in degree. 
    /// </summary>
    public static double RatioToDegree(double length, double circumference)
    {
        return (length / circumference) * 360;
    }

    public override string ToString()
    => "========================================\n"
    + $"Radius|Diameter: {Wheel_Radius} | {Wheel_Diameter}\n"
    + $"Circumference: {Wheel_Circumference}\n"
    + $"ChairWidth(Half): {Chair_Width} | {Chair_HalfWidth}\n"
    + $"(Inner)TurningCircle: {InnerTurningCircle} | {OuterTurningCircle}"
    + "\n========================================";
}