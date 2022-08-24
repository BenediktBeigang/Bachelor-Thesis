public class RealisticWheelchair : Mapping
{
    public RealisticWheelchair(double wheelRadius, double chairWidth, int wheelMovement_Threshold, int buttonPressingThreshold, int wheelMovement_Max = 0)
    : base(MappingMode.Wheelchair_Realistic, wheelMovement_Threshold, buttonPressingThreshold, wheelRadius, chairWidth, wheelMovement_Max)
    {
    }

    public override ControllerInput Values_Next(Rotations rotations)
    {
        double moveVector = 0;
        double turningVector = 0;

        // Calculation of one-wheel movement
        (double moveVector, double turningVector) singleWheelComponent = SingleWheel(rotations);
        moveVector += singleWheelComponent.moveVector;
        turningVector += singleWheelComponent.turningVector;

        // Calculation of dual-wheel movement
        (double moveVector, double turningVector) dualWheelMovement = DualWheel(rotations);
        moveVector += dualWheelMovement.moveVector;
        turningVector += dualWheelMovement.turningVector;

        return new ControllerInput()
        {
            LeftThumbY = Wheelchair.AngularVelocityToControllerAxis(moveVector),
            RightThumbX = Wheelchair.AngularVelocityToControllerAxis(turningVector)
        };
    }
}