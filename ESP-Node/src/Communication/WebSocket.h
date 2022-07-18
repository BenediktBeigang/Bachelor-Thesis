#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <WebSocketsServer.h>
#include <AsyncUDP.h>

#define USE_SERIAL Serial1
#define CLIENT_PORT 11000

AsyncUDP UDP;
const IPAddress BROADCAST_ADDRESS = IPAddress(196, 168, 255, 255);
const char BROADCAST_MESSAGE[] = "I am the left Node";
unsigned long ConnectionTimer = millis();
int16_t TIME_BETWEEN_CONNECTION_CALLS = 5000;

bool IsConnected = false;
WebSocketsServer webSocket = WebSocketsServer(81);

char intConversion[7];
char data[8];

void Int16_To_CharArray(int16_t i)
{
    sprintf(intConversion, "%6d", i);
}

void GetData(char wheelside, int16_t gyroValue)
{
    Int16_To_CharArray(gyroValue);
    data[0] = wheelside;
    for (uint8_t i = 0; i < 7; i++)
    {
        data[i + 1] = intConversion[i];
    }
}

void SendGyroData(char wheelside, int16_t gyroValue)
{
    GetData(wheelside, gyroValue);
    webSocket.broadcastTXT(data);
}

void Broadcast()
{
    UDP.broadcastTo(BROADCAST_MESSAGE, CLIENT_PORT);
}

void ConnectToClient()
{
    if (millis() - ConnectionTimer > TIME_BETWEEN_CONNECTION_CALLS)
    {
        Serial.print(".");
        Broadcast();
        ConnectionTimer = millis();
    }
}

void WebSocketEvent(uint8_t num, WStype_t type, uint8_t *payload, size_t length)
{
    switch (type)
    {
    case WStype_DISCONNECTED:
        USE_SERIAL.printf("[%u] Disconnected!\n", num);
        break;
    case WStype_CONNECTED:
    {
        webSocket.sendTXT(num, "L Connected");
        IsConnected = true;
        Serial.println("\nConnection established");

        IPAddress ip = webSocket.remoteIP(num);
        Serial.print("Connected from: ");
        Serial.println(ip);
    }
    break;
    }
}

void WebSocket_Setup(uint16_t espPort)
{
    USE_SERIAL.begin(115200);
    USE_SERIAL.setDebugOutput(true);

    USE_SERIAL.print("\n\n\n");

    for (uint8_t t = 4; t > 0; t--)
    {
        USE_SERIAL.printf("[SETUP] BOOT WAIT %d...\n", t);
        USE_SERIAL.flush();
        delay(1000);
    }

    webSocket.begin();
    webSocket.onEvent(WebSocketEvent);

    UDP.connect(WiFi.localIP(), espPort);
}