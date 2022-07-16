typedef enum
{
    WiFi_Connection,
    EspNow_Connection,
    Bluetooth_Connection
} CommunicationType;

typedef enum
{
    MPU6050_ACC_RANGE_2G, // +/- 2g (default)
    MPU6050_ACC_RANGE_4G, // +/- 4g
    MPU6050_ACC_RANGE_8G, // +/- 8g
    MPU6050_ACC_RANGE_16G // +/- 16g
} mpu6050_acc_range;

typedef enum
{
    MPU6050_GYR_RANGE_250,  // +/- 250 deg/s (default)
    MPU6050_GYR_RANGE_500,  // +/- 500 deg/s
    MPU6050_GYR_RANGE_1000, // +/- 1000 deg/s
    MPU6050_GYR_RANGE_2000  // +/- 2000 deg/s
} mpu6050_gyr_range;

float DegreeSteps(mpu6050_gyr_range range)
{
    switch (range)
    {
    case MPU6050_GYR_RANGE_250:
        return 131;
    case MPU6050_GYR_RANGE_500:
        return 65.5;
    case MPU6050_GYR_RANGE_1000:
        return 32.8;
    case MPU6050_GYR_RANGE_2000:
        return 16, 4;
    default:
        return 131;
    }
}
