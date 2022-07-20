using System.Timers;

public class Gyro
{
    // Consturctor
    public readonly DeviceNumber DeviceNumber;
    public readonly GyroMode Mode;

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
        return _rawValue / StepsPerDegree(Mode);
    }
    public CalibrationStatus CalibrationStatus { get; set; }

    // private
    private int _rawValue;
    private List<int> calibrationValues;
    private int Offset { get; set; }
    private readonly System.Timers.Timer? calibrationTimer;

    // Constants
    private const int SAMPLE_RATE = 10;

    public Gyro(GyroMode mode, DeviceNumber device)
    {
        Mode = mode;
        DeviceNumber = device;
        calibrationTimer = new System.Timers.Timer(SAMPLE_RATE);
        calibrationValues = new();
        CalibrationStatus = CalibrationStatus.NOT_CALIBRATED;
    }

    public void Calibration(int seconds)
    {
        CalibrationStatus = CalibrationStatus.CALIBRATING;
        GlobalData.LastMessages.Add($"Calibrating Gyro for {seconds}seconds.");
        calibrationValues = new();
        calibrationTimer!.Elapsed += TakeSample!;
        calibrationTimer!.AutoReset = true;
        calibrationTimer!.Enabled = true;
        Task.Run(() => CalibrationEnd(seconds));
    }

    private void CalibrationEnd(int seconds)
    {
        Thread.Sleep(seconds * 1000);
        calibrationTimer!.Stop();
        Offset = -(int)calibrationValues.Average();
        CalibrationStatus = CalibrationStatus.CALIBRATED;
    }

    private void TakeSample(object sender, ElapsedEventArgs e)
    {
        calibrationValues.Add(RawValue);
    }

    public static double StepsPerDegree(GyroMode mode)
    {
        switch (mode)
        {
            case GyroMode.GYRO_250: return 131;
            case GyroMode.GYRO_500: return 65.5;
            case GyroMode.GYRO_1000: return 32.8;
            case GyroMode.GYRO_2000: return 16.4;
            default: return 131;
        }
    }
}