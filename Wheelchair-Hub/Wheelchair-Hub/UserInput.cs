public static class UserInput
{
    public static void Input(Connection connection)
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
                case 'g': Change_GyroMode(connection); break;
                default: GlobalData.LastMessages.Add($"Invalid Input!"); continue;
            }
        }
    }

    #region Commands
    private static void Flip_Nodes()
    {
        GlobalData.NodesFlipped = !GlobalData.NodesFlipped; GlobalData.LastMessages.Add("Nodes Flipped");
    }

    private static void Flip_NodeOne_WheelDirection()
    {
        GlobalData.Node_One.Gyro.RotationValueFlip = !GlobalData.Node_One.Gyro.RotationValueFlip; GlobalData.LastMessages.Add("Device ONE: Rotation Flipped!");
    }

    private static void Flip_NodeTwo_WheelDirection()
    {
        GlobalData.Node_Two.Gyro.RotationValueFlip = !GlobalData.Node_Two.Gyro.RotationValueFlip; GlobalData.LastMessages.Add("Device TWO: Rotation Flipped!");
    }

    private static void Request_Calibration()
    {
        if (GlobalData.Node_One.ConnectionType is not ConnectionType.NOTHING) GlobalData.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
        if (GlobalData.Node_Two.ConnectionType is not ConnectionType.NOTHING) GlobalData.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
    }
    #endregion

    #region Selection
    private static void Flip()
    {
        GlobalData.LastMessages.Add("Select Flip-Entity: ");
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case 'n': Flip_Nodes(); break;
                case '1': Flip_NodeOne_WheelDirection(); break;
                case '2': Flip_NodeTwo_WheelDirection(); break;
                default: GlobalData.LastMessages.Add($"Flip-Entity '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }

    private static void Change_GyroMode(Connection connection)
    {
        GlobalData.LastMessages.Add("Select new GyroMode: ");
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case '0': connection.Change_GyroMode(GyroMode.GYRO_250); break;
                case '1': connection.Change_GyroMode(GyroMode.GYRO_500); break;
                case '2': connection.Change_GyroMode(GyroMode.GYRO_1000); break;
                case '3': connection.Change_GyroMode(GyroMode.GYRO_2000); break;
                default: GlobalData.LastMessages.Add($"GyroMode '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }
    #endregion
}