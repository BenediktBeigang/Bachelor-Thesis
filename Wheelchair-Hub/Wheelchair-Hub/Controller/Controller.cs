using System.Timers;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

public static class Controller
{
    private const int CLICK_DELAY = 500;
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
        (GyroSnapshot gyroOne, GyroSnapshot gyroTwo) snapshots = Gyro.Get_GyroSnapshots();
        Rotations rotations = new Rotations(snapshots.gyroOne, snapshots.gyroTwo, Node.NodesFlipped);

        if (Playback.Is_PlaybackRunning) Playback.Update_Gyro();
        if (Record.Is_Recording) Record.TakeSample(snapshots.gyroOne, snapshots.gyroTwo, Mapping._Mapping!.Get_MovementState(rotations));
        ControllerInput input = Mapping._Mapping!.Values_Next(rotations);

        Terminal.Other = input.ToString();
        ValuesToController(input);
    }
    #endregion

    private static void ValuesToController(ControllerInput input)
    {
        if (controller is not null)
        {
            controller.SetAxisValue(Xbox360Axis.LeftThumbX, input.LeftThumbX);
            controller.SetAxisValue(Xbox360Axis.LeftThumbY, input.LeftThumbY);
            controller.SetAxisValue(Xbox360Axis.RightThumbX, input.RightThumbX);
            controller.SetAxisValue(Xbox360Axis.RightThumbY, input.RightThumbY);
            controller.SetButtonState(Xbox360Button.A, input.A);
            controller.SetButtonState(Xbox360Button.B, input.B);
            controller.SetButtonState(Xbox360Button.X, input.X);
            controller.SetButtonState(Xbox360Button.Y, input.Y);
        }
    }

    public static void Clear_Controller()
    {
        controller!.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
        controller!.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
        controller!.SetAxisValue(Xbox360Axis.RightThumbX, 0);
        controller!.SetAxisValue(Xbox360Axis.RightThumbY, 0);
    }
}