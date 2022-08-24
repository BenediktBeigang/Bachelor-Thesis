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
                case 'm': Change_Mapping(); break;
                default: Terminal.Add_Command($"Invalid Input!"); continue;
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
        Terminal.Add_Command("Nodes Flipped");
    }

    private static void Flip_NodeOne_WheelDirection()
    {
        Node.Node_One.Gyro.RotationValueFlip = !Node.Node_One.Gyro.RotationValueFlip;
        Terminal.Add_Command("Device ONE: Rotation Flipped!");
    }

    private static void Flip_NodeTwo_WheelDirection()
    {
        Node.Node_Two.Gyro.RotationValueFlip = !Node.Node_Two.Gyro.RotationValueFlip;
        Terminal.Add_Command("Device TWO: Rotation Flipped!");
    }

    private static void Request_Calibration()
    {
        if (Node.Node_One.ConnectionType is not ConnectionType.NOTHING) Node.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
        if (Node.Node_Two.ConnectionType is not ConnectionType.NOTHING) Node.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
    }
    #endregion

    #region Selection
    public static void Change_Mapping()
    {
        MappingMode mode = MappingMode.GUI;

        Terminal.Add_Command("Select Mapping: ");
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case '1': mode = MappingMode.Wheelchair_Realistic; break;
                case '2': mode = MappingMode.Wheelchair_Simple; break;
                case '3': mode = MappingMode.Wheelchair_WithButtons; break;
                case '4': mode = MappingMode.GUI; break;
                default: Terminal.Add_Command($"Flip-Entity '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
        Terminal.Add_Command($"{mode}");
        if (Question("Change Mapping-Options?")) Enter_MappingOptions(mode);
        else Mapping._Mapping.Change_Mapping(mode);
    }

    private static void Enter_MappingOptions(MappingMode mode)
    {
        double wheelRadius = Enter_Number("Enter Wheel-Radius").Double;
        double chairWidth = Enter_Number("Enter Chair-Width").Double;
        int wheelMovementThreshold = Enter_Number("Enter Wheel-Movement-Threshold").Integer;
        int buttonPressingThreshold = Enter_Number("Enter Button-Pressing-Threshold").Integer;
        int wheelMovement_Max = Enter_Number("Enter Wheel-Movement-Max (0 if not needed)").Integer;

        Terminal.Add_Command($"New Mapping:\nMode->{mode}\nWheel-Radius->{wheelRadius}\nChair-Width->{chairWidth}\nWheelMovementThreshold->{wheelMovementThreshold}\nButtonPressingThreshold->{buttonPressingThreshold}\nWheelMovementMax->{wheelMovement_Max}");
        Mapping._Mapping.Change_Mapping(mode, wheelRadius, chairWidth, wheelMovementThreshold, buttonPressingThreshold, wheelMovement_Max);
    }

    private static void Flip()
    {
        Terminal.Add_Command("Select Flip-Entity: ");
        bool running = true;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case 'n': Flip_Nodes(); break;
                case '1': Flip_NodeOne_WheelDirection(); break;
                case '2': Flip_NodeTwo_WheelDirection(); break;
                default: Terminal.Add_Command($"Flip-Entity '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }

    private static void Change_Wheelchair()
    {
        Terminal.Add_Command("Enter Wheelchair Data: ");
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
                default: Terminal.Add_Command($"GyroMode '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }

    private static void Change_GyroMode()
    {
        Terminal.Add_Command("Select new GyroMode: ");
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
                default: Terminal.Add_Command($"GyroMode '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
    }

    private static (double Double, int Integer) Enter_Number(string message)
    {
        Terminal.Add_Command(message);
        bool running = true;
        string number = "";
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case '0': number += '0'; break;
                case '1': number += '1'; break;
                case '2': number += '2'; break;
                case '3': number += '3'; break;
                case '4': number += '4'; break;
                case '5': number += '5'; break;
                case '6': number += '6'; break;
                case '7': number += '7'; break;
                case '8': number += '8'; break;
                case '9': number += '9'; break;
                case '.': number += '.'; break;
                case (char)13: running = false; break;
                default: Terminal.Add_Command($"GyroMode '{k.KeyChar}' not valid!"); continue;
            }
        }
        Terminal.Add_Command(number);
        double Double = double.Parse(number);
        return (Double, (int)Double);
    }

    private static bool Question(string message)
    {
        Terminal.Add_Command(message);
        bool running = true;
        bool answer = false;
        while (running)
        {
            ConsoleKeyInfo k = Console.ReadKey();
            switch (k.KeyChar)
            {
                case 'y': answer = true; break;
                case 'Y': answer = true; break;
                case 'n': answer = false; break;
                case 'N': answer = false; break;
                default: Terminal.Add_Command($"GyroMode '{k.KeyChar}' not valid!"); continue;
            }
            running = false;
        }
        Terminal.Add_Command((answer) ? "Yes" : "No");
        return answer;
    }
    #endregion
}