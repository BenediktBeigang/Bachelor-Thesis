#include <Arduino.h>
#include <Wire.h>
#include "Helper/Enums.h"

#define MPU6050_ADDR 0x68        // Alternatively set AD0 to HIGH  --> Address = 0x69
#define MPU6050_GYRO_CONFIG 0x1B ///< Gyro specfic configuration register
#define MPU6050_ACCEL_CONFIG 0x1C

#define MPU6050_ACCEL_XOUT_H 0x3B
#define MPU6050_GYRO_XOUT_H 0x43
#define MPU6050_GYRO_ZOUT_H 0x47

#define SDApin 21 // SDA
#define SCLpin 22 // SCL

float DEGREE_STEPS = 131;

int16_t accX, accY, accZ, gyroX, gyroY, gyroZ, tRaw; // Raw register values (accelaration, gyroscope, temperature)
char result[7];
uint8_t gyroZ_Hi, gyroZ_Lo;

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

void SleepModeOff()
{
    writeRegister(0x6B, 0);
}

void Gyro_Setup(mpu6050_gyr_range gyroMode)
{
    Serial.print("\n<<<Gyro Setup - Mode: ");
    Serial.println((int)gyroMode + ">>>\n");
    Wire.begin(SDApin, SCLpin);
    SleepModeOff();
    setGyrRange(gyroMode);
    DEGREE_STEPS = DegreeSteps(gyroMode);
}

void Gyro_Update()
{
    Wire.beginTransmission(MPU6050_ADDR);
    Wire.write(MPU6050_GYRO_XOUT_H);         // starting with register ACCEL_XOUT_H
    Wire.endTransmission(false);             // the parameter indicates that the Arduino will send a restart.
                                             // As a result, the connection is kept active.
    Wire.requestFrom(MPU6050_ADDR, 2, true); // request a total of 1*2=2 registers

    // "Wire.read()<<8 | Wire.read();" means two registers are read and stored in the same variable
    gyroX = Wire.read() << 8 | Wire.read(); // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)

    // ----------------------------------
    // Wire.requestFrom(MPU6050_ADDR, 6, true); // request a total of 3*2=6 registers

    // // "Wire.read()<<8 | Wire.read();" means two registers are read and stored in the same variable
    // gyroX = Wire.read() << 8 | Wire.read(); // reading registers: 0x43 (GYRO_XOUT_H) and 0x44 (GYRO_XOUT_L)
    // gyroY = Wire.read() << 8 | Wire.read(); // reading registers: 0x45 (GYRO_YOUT_H) and 0x46 (GYRO_YOUT_L)
    // // gyroZ = Wire.read() << 8 | Wire.read(); // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)

    // gyroZ_Hi = Wire.read();
    // gyroZ_Lo = Wire.read(); // reading registers: 0x47 (GYRO_ZOUT_H) and 0x48 (GYRO_ZOUT_L)
    // gyroZ = gyroZ_Lo | gyroZ_Hi << 8;
}

void Gyro_ChangeMode(char mode)
{
    Wire.end();
    switch (mode)
    {
    case '0':
        Gyro_Setup(MPU6050_GYR_RANGE_250);
        break;
    case '1':
        Gyro_Setup(MPU6050_GYR_RANGE_500);
        break;
    case '2':
        Gyro_Setup(MPU6050_GYR_RANGE_1000);
        break;
    case '3':
        Gyro_Setup(MPU6050_GYR_RANGE_2000);
        break;
    }
}