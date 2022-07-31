public enum GyroMode
{
    GYRO_250,
    GYRO_500,
    GYRO_1000,
    GYRO_2000
}

public enum ConnectionType
{
    NOTHING,
    WIFI,
    ESP_NOW,
    BLUETOOTH
}

public enum DeviceNumber
{
    ONE,
    TWO
}

public enum CalibrationStatus
{
    NOT_CALIBRATED,
    REQUESTED,
    CALIBRATING,
    CALIBRATED
}

public enum WheelchairMode
{
    Wheelchair_Simple,
    Wheelchair_Realistic,
    Mouse
}