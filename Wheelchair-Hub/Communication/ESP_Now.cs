using System.IO.Ports;

public class ESP_Now : Connection
{
    public bool Listening { get; set; }

    private readonly SerialPort serialPort;
    private readonly string Com;
    private readonly int Baudrate;

    public ESP_Now(string com, int baudrate)
    {
        Com = com;
        Baudrate = baudrate;
        serialPort = new SerialPort(com, baudrate);
        serialPort.ReceivedBytesThreshold = 2; // Gets or sets the number of bytes in the internal input buffer before a DataReceived event occurs.
    }

    public override void ConnectToHost(GyroMode gyroMode)
    {
        InitializeNode(ConnectionType.ESP_NOW, DeviceNumber.ONE, gyroMode);
        InitializeNode(ConnectionType.ESP_NOW, DeviceNumber.TWO, gyroMode);
        serialPort.Open();
        GlobalData.LastMessages.Add($"Listening to Serial Port:\nCom: {Com}\nBaudrate: {Baudrate}");
    }

    private void ReceiveData()
    {
        Listening = true;
        while (Listening)
        {
            try
            {
                serialPort.DataReceived += HandleMessage;
            }
            catch (Exception)
            {
                GlobalData.LastMessages.Add("Could not read line from Serial-Port");
            }
        }
    }

    private void HandleMessage(object sender, SerialDataReceivedEventArgs e)
    {
        string data = ((SerialPort)sender).ReadExisting();
        char side = data.First();
        data = data.Substring(1);
        switch (side)
        {
            case 'L':
                GlobalData.Node_One.DataCount++;
                GlobalData.Node_One.Gyro.RawValue_Next(short.Parse(data));
                break;
            case 'R':
                GlobalData.Node_Two.DataCount++;
                GlobalData.Node_Two.Gyro.RawValue_Next(short.Parse(data));
                break;
        }

    }

    public override void Disconnect_AllNodes()
    {
        GlobalData.LastMessages.Add("Could not read line from Serial-Port");
        serialPort.Close();
    }

    protected override void Disconnect_Node(Node node)
    {
        serialPort.Close();
    }
}