#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <WebSocketsServer.h>
#include <AsyncUDP.h>
#include "Helper/Conversion.h"

#define USE_SERIAL Serial1
#define CLIENT_PORT 11000

AsyncUDP UDP;
const IPAddress BROADCAST_ADDRESS = IPAddress(196, 168, 255, 255);
const char BROADCAST_MESSAGE[] = "I am the left Node";
unsigned long ConnectionTimer = millis();
int16_t TIME_BETWEEN_CONNECTION_CALLS = 5000;

bool IsConnected = false;
WebSocketsServer webSocket = WebSocketsServer(81);
const int8_t DISCONNECT_MESSAGE[10] = {
    68, 73, 83, 67, 79, 78, 78, 69, 67, 84};

char intConversion[7];
char data[8];

char message[32];

void Int16_To_CharArray(int16_t i)
{
    sprintf(intConversion, "%6d", i);
}

void ClearMessage()
{
    for (int i = 0; i < 32; i++)
    {
        message[i] = 32;
    }
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
    Serial.println(data);
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

bool IsDisconnect(uint8_t *payload, size_t length)
{
    if (length != 10)
        return false;
    for (int i = 0; i < 10; i++)
    {
        if (payload[i] != DISCONNECT_MESSAGE[i])
            return false;
    }
    return true;
}

void WebSocketEvent(uint8_t num, WStype_t type, uint8_t *payload, size_t length)
{
    switch (type)
    {
    case WStype_DISCONNECTED:
        break;
    case WStype_CONNECTED:
    {
        webSocket.sendTXT(num, "L Connected");
        IsConnected = true;
        Serial.print("\nConnection established from: ");
        Serial.println(webSocket.remoteIP(num));
    }
    break;
    case WStype_TEXT:
        Serial.print("Client-Message: ");
        Int8ArrayToCharArray(message, payload, length);
        Serial.println(message);
        if (IsDisconnect(payload, length))
        {
            webSocket.disconnect(num);
            IsConnected = false;
            Serial.print("Disconnected: Try to reconnect to Client");
        }
        break;
    }
}

void WebSocket_Setup(uint16_t espPort)
{
    // USE_SERIAL.begin(115200);
    // USE_SERIAL.setDebugOutput(true);

    // USE_SERIAL.print("\n\n\n");

    // for (uint8_t t = 4; t > 0; t--)
    // {
    //     USE_SERIAL.printf("[SETUP] BOOT WAIT %d...\n", t);
    //     USE_SERIAL.flush();
    //     delay(1000);
    // }

    webSocket.begin();
    webSocket.onEvent(WebSocketEvent);

    UDP.connect(WiFi.localIP(), espPort);
}