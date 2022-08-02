public class SimpleWheelchair : ValueTransformation
{
    private const double MOVEMENT_THRESHOLD = 0.5;

    public SimpleWheelchair(double wheelRadius, double chairWidth) : base(wheelRadius, chairWidth) { }

    public override (double, double) TransformedValues_Next(short rawValue_One, short rawValue_Two, double value_One, double value_Two)
    {
        double valueInterpolation = Math.Abs((value_One + value_Two) / 2);
        switch (Movement(value_One, value_One))
        {
            case MovementState.ViewAxis_Motion:
                return WheelchairMovement.DualWheel_Move(valueInterpolation);
            case MovementState.SingleWheel_Turn:
                return WheelchairMovement.SingleWheel(
                    Math.Max(Math.Abs(value_One), Math.Abs(value_Two)),
                    wheelchair.OuterTurningCircle,
                    wheelchair.InnerTurningCircle,
                    IsForwardRotation(value_One, value_Two));
            case MovementState.DualWheel_Turn:
                return WheelchairMovement.DualWheel_Turn(
                    valueInterpolation,
                    wheelchair.InnerTurningCircle);
            default: return (0, 0);
        }
    }

    private MovementState Movement(double value_One, double value_Two)
    {
        if (AreBothRotationsUnderThreshold(value_One, value_Two)) return MovementState.StandingStill;
        if (IsOneRotationUnderThreshold(value_One, value_Two)) return MovementState.SingleWheel_Turn;
        if (IsRotationAgainstEachOther(value_One, value_Two)) return MovementState.DualWheel_Turn;
        return MovementState.ViewAxis_Motion;
    }

    private static bool IsOneRotationUnderThreshold(double value_One, double value_Two)
    {
        bool b1 = Math.Abs(value_One) < MOVEMENT_THRESHOLD;
        bool b2 = Math.Abs(value_Two) < MOVEMENT_THRESHOLD;
        return b1 ^ b2;
    }

    private static bool AreBothRotationsUnderThreshold(double value_One, double value_Two)
    {
        bool b1 = Math.Abs(value_One) < MOVEMENT_THRESHOLD;
        bool b2 = Math.Abs(value_Two) < MOVEMENT_THRESHOLD;
        return b1 && b2;
    }
}