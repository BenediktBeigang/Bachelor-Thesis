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

    // /// <summary>
    // /// Uses the given values to trigger a controller-input.
    // /// If the WheelchairMode is set to a WheelchairOutput v1 and v2 representing movement and rotation.
    // /// If the WheelchairMode is set to Mouse v1 and v2 representing the Values for the y- and x-axis.
    // /// </summary>
    // /// <param name="values"></param>
    // private static void ValuesToController(ControllerInput input)
    // {
    //     switch (Mapping.Get_Mode())
    //     {
    //         case MappingMode.Wheelchair_Realistic: Handle_Wheelchair(input); break;
    //         case MappingMode.Wheelchair_Simple: Handle_Wheelchair(input); break;
    //         case MappingMode.Wheelchair_WithButtons: Handle_Wheelchair(input); break;
    //         case MappingMode.GUI: Handle_Mouse(input); break;
    //     }
    //     Handle_Buttons(input);
    // }

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

    #region Handle Inputs
    // private static void Handle_Wheelchair(ControllerInput input)
    // {
    //     controller!.SetAxisValue(Xbox360Axis.LeftThumbY, input.Value_One);
    //     controller!.SetAxisValue(Xbox360Axis.RightThumbX, input.Value_Two);
    // }

    // private static void Handle_Mouse(ControllerInput input)
    // {
    //     controller!.SetAxisValue(Xbox360Axis.LeftThumbY, input.Value_One);
    //     controller!.SetAxisValue(Xbox360Axis.LeftThumbX, input.Value_Two);
    // }

    // private static void Handle_Buttons(ControllerInput input)
    // {
    //     controller!.SetButtonState(Xbox360Button.A, input.RightPositive);
    //     controller!.SetButtonState(Xbox360Button.B, input.RightNegative);
    //     controller!.SetButtonState(Xbox360Button.X, input.LeftPositive);
    //     controller!.SetButtonState(Xbox360Button.Y, input.LeftNegative);
    //     // if (input.LeftNegative) Switch_MovementState();
    // }

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
    #endregion
}