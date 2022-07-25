using System.IO.Ports;

public class ESP_Now : Connection
{
    private readonly SerialPort serialPort;
    private readonly string Com;
    private readonly int Baudrate;

    private bool FirstMessage_NodeOne = true; // true when the first message is not arrived yet.
    private bool FirstMessage_NodeTwo = true;

    public ESP_Now(string com, int baudrate)
    {
        Com = com;
        Baudrate = baudrate;
        serialPort = new SerialPort(com, baudrate);
        serialPort.ReceivedBytesThreshold = 3; // Gets or sets the number of bytes in the internal input buffer before a DataReceived event occurs.
    }

    public override void Connect_ToHost()
    {
        InitializeNode(ConnectionType.ESP_NOW, DeviceNumber.ONE);
        InitializeNode(ConnectionType.ESP_NOW, DeviceNumber.TWO);
        serialPort.DataReceived += Handle_Message;
        serialPort.Open();
        GlobalData.LastMessages.Add($"Listening to Serial Port:\nCom: {Com}\nBaudrate: {Baudrate}");
    }

    private void Handle_Message(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
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
                    GlobalData.Node_One.DataCount++;
                    GlobalData.Node_One.Gyro.RawValue_Next(value);
                    break;
                case (byte)'2':
                    GlobalData.Node_Two.DataCount++;
                    GlobalData.Node_Two.Gyro.RawValue_Next(value);
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
                    GlobalData.Node_One.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
                    FirstMessage_NodeOne = false;
                }
                break;
            case '2':
                if (FirstMessage_NodeTwo)
                {
                    GlobalData.Node_Two.Gyro.CalibrationStatus = CalibrationStatus.REQUESTED;
                    FirstMessage_NodeTwo = false;
                }
                break;
        }
    }

    #region Disconnect
    public override void Disconnect_AllNodes()
    {
        serialPort.Close();
        GlobalData.Reset_AllNodes();
        Connect_ToHost();
    }

    protected override void Disconnect_Node(Node node)
    {
        serialPort.Close();
        node.Reset();
        Connect_ToHost();
    }
    #endregion
}