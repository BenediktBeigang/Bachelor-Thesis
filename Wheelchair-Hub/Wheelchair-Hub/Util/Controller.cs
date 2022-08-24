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
    private static bool SwitchButtonPressed;

    #region Controller API
    public static void Start()
    {
        SwitchButtonPressed = false;
        client = new ViGEmClient();
        controller = client.CreateXbox360Controller();
        controller.Connect();
        Terminal.Add_Message($"Controller connected!");
    }

    public static void Refresh_Controller(object sender, ElapsedEventArgs e)
    {
        if (controller is null) return;
        Rotations rotations = Node.Rotations();
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
    /// <summary>
    /// Switches between WheelchairWithButtons and GUI
    /// </summary>
    private static void Switch_MovementState()
    {
        SwitchButtonPressed = true;
        if (Mapping.Get_Mode() == MappingMode.GUI) Mapping._Mapping.Change_Mapping(MappingMode.Wheelchair_WithButtons);
        if (Mapping.Get_Mode() == MappingMode.Wheelchair_WithButtons) Mapping._Mapping.Change_Mapping(MappingMode.GUI);
        Task.Run(() => Click_Switch());
    }

    private static void Click_Switch()
    {
        Thread.Sleep(CLICK_DELAY);
        SwitchButtonPressed = false;
    }

    public static void Reset_Controller()
    {
        controller!.SetAxisValue(Xbox360Axis.LeftThumbX, 0);
        controller!.SetAxisValue(Xbox360Axis.LeftThumbY, 0);
        controller!.SetAxisValue(Xbox360Axis.RightThumbX, 0);
        controller!.SetAxisValue(Xbox360Axis.RightThumbY, 0);
    }
}