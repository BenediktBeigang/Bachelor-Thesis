public enum GyroMode
{
    GYRO_250 = 0,
    GYRO_500 = 1,
    GYRO_1000 = 2,
    GYRO_2000 = 3
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

public enum MappingMode
{
    Wheelchair_Simple,
    Wheelchair_Realistic,
    GUI,
    Wheelchair_WithButtons
}

public enum MovementState
{
    StandingStill,
    ViewAxis_Motion,
    SingleWheel_Turn,
    DualWheel_Turn,
    Tilt
}

public enum ValueType
{
    Real,
    Playback
}