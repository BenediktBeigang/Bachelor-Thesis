#include <esp_now.h>
#include <WiFi.h>

typedef struct struct_message
{
    uint8_t side;
    uint8_t hi;
    uint8_t lo;
} struct_message;

const uint8_t HUB_ADDRESS[] = {0x30, 0xC6, 0xF7, 0x30, 0x29, 0xC0};
struct_message data_espNow;
esp_now_peer_info_t peerInfo;
bool ESPNow_Connected = false;

// callback when data is sent
void OnDataSent(const uint8_t *mac_addr, esp_now_send_status_t status)
{
    // Serial.print("\r\nLast Packet Send Status:\t");
    // Serial.println(status == ESP_NOW_SEND_SUCCESS ? "Delivery Success" : "Delivery Fail");
}

void ESPNow_ConnectToClient()
{
    WiFi.mode(WIFI_STA);

    if (esp_now_init() != ESP_OK)
    {
        Serial.println("Error initializing ESP-NOW");
        ESPNow_Connected = false;
        return;
    }

    // Once ESPNow is successfully Init, we will register for Send CB to
    // get the status of Transmitted packet
    esp_now_register_send_cb(OnDataSent);

    // Register peer
    memcpy(peerInfo.peer_addr, HUB_ADDRESS, 6);
    peerInfo.channel = 0;
    peerInfo.encrypt = false;

    // Add peer
    if (esp_now_add_peer(&peerInfo) != ESP_OK)
    {
        Serial.println("Failed to add peer");
        ESPNow_Connected = false;
        return;
    }

    ESPNow_Connected = true;
    Serial.println("Connection established with Hub-Node.");
}

void ESPNow_SendGyroData(uint8_t gyroZ_Hi, uint8_t gyroZ_Lo, uint8_t device)
{
    data_espNow.side = device;
    data_espNow.hi = gyroZ_Hi; // High Byte
    data_espNow.lo = gyroZ_Lo; // Low Byte

    // Send message via ESP-NOW
    esp_err_t result = esp_now_send(HUB_ADDRESS, (uint8_t *)&data_espNow, sizeof(data_espNow));
}
