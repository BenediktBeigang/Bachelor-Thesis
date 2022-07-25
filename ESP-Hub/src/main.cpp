#include <Arduino.h>
#include <esp_now.h>
#include <WiFi.h>

// Structure example to receive data
// Must match the sender structure
typedef struct struct_message
{
  uint8_t side;
  uint8_t hi;
  uint8_t lo;
} struct_message;

// Create a struct_message called myData
struct_message receivingData;
uint8_t sendingData[3];

void PrintReceived(int hi, int lo)
{
  Serial.print(hi);
  Serial.print('|');
  Serial.println(lo);
}

// callback function that will be executed when data is received
void OnDataRecv(const uint8_t *mac, const uint8_t *incomingData, int len)
{
  memcpy(&receivingData, incomingData, sizeof(receivingData));
  sendingData[0] = receivingData.side;
  sendingData[1] = receivingData.hi;
  sendingData[2] = receivingData.lo;
  Serial.write(sendingData, 3);

  // PrintReceived(receivingData.hi, receivingData.lo);
}

void setup()
{
  // Initialize Serial Monitor
  Serial.begin(115200);

  // Set device as a Wi-Fi Station
  WiFi.mode(WIFI_STA);

  // Init ESP-NOW
  if (esp_now_init() != ESP_OK)
  {
    Serial.println("Error initializing ESP-NOW");
    return;
  }

  // Once ESPNow is successfully Init, we will register for recv CB to
  // get recv packer info
  esp_now_register_recv_cb(OnDataRecv);
}

void loop()
{
}