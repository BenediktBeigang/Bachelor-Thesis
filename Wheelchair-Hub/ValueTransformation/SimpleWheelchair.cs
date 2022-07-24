public class SimpleWheelchair : ValueTransformation
{
    public SimpleWheelchair(double wheelRadius, double chairWidth) : base(wheelRadius, chairWidth) { }

    public override (WheelchairMode, double, double) NextTransformedValues()
    {
        (double Left, double Right) Rotation = GlobalData.Rotations();
        double speed = 0;
        double rotation = 0;

        double rotationLeftAbs = Math.Abs(Rotation.Left);
        double rotationRightAbs = Math.Abs(Rotation.Right);
        double minimum = Math.Min(rotationLeftAbs, rotationRightAbs); // distance both wheels traveld 

        if (IsRotationAgainstEachOther(Rotation.Left, Rotation.Right))
        {
            speed = 0;
            rotation = RatioToDegree(minimum, wheelchair.InnerTurningCircle);
        }
        else
        {
            speed = (Rotation.Left + Rotation.Right) / 2;
            rotation = 0;
        }

        return (WheelchairMode.Wheelchair_Realistic, speed, rotation);
    }
}