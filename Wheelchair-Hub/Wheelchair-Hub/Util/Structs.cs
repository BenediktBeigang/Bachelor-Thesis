using System.Net;

public struct Received
{
    public IPEndPoint Sender;
    public string Message;
    public override string ToString() => $"IP: {Sender.Address.ToString()}\nPORT: {Sender.Port}\nMessage: {Message}";
}

public struct Message
{
    public string Text;
    public DateTime Time;
}

public readonly struct Wheelchair
{
    public Wheelchair()
    {
        double wheelRadius = 0.295;
        double chairWidth = 0.5;
        Wheel_Radius = wheelRadius;
        Wheel_Diameter = wheelRadius * 2;
        Wheel_Circumference = (float)Math.PI * Wheel_Diameter;
        Chair_Width = chairWidth;
        Chair_HalfWidth = chairWidth / 2;
        OuterTurningCircle = (float)Math.PI * chairWidth * 2;
        InnerTurningCircle = (float)Math.PI * chairWidth;
        LengthOfOneDegree = Wheel_Circumference / 360;
    }

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

    public double Wheel_Radius { get; }
    public double Chair_Width { get; }
    public double Wheel_Diameter { get; }
    public double Wheel_Circumference { get; }
    public double Chair_HalfWidth { get; }
    public double OuterTurningCircle { get; }
    public double InnerTurningCircle { get; }
    public double LengthOfOneDegree { get; }
    public override string ToString()
    => "========================================\n"
    + $"Radius|Diameter: {Wheel_Radius} | {Wheel_Diameter}\n"
    + $"Circumference: {Wheel_Circumference}\n"
    + $"ChairWidth(Half): {Chair_Width} | {Chair_HalfWidth}\n"
    + $"(Inner)TurningCircle: {InnerTurningCircle} | {OuterTurningCircle}"
    + "\n========================================";
}