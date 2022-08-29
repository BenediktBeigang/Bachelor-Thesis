public class MovementStateDetection
{
    public readonly int DualWheel_Threshold;
    public readonly int SingleWheel_Threshold; // gyro value
    private const int ACCELERATION_THRESHOLD = 15;

    public MovementStateDetection(int dualWheel_Threshold, int singleWheel_Threshold)
    {
        DualWheel_Threshold = dualWheel_Threshold;
        SingleWheel_Threshold = singleWheel_Threshold;
    }

    public MovementState Get_MovementState_GUI(Rotations rotations)
    {
        if (Is_StandingStill(rotations)) return MovementState.StandingStill;
        if (Is_ButtonPressed(rotations)) return MovementState.SingleWheel_Turn;
        if (Is_DualWheelTurn(rotations)) return MovementState.DualWheel_Turn;
        if (Is_ViewAxisMotion(rotations)) return MovementState.ViewAxis_Motion;
        return MovementState.StandingStill;
    }

    public MovementState Get_MovementState_SimpleWheelchair(Rotations rotations)
    {
        if (Is_StandingStill(rotations)) return MovementState.StandingStill;
        if (Is_SingleWheel(rotations)) return MovementState.SingleWheel_Turn;
        if (Is_DualWheelTurn(rotations)) return MovementState.DualWheel_Turn;
        if (Is_ViewAxisMotion(rotations)) return MovementState.ViewAxis_Motion;
        return MovementState.StandingStill;
    }

    /// <summary>
    /// Returns the current state of movement, dependend by the rotation of the wheels. TODO...
    /// </summary>
    /// <param name="value_One"></param>
    /// <param name="value_Two"></param>
    /// <returns></returns>
    public MovementState Get_MovementState_WheelchairWithButtons(Rotations rotations)
    {
        if (Is_StandingStill(rotations)) return MovementState.StandingStill;
        if (Is_Tilt(rotations)) return MovementState.Tilt;
        if (Is_ButtonPressed(rotations)) return MovementState.SingleWheel_Turn;
        if (Is_DualWheelTurn(rotations)) return MovementState.DualWheel_Turn;
        if (Is_ViewAxisMotion_WithThreshold(rotations)) return MovementState.ViewAxis_Motion;
        return MovementState.StandingStill;
    }

    #region State-Checks
    private bool Is_StandingStill(Rotations rotations)
    {
        return (rotations.Left.RawValue is 0 && rotations.Right.RawValue is 0);
    }

    private bool Is_Tilt(Rotations rotations)
    {
        bool b1 = (Math.Abs(rotations.Left.AngularVelocity_Smoothed) <= DualWheel_Threshold) && (Math.Abs(rotations.Right.AngularVelocity_Smoothed) <= DualWheel_Threshold);
        bool b2 = Is_RotationAgainstEachOther(rotations) is false;
        bool b3 = Is_MovementStateChanging() is false;
        return b1 && b2 && b3;
    }

    private bool Is_ViewAxisMotion(Rotations rotations)
    {
        return (Is_RotationAgainstEachOther(rotations) is false);
    }

    private bool Is_ViewAxisMotion_WithThreshold(Rotations rotations)
    {
        bool b1 = (Math.Abs(rotations.Left.AngularVelocity_Smoothed) > DualWheel_Threshold) && (Math.Abs(rotations.Right.AngularVelocity_Smoothed) > DualWheel_Threshold);
        bool b2 = (Is_RotationAgainstEachOther(rotations) is false);
        return b1 && b2;
    }

    private bool Is_DualWheelTurn(Rotations rotations)
    {
        return ((rotations.Left.AngularVelocity_Smoothed > 0) ^ (rotations.Right.AngularVelocity_Smoothed > 0));
    }

    private bool Is_SingleWheel(Rotations rotations)
    {
        bool b1 = Math.Abs(rotations.Left.AngularVelocity_Smoothed) < DualWheel_Threshold;
        bool b2 = Math.Abs(rotations.Right.AngularVelocity_Smoothed) < DualWheel_Threshold;
        return b1 ^ b2;
    }

    private bool Is_ButtonPressed(Rotations rotations)
    {
        bool b1 = (Math.Abs(rotations.Left.AngularVelocity_Smoothed) < SingleWheel_Threshold) ^ (Math.Abs(rotations.Right.AngularVelocity_Smoothed) < SingleWheel_Threshold);
        bool b2 = (Math.Abs(rotations.Left.AngularVelocity_Smoothed) > DualWheel_Threshold) ^ (Math.Abs(rotations.Right.AngularVelocity_Smoothed) > DualWheel_Threshold);
        return b1 && b2;
    }
    #endregion

    #region Helper
    private static bool Is_MovementStateChanging()
    {
        // return false;
        (double, double) acceleration = Gyro.Acceleration_BothNodes();
        return acceleration.Item1 + acceleration.Item2 >= ACCELERATION_THRESHOLD;
    }

    private static bool Is_RotationAgainstEachOther(Rotations rotations)
    {
        return ((rotations.Left.AngularVelocity_Smoothed > 0) ^ (rotations.Right.AngularVelocity_Smoothed > 0));
    }
    #endregion
}