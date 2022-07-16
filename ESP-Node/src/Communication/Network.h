#include <WiFi.h>
#include <WiFiManager.h>

WiFiManager wifiManager;
const char *WIFI_MANAGER_NAME = "AutoConnectAP";
const char *WIFI_MANAGER_PASSWORD = "Benedikt";

void WiFi_Manager_Setup(uint16_t espPort)
{
    bool IsConnected = wifiManager.autoConnect(WIFI_MANAGER_NAME, WIFI_MANAGER_PASSWORD);

    if (!IsConnected)
    {
        Serial.println("Failed to connect to Network");
        // ESP.restart();
    }
    else
    {
        Serial.println("Connected to Network!");
    }
}