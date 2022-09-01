using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class MappingChecks
{
    private static readonly int ACCELERATION_THRESHOLD = 15;

    public static bool Is_MovementStateChanging(this Mapping mapping, Rotations rotations)
    {
        return rotations.Left.Acceleration + rotations.Right.Acceleration >= ACCELERATION_THRESHOLD;
    }

    public static bool Is_RotationAgainstEachOther(this Mapping mapping, Rotations rotations)
    {
        return ((rotations.Left.AngularVelocity_Smoothed > 0) ^ (rotations.Right.AngularVelocity_Smoothed > 0));
    }

    public static bool Is_LeftRotation(this Mapping mapping, Rotations rotations)
    {
        return (rotations.Left.AngularVelocity_Smoothed < rotations.Right.AngularVelocity_Smoothed);
    }

    public static bool Is_RotationSumForeward(this Mapping mapping, Rotations rotations)
    {
        return (rotations.Left.AngularVelocity_Smoothed + rotations.Right.AngularVelocity_Smoothed) >= 0;
    }
}
