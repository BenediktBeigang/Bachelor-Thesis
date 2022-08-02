using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

public class Controller
{
    /// <summary>
    /// Determines what rotation-speed is equivalent with the maximum controller-input
    /// </summary>
    private const int CLICK_DELAY = 200;
    private const int CONTROLLER_MAX = 32768;
    private readonly ViGEmClient client;
    private readonly IXbox360Controller controller;

    public Controller()
    {
        client = new ViGEmClient();
        controller = client.CreateXbox360Controller();
        controller.Connect();
        GlobalData.LastMessages.Add($"Controller connected!");
        Reset_Controller();
    }

    #region Controller API
    /// <summary>
    /// Uses the given values to trigger a controller-input.
    /// If the WheelchairMode is set to a WheelchairOutput v1 and v2 representing movement and rotation.
    /// If the WheelchairMode is set to Mouse v1 and v2 representing the Values for the y- and x-axis.
    /// </summary>
    /// <param name="values"></param>
    public void ValuesToController((double, double) values)
    {
        double GyroModeScalar = Gyro.GyroModeToStepsPerDegree(GlobalData.GyroMode);
        short v1 = (short)(values.Item1 * GyroModeScalar);
        short v2 = (short)(values.Item2 * GyroModeScalar);
        switch (GlobalData.WheelchairMode)
        {
            case WheelchairMode.Wheelchair_Realistic: Handle_Wheelchair(v1, v2); break;
            case WheelchairMode.Wheelchair_Simple: Handle_Wheelchair(v1, v2); break;
            case WheelchairMode.Mouse: Handle_Mouse(v1, v2); break;
        }
    }

    public void Click()
    {
        controller.SetButtonState(Xbox360Button.A, true);
        Task.Run(() => Click_Delay());
    }
    #endregion

    #region Handle Inputs
    private void Handle_Wheelchair(short move, short rotation)
    {
        controller.SetAxisValue(Xbox360Axis.LeftThumbY, move);
        controller.SetAxisValue(Xbox360Axis.RightThumbX, rotation);
    }

    private void Handle_Mouse(short left, short right)
    {
        controller.SetAxisValue(Xbox360Axis.LeftThumbX, right);
        controller.SetAxisValue(Xbox360Axis.LeftThumbY, left);
    }

    private void Click_Delay()
    {
        Thread.Sleep(CLICK_DELAY);
        controller.SetButtonState(Xbox360Button.A, false);
    }

    public void Reset_Controller()
    {
        controller.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
        controller.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
        controller.SetAxisValue(Xbox360Axis.RightThumbX, 0);
        controller.SetAxisValue(Xbox360Axis.RightThumbY, 0);
    }
    #endregion
}