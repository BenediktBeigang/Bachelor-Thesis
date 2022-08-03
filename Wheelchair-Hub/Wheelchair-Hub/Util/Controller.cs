using System.Timers;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

public static class Controller
{
    /// <summary>
    /// Determines what rotation-speed is equivalent with the maximum controller-input
    /// </summary>
    private const int CLICK_DELAY = 200;
    private const int CONTROLLER_MAX = 32768;
    private static ViGEmClient? client;
    private static IXbox360Controller? controller;

    #region Controller API
    public static void Start()
    {
        client = new ViGEmClient();
        controller = client.CreateXbox360Controller();
        controller.Connect();
        Terminal.Add_Message($"Controller connected!");
    }

    public static void Refresh_Controller(object sender, ElapsedEventArgs e)
    {
        if (controller is null) return;
        (short RawLeft, short RawRight, double Left, double Right) rawValues = Node.Rotations();
        (double, double) controllerValues = Mapping._Mapping!.Values_Next(rawValues.RawLeft, rawValues.RawRight, rawValues.Left, rawValues.Right);
        ValuesToController(controllerValues);
    }

    public static void Click()
    {
        if (controller is null) return;
        controller.SetButtonState(Xbox360Button.A, true);
        Task.Run(() => Click_Delay());
    }
    #endregion

    /// <summary>
    /// Uses the given values to trigger a controller-input.
    /// If the WheelchairMode is set to a WheelchairOutput v1 and v2 representing movement and rotation.
    /// If the WheelchairMode is set to Mouse v1 and v2 representing the Values for the y- and x-axis.
    /// </summary>
    /// <param name="values"></param>
    private static void ValuesToController((double, double) values)
    {
        double GyroModeScalar = Gyro.StepsPerDegree;
        short v1 = (short)(values.Item1 * GyroModeScalar);
        short v2 = (short)(values.Item2 * GyroModeScalar);
        switch (Mapping.Mode)
        {
            case MappingMode.Wheelchair_Realistic: Handle_Wheelchair(v1, v2); break;
            case MappingMode.Wheelchair_Simple: Handle_Wheelchair(v1, v2); break;
            case MappingMode.GUI: Handle_Mouse(v1, v2); break;
        }
    }

    #region Handle Inputs
    private static void Handle_Wheelchair(short move, short rotation)
    {
        controller!.SetAxisValue(Xbox360Axis.LeftThumbY, move);
        controller!.SetAxisValue(Xbox360Axis.RightThumbX, rotation);
    }

    private static void Handle_Mouse(short left, short right)
    {
        controller!.SetAxisValue(Xbox360Axis.LeftThumbX, right);
        controller!.SetAxisValue(Xbox360Axis.LeftThumbY, left);
    }

    private static void Click_Delay()
    {
        Thread.Sleep(CLICK_DELAY);
        controller!.SetButtonState(Xbox360Button.A, false);
    }

    public static void Reset_Controller()
    {
        controller!.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
        controller!.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
        controller!.SetAxisValue(Xbox360Axis.RightThumbX, 0);
        controller!.SetAxisValue(Xbox360Axis.RightThumbY, 0);
    }
    #endregion
}