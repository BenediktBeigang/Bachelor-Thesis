using System.IO.Ports;

public class ESP_Now : Connection
{
    private SerialPort? serialPort;
    private readonly string Com;
    private readonly int Baudrate;

    private bool FirstMessage_NodeOne = true; // true when the first message is not arrived yet.
    private bool FirstMessage_NodeTwo = true;

    public ESP_Now(string com, int baudrate)
    {
        Com = com;
        Baudrate = baudrate;
        Connect_ToHost();
    }

    protected override void Connect_ToHost()
    {
        Task.Run(() => Open_SerialPort());
    }

    /// <summary>
    /// Opens Serial-Port so that messages can be read from it.
    /// </summary>
    private void Open_SerialPort()
    {
        bool first = true;
        while (true)
        {
            try
            {
                serialPort = new SerialPort(Com, Baudrate);
                serialPort.ReceivedBytesThreshold = 3; // Gets or sets the number of bytes in the internal input buffer before a DataReceived event occurs.
                serialPort.DataReceived += Handle_Message;
                Initialize_Node(ConnectionType.ESP_NOW, DeviceNumber.ONE);
                Initialize_Node(ConnectionType.ESP_NOW, DeviceNumber.TWO);
                serialPort.Open();
                Terminal.Add_Message($"Listening to Serial Port:\nCom: {Com}\nBaudrate: {Baudrate}");
                return;
            }
            catch (Exception)
            {
                if (first) Terminal.Add_Message($"Serial Port ({Com}) is not existing.");
                first = false;
                Thread.Sleep(1000);
            }
        }
    }

    /// <summary>
    /// Uses incoming message to extract the containing gyrovalues.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Handle_Message(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            if (serialPort is null) return;
            int bytes = serialPort.BytesToRead;
            byte[] data = new byte[bytes];

            if (!serialPort.IsOpen) return;
            serialPort.Read(data, 0, bytes);

            byte device = data[0];
            byte hi = data[1];
            byte lo = data[2];

            short value = (short)(hi << 8 | lo);
            switch (device)
            {
                case (byte)'1':
                    Node.Node_One.DataCount++;
                    Node.Node_One.Gyro.Push_RawValue(value, ValueSource.CONNECTION);
                    break;
                case (byte)'2':
                    Node.Node_Two.DataCount++;
                    Node.Node_Two.Gyro.Push_RawValue(value, ValueSource.CONNECTION);
                    break;
            }

            Check_FirstMessage((char)device);
        }
        catch (Exception) { }
    }

    private void Check_FirstMessage(char device)
    {
        switch (device)
        {
            case '1':
                if (FirstMessage_NodeOne)
                {
                    Node.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
                    FirstMessage_NodeOne = false;
                }
                break;
            case '2':
                if (FirstMessage_NodeTwo)
                {
                    Node.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
                    FirstMessage_NodeTwo = false;
                }
                break;
        }
    }

    #region Disconnect
    public override void Disconnect_AllNodes()
    {
        if (serialPort is null) return;
        serialPort.Close();
        Node.Reset_AllNodes();
        Connect_ToHost();
    }

    protected override void Disconnect_Node(Node node)
    {
        if (serialPort is null) return;
        serialPort.Close();
        node.Reset();
        Connect_ToHost();
    }

    public override void Change_GyroMode(GyroMode mode)
    {
        Terminal.Add_Message("Change GyroMode not implemented!");
    }
    #endregion
}