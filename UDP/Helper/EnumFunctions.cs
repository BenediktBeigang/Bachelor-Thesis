public static class EnumFunctions
{
    public static double StepsPerDegree(GyroMode mode)
    {
        switch (mode)
        {
            case GyroMode.Gyro_250: return 131;
            case GyroMode.Gyro_500: return 65.5;
            case GyroMode.Gyro_1000: return 32.8;
            case GyroMode.Gyro_2000: return 16.4;
            default: return 131;
        }
    }
}