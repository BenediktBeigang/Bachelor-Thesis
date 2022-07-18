#include <Arduino.h>
#include "Gyroscope/Gyro.h"
#include "Communication/Network.h"
#include "Communication/WebSocket.h"

unsigned long SendingTimer = 0;
const int16_t CALLS_PER_SECOND = 120;
int16_t TIME_BETWEEN_CALLS = 1000;

#define ESP_PORT 81

void setup()
{
  Serial.begin(115200);

  // Gyro
  Gyro_Setup();

  // Connection
  WiFi_Manager_Setup(ESP_PORT);
  WebSocket_Setup(ESP_PORT);
  Serial.print("Try to connect to Client");

  // // Timing
  TIME_BETWEEN_CALLS = (int)(1000 / CALLS_PER_SECOND);
  SendingTimer = millis();
}

void loop()
{
  webSocket.loop();
  if (!IsConnected)
  {
    ConnectToClient();
  }
  else if (millis() - SendingTimer > TIME_BETWEEN_CALLS)
  {
    Gyro_Update();
    SendGyroData('L', gyroZ);
    SendingTimer = millis();
  }
}
