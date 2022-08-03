public class Loop
{
    public static List<Loop> Loops { get; set; } = new();

    private readonly int LOOP_DELAY;
    private readonly System.Timers.Timer Timer;

    public const int LOOP_DELAY_CONSOLE = 100;
    public const int LOOP_DELAY_CALIBRATION = 1000;
    public const int LOOP_DELAY_HEARTBEAT = 1000;
    public const int LOOP_DELAY_CONTROLLER = 16;

    public Loop(int loopDelay, System.Timers.ElapsedEventHandler function)
    {
        Timer = new System.Timers.Timer(loopDelay);
        Timer.Elapsed += function!;
        Timer.AutoReset = true;
        Timer.Enabled = true;
        Loops.Add(this);
    }

    public void Start()
    {
        Timer.Start();
    }

    public void Stop()
    {
        Timer.Stop();
    }

    public static void Close_AllLoops()
    {
        foreach (Loop l in Loops)
        {
            l.Stop();
        }
        Loops.Clear();
    }
}