using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

public class Gyro
{
    #region Fields
    // public
    public CalibrationStatus CalibrationStatus { get; set; }

    // readonly
    public readonly DeviceNumber DeviceNumber;
    public readonly GyroMode Mode;

    // private
    private short BufferPointer;
    private short[] RawValueBuffer;
    private List<int> calibrationValues;
    private short Offset { get; set; }
    private readonly System.Timers.Timer? calibrationTimer;
    private double StepsPerDegree;

    // Constants
    private const short SAMPLE_RATE = 10; // After what time the next calibration-sample is taken
    private const short BUFFER_SIZE = 100;
    #endregion

    #region Initialization
    public Gyro(GyroMode mode, DeviceNumber device)
    {
        RawValueBuffer = new short[BUFFER_SIZE];
        // RawValues = new();
        Mode = mode;
        StepsPerDegree = GyroModeToStepsPerDegree(mode);
        DeviceNumber = device;
        calibrationTimer = new System.Timers.Timer(SAMPLE_RATE);
        calibrationValues = new();
        CalibrationStatus = CalibrationStatus.NOT_CALIBRATED;
    }

    private static double GyroModeToStepsPerDegree(GyroMode mode)
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
    #endregion

    #region Value_Handling
    /// <summary>
    /// Overides oldest RawValue in Buffer with new RawValue.
    /// </summary>
    /// <param name="newValue"></param>
    public void RawValue_Next(short newValue)
    {
        BufferPointer++;
        if (BufferPointer >= BUFFER_SIZE)
        {
            BufferPointer = 0;
        }
        RawValueBuffer[BufferPointer] = newValue;
    }

    /// <summary>
    /// Returns newest RawValue.
    /// </summary>
    /// <returns></returns>
    public short RawValue_Last()
    {
        return RawValueBuffer[BufferPointer];
    }

    /// <summary>
    /// Returns the Buffer, containg the newest RawValues.
    /// </summary>
    /// <returns></returns>
    public short[] RawValues_Buffer()
    {
        short[] result = new short[BUFFER_SIZE];
        short pivot = (short)(BufferPointer + 1);
        Array.Copy(RawValueBuffer, 0, result, pivot, pivot);
        Array.Copy(RawValueBuffer, pivot, result, 0, pivot);
        return result.Reverse().ToArray();
    }

    public double DegreePerSecond()
    {
        return RawValue_Last() / StepsPerDegree;
    }
    #endregion

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
        Offset = (short)(-calibrationValues.Average());
        CalibrationStatus = CalibrationStatus.CALIBRATED;
    }

    private void TakeSample(object sender, ElapsedEventArgs e)
    {
        short value = RawValue_Last();
        if (value != 0) calibrationValues.Add(value);
    }
    # endregion
}