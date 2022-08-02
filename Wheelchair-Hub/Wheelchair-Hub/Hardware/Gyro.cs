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
    public bool RotationValueFlip { get; set; }
    public GyroMode Mode { get; set; }

    // readonly
    public readonly DeviceNumber DeviceNumber;

    // private
    private short BufferPointer;
    private short[] RawValueBuffer;
    private List<int> calibrationValues;
    private short Offset { get; set; }
    private double StepsPerDegree;

    // Constants
    private const short BUFFER_SIZE = 100;
    #endregion

    #region Initialization
    public Gyro(GyroMode mode, DeviceNumber device, bool flipped = false)
    {
        RawValueBuffer = new short[BUFFER_SIZE];
        Mode = mode;
        StepsPerDegree = GyroModeToStepsPerDegree(mode);
        DeviceNumber = device;
        calibrationValues = new();
        CalibrationStatus = CalibrationStatus.NOT_CALIBRATED;
        RotationValueFlip = flipped;
    }

    public static double GyroModeToStepsPerDegree(GyroMode mode)
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
        if (CalibrationStatus is CalibrationStatus.CALIBRATING) calibrationValues.Add(newValue);
        RawValueBuffer[BufferPointer] = (short)((RotationValueFlip) ? (newValue - Offset) : (newValue - Offset) * -1);
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

    public double DegreePerSecond_Last()
    {
        return RawValue_Last() / StepsPerDegree;
    }
    #endregion

    # region Calibration
    public void Calibration(int seconds)
    {
        CalibrationStatus = CalibrationStatus.CALIBRATING;
        calibrationValues.Clear();
        GlobalData.LastMessages.Add($"Calibrating Gyro for {seconds} seconds.");
        Task.Run(() => CalibrationEnd(seconds));
    }

    private void CalibrationEnd(int seconds)
    {
        Thread.Sleep(seconds * 1000);
        Offset = (short)calibrationValues.Average();
        GlobalData.LastMessages.Add($"Device {DeviceNumber} Offset: {Offset}");
        CalibrationStatus = CalibrationStatus.CALIBRATED;
    }
    # endregion
}