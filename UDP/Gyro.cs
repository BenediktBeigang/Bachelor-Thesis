using System.Timers;

public class Gyro
{
    // Consturctor
    public readonly GyroMode Gyro_Mode;
    private readonly int CalibrationTime;
    private readonly System.Timers.Timer? calibrationTimer;

    // public
    public int RawValue
    {
        get
        {
            return _rawValue + Offset;
        }
        set
        {
            _rawValue = value;
        }
    }
    public double DegreePerSecond()
    {
        return _rawValue / StepsPerDegree(Gyro_Mode);
    }

    // private
    private int _rawValue;
    private List<int> calibrationValues;
    private int Offset { get; set; }

    // Constants
    private const int SAMPLE_RATE = 10;

    public Gyro(GyroMode mode, int calibrationTime)
    {
        Gyro_Mode = mode;
        CalibrationTime = calibrationTime;
        calibrationTimer = new System.Timers.Timer(SAMPLE_RATE);
        calibrationValues = new();
    }

    public void GyroCalibration(int seconds)
    {
        GlobalData.LastMessages.Add("Gyro");

        calibrationTimer!.Elapsed += TakeSample!;
        calibrationTimer!.AutoReset = true;
        calibrationTimer!.Enabled = true;
        Thread.Sleep(CalibrationTime);
        calibrationTimer.Stop();

        Offset = -(int)calibrationValues.Average();
    }

    private void TakeSample(object sender, ElapsedEventArgs e)
    {
        calibrationValues.Add(RawValue);
    }

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