#include <Arduino.h>
#include "Wire.h"
#include "Helper/Enums.h"

#define MPU6050_ADDR 0x68        // Alternatively set AD0 to HIGH  --> Address = 0x69
#define MPU6050_GYRO_CONFIG 0x1B ///< Gyro specfic configuration register
#define MPU6050_ACCEL_CONFIG 0x1C

#define MPU6050_ACCEL_XOUT_H 0x3B
#define MPU6050_GYRO_XOUT_H 0x43

mpu6050_gyr_range GYRO_MODE = MPU6050_GYR_RANGE_2000;
float DEGREE_STEPS = 131;

int16_t accX, accY, accZ, gyroX, gyroY, gyroZ, tRaw; // Raw register values (accelaration, gyroscope, temperature)
char result[7];

// int16 to string plus output format
char *toStr(int16_t i)
{
    sprintf(result, "%6d", i);
    return result;
}

int16_t AngleSpeed(int16_t value)
{
    return value / DEGREE_STEPS;
}

void writeRegister(uint16_t reg, byte value)
{
    Wire.beginTransmission(MPU6050_ADDR);
    Wire.write(reg);
    Wire.write(value);
    Wire.endTransmission(true);
}

void setGyrRange(mpu6050_gyr_range range)
{
    writeRegister(MPU6050_GYRO_CONFIG, range << 3);
}

void PrintGyro()
{
    Serial.print("GyX = ");
    Serial.print(toStr(AngleSpeed(gyroX)));
    Serial.print(" | GyY = ");
    Serial.print(toStr(AngleSpeed(gyroY)));
    Serial.print(" | GyZ = ");
    Serial.print(toStr(AngleSpeed(gyroZ)));
    Serial.println();
}

void Gyro_Setup()
{
    Serial.println("Gyro Setup");
    Wire.begin();
    setGyrRange(GYRO_MODE);
    DEGREE_STEPS = DegreeSteps(GYRO_MODE);
}

int16_t Gyro_Update()
{
    Wire.beginTransmission(MPU6050_ADDR);
    Wire.write(MPU6050_GYRO_XOUT_H);         // starting with register ACCEL_XOUT_H
    Wire.endTransmission(false);             // the parameter indicates that the Arduino will send a restart.
                                             // As a result, the connection is kept active.
    Wire.requestFrom(MPU6050_ADDR, 6, true); // request a total of 3*2=6 registers

    // "Wire.read()<<8 | Wire.read();" means two registers are read and stored in the same variable
    gyroX = Wire.read() << 8 | Wire.read(); // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)
    gyroY = Wire.read() << 8 | Wire.read(); // reading registers: 0x45 (GYRO_YOUT_H) and 0x46 (GYRO_YOUT_L)
    gyroZ = Wire.read() << 8 | Wire.read(); // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)

    return gyroZ;
}

// char *Gyro_Update2()
// {
//     Wire.beginTransmission(MPU6050_ADDR);
//     Wire.write(MPU6050_GYRO_XOUT_H);         // starting with register ACCEL_XOUT_H
//     Wire.endTransmission(false);             // the parameter indicates that the Arduino will send a restart.
//                                              // As a result, the connection is kept active.
//     Wire.requestFrom(MPU6050_ADDR, 6, true); // request a total of 3*2=6 registers

//     // "Wire.read()<<8 | Wire.read();" means two registers are read and stored in the same variable
//     gyroX = Wire.read() << 8 | Wire.read(); // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)
//     gyroY = Wire.read() << 8 | Wire.read(); // reading registers: 0x45 (GYRO_YOUT_H) and 0x46 (GYRO_YOUT_L)

//     // gyroZ = Wire.read() << 8 | Wire.read(); // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)

//     char hi = Wire.read();
//     char lo = Wire.read();
//     char values[2] = {hi, lo};
//     gyroY = hi << 8 | lo;

//     return values;
// }