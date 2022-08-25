using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

public class Gyro
{
    #region Fields
    // static
    private static GyroMode mode;
    public static GyroMode Mode
    {
        get { return mode; }
        set
        {
            mode = value;
            stepsPerDegree = GyroModeToStepsPerDegree(value);
        }
    }
    private static double stepsPerDegree;
    public static double StepsPerDegree
    {
        get { return stepsPerDegree; }
    }

    // public
    public CalibrationStatus CalibrationStatus { get; set; }
    public bool RotationValueFlip { get; set; }

    // readonly
    public readonly DeviceNumber DeviceNumber;

    // private
    private short BufferPointer;
    private short[] RawValueBuffer;
    private List<int> CalibrationValues;
    private short Offset { get; set; }

    // Constants
    private const short BUFFER_SIZE = 100;
    private const int CALIBRATION_TIME = 3; // in seconds
    #endregion

    public Gyro(GyroMode mode, DeviceNumber device, bool flipped = false)
    {
        RawValueBuffer = new short[BUFFER_SIZE];
        stepsPerDegree = GyroModeToStepsPerDegree(mode);
        DeviceNumber = device;
        CalibrationValues = new();
        CalibrationStatus = CalibrationStatus.NOT_CALIBRATED;
        RotationValueFlip = flipped;
    }

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
        if (CalibrationStatus is CalibrationStatus.CALIBRATING) CalibrationValues.Add(newValue);
        short transformedValue = (short)((RotationValueFlip) ? (newValue - Offset) : (newValue - Offset) * -1);
        double NOISE_THRESHOLD = StepsPerDegree; // Threshold is set to 1 degree
        transformedValue = (short)((Math.Abs(transformedValue) > NOISE_THRESHOLD) ? transformedValue : 0);
        RawValueBuffer[BufferPointer] = transformedValue;
    }

    /// <summary>
    /// Returns newest RawValue.
    /// </summary>
    /// <returns></returns>
    public short RawValue_Last()
    {
        return RawValueBuffer[BufferPointer];
    }

    public short[] RawValue_Last(int count)
    {
        short[] result = new short[count];
        int resultCounter = 0;
        int bufferPointer = BufferPointer;
        while (resultCounter < count)
        {
            if (bufferPointer < 0)
            {
                bufferPointer = BUFFER_SIZE - 1;
                continue;
            }
            result[resultCounter] = RawValueBuffer[bufferPointer];
            resultCounter++;
            bufferPointer--;
        }
        return result;
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

    public double[] DegreePerSecond_Last(int count)
    {
        double[] result = new double[count];
        short[] rawValues = RawValue_Last(count);
        for (int i = 0; i < count; i++)
        {
            result[i] = rawValues[i] / StepsPerDegree;
        }
        return result;
    }

    public double Acceleration()
    {
        const int samples = 10;
        double[] values = DegreePerSecond_Last(samples);
        double[] accelerations = new double[samples - 1];
        for (int i = 0; i < samples - 1; i++)
        {
            accelerations[i] = Math.Abs(values[i] - values[i + 1]);
        }
        return accelerations.Average();
    }

    public static (double, double) Acceleration_BothNodes()
    {
        return (Node.Node_One.Gyro.Acceleration(), Node.Node_Two.Gyro.Acceleration());
    }
    #endregion

    #region Calibration
    /// <summary>
    /// Checks if Calibration is requested and starts the Calibration-Process if true.
    /// </summary>
    /// <param name="node"></param>
    public static void Check_Calibration(object sender, ElapsedEventArgs e)
    {
        if (Node.Node_One.Gyro.CalibrationStatus is CalibrationStatus.REQUESTED) Node.Node_One.Gyro.Start_Calibration(CALIBRATION_TIME);
        if (Node.Node_Two.Gyro.CalibrationStatus is CalibrationStatus.REQUESTED) Node.Node_Two.Gyro.Start_Calibration(CALIBRATION_TIME);
    }

    public static void Request_Calibration()
    {
        if (Node.Node_One.ConnectionType is not ConnectionType.NOTHING) Node.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
        if (Node.Node_Two.ConnectionType is not ConnectionType.NOTHING) Node.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
    }

    public void Start_Calibration(int seconds)
    {
        CalibrationStatus = CalibrationStatus.CALIBRATING;
        CalibrationValues.Clear();
        Terminal.Add_Message($"Calibrating Gyro for {seconds} seconds.");
        Task.Run(() => Stop_Calibration(seconds));
    }

    private void Stop_Calibration(int seconds)
    {
        Thread.Sleep(seconds * 1000);
        Offset = (short)CalibrationValues.Average();
        Terminal.Add_Message($"Device {DeviceNumber} Offset: {Offset}");
        CalibrationStatus = CalibrationStatus.CALIBRATED;
    }
    #endregion

    #region Conversion
    private static double GyroModeToStepsPerDegree(GyroMode mode)
    {
        switch (mode)
        {
            case GyroMode.GYRO_250: return 131.068;
            case GyroMode.GYRO_500: return 65.534;
            case GyroMode.GYRO_1000: return 32.767;
            case GyroMode.GYRO_2000: return 16.3835;
            default: return 131;
        }
    }

    public static int ModeAsInteger()
    {
        switch (mode)
        {
            case GyroMode.GYRO_250: return 250;
            case GyroMode.GYRO_500: return 500;
            case GyroMode.GYRO_1000: return 1000;
            case GyroMode.GYRO_2000: return 2000;
            default: return 250;
        }
    }
    #endregion
}