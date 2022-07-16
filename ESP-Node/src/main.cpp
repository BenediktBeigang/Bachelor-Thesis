#include <Arduino.h>
#include "Gyroscope/Gyro.h"
#include "Communication/Network.h"
#include "Communication/WebSocket.h"

unsigned long SendingTimer = 0;
const int16_t CALLS_PER_SECOND = 60;
int16_t TIME_BETWEEN_CALLS = 1000;

#define ESP_PORT 81

void setup()
{
  Serial.begin(115200);

  // Connection
  WiFi_Manager_Setup(ESP_PORT);
  WebSocket_Setup(ESP_PORT);

  // Gyro
  Gyro_Setup();

  // Timing
  TIME_BETWEEN_CALLS = (int)(1000 / CALLS_PER_SECOND);
  SendingTimer = millis();

  Serial.print("Try to connect to Client");
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
    SendGyroData('L', Gyro_Update());
    SendingTimer = millis();
  }

  // Serial.println(toStr(Gyro_Update()));
  // delay(50);
}
