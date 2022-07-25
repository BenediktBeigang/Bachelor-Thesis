using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

public class Controller
{
    /// <summary>
    /// Determines what rotation-speed is equivalent with the maximum controller-input
    /// </summary>
    private const int MAX_ANGULAR_VELOCITY = 2000;
    private const int CLICK_DELAY = 200;
    private readonly ViGEmClient client;
    private readonly IXbox360Controller controller;

    public Controller()
    {
        client = new ViGEmClient();
        controller = client.CreateXbox360Controller();
        controller.Connect();
    }

    public void Move(short rawValue)
    {
        controller.SetAxisValue(Xbox360Axis.LeftThumbY, rawValue);
    }

    public void Rotate(short rawValue)
    {
        controller.SetAxisValue(Xbox360Axis.RightThumbX, rawValue);
    }

    public void Click(short rawValue)
    {
        controller.SetButtonState(Xbox360Button.A, true);
        Task.Run(() => Click_Delay());
    }

    private void Click_Delay()
    {
        Thread.Sleep(CLICK_DELAY);
        controller.SetButtonState(Xbox360Button.A, false);
    }
}