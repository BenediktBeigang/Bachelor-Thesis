using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

public class Gyro
{
    // Consturctor
    public readonly DeviceNumber DeviceNumber;
    public readonly GyroMode Mode;

    // public
    public readonly Queue<int> RawValues;
    public int LastRawValue
    {
        get
        {
            return RawValues.Peek() + Offset;
        }
        set
        {
            if (RawValues.Count > VALUE_BUFFER)
            {
                RawValues.Dequeue();
            }
            RawValues.Enqueue(value);
        }
    }
    public double DegreePerSecond()
    {
        return LastRawValue / StepsPerDegree(Mode);
    }
    public CalibrationStatus CalibrationStatus { get; set; }

    // private
    private List<int> calibrationValues;
    private int Offset { get; set; }
    private readonly System.Timers.Timer? calibrationTimer;

    // Constants
    private const int SAMPLE_RATE = 10; // After what time the next calibration-sample is taken
    private const int VALUE_BUFFER = 100;

    public Gyro(GyroMode mode, DeviceNumber device)
    {
        RawValues = new();
        Mode = mode;
        DeviceNumber = device;
        calibrationTimer = new System.Timers.Timer(SAMPLE_RATE);
        calibrationValues = new();
        CalibrationStatus = CalibrationStatus.NOT_CALIBRATED;
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

    # region Calibration
    /// <summary>
    /// Starts calibration of the Gyro of this Node.
    /// For a given time a Timer is collecting samples of Gyro-Values.
    /// After that time the average value is calculated 
    /// and used as offset to improve the precision of the gyro-data.
    /// </summary>
    /// <param name="seconds"></param>
    public void Calibration(int seconds)
    {
        CalibrationStatus = CalibrationStatus.CALIBRATING;
        GlobalData.LastMessages.Add($"Calibrating Gyro for {seconds} seconds.");
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
        calibrationValues.Add(LastRawValue);
    }
    # endregion
}