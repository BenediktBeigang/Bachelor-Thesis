public static class UserInput
{
    public static void Input()
    {
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case 'q': running = false; break; // programm stopps
                case 'c': Request_Calibration(); break; // gyros start calibration
                case 'f': Flip(); break;
                case 'g': Change_GyroMode(); break;
                case 'r': Switch_Record(); break;
                default: Terminal.Add_Message($"Invalid Input!"); continue;
            }
        }
    }

    #region Commands
    private static void Switch_Record()
    {
        Record.Switch_Record();
    }

    private static void Flip_Nodes()
    {
        Node.NodesFlipped = !Node.NodesFlipped;
        Terminal.Add_Message("Nodes Flipped");
    }

    private static void Flip_NodeOne_WheelDirection()
    {
        Node.Node_One.Gyro.RotationValueFlip = !Node.Node_One.Gyro.RotationValueFlip;
        Terminal.Add_Message("Device ONE: Rotation Flipped!");
    }

    private static void Flip_NodeTwo_WheelDirection()
    {
        Node.Node_Two.Gyro.RotationValueFlip = !Node.Node_Two.Gyro.RotationValueFlip;
        Terminal.Add_Message("Device TWO: Rotation Flipped!");
    }

    private static void Request_Calibration()
    {
        if (Node.Node_One.ConnectionType is not ConnectionType.NOTHING) Node.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
        if (Node.Node_Two.ConnectionType is not ConnectionType.NOTHING) Node.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
    }
    #endregion

    #region Selection
    private static void Flip()
    {
        Terminal.Add_Message("Select Flip-Entity: ");
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case 'n': Flip_Nodes(); break;
                case '1': Flip_NodeOne_WheelDirection(); break;
                case '2': Flip_NodeTwo_WheelDirection(); break;
                default: Terminal.Add_Message($"Flip-Entity '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }

    private static void Change_GyroMode()
    {
        Terminal.Add_Message("Select new GyroMode: ");
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case '0': Connection._Connection!.Change_GyroMode(GyroMode.GYRO_250); break;
                case '1': Connection._Connection!.Change_GyroMode(GyroMode.GYRO_500); break;
                case '2': Connection._Connection!.Change_GyroMode(GyroMode.GYRO_1000); break;
                case '3': Connection._Connection!.Change_GyroMode(GyroMode.GYRO_2000); break;
                default: Terminal.Add_Message($"GyroMode '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }
    #endregion
}