#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <WebSocketsServer.h>
#include <AsyncUDP.h>
#include "Helper/Conversion.h"

#define USE_SERIAL Serial1
#define CLIENT_PORT 11000

AsyncUDP UDP;
const IPAddress BROADCAST_ADDRESS = IPAddress(196, 168, 255, 255);

const char GYROMODE_CHANGE_MESSAGE[] = "GyroMode";
const char GYROMODE_INDEX = 8;

const char BROADCAST_MESSAGE[] = "I am Node ONE";
const char CONNECTION_MESSAGE[] = "1 Connected";
const char HEARTBEAT_MESSAGE[] = "ONE Heartbeat";
const char NODE_NUMBER = '1';
// const char BROADCAST_MESSAGE[] = "I am Node TWO";
// const char CONNECTION_MESSAGE[] = "2 Connected";
// const char HEARTBEAT_MESSAGE[] = "TWO Heartbeat";
// const char NODE_NUMBER = '2';

unsigned long ConnectionTimer = millis();
int16_t TIME_BETWEEN_CONNECTION_CALLS = 5000;

bool WebSocket_Connected = false;
WebSocketsServer webSocket = WebSocketsServer(81);

char data_wifi[7];

char message[32];

void GetData(int16_t value)
{
    sprintf(data_wifi, "%6d", value);
}

void ClearMessage()
{
    for (int i = 0; i < 32; i++)
    {
        message[i] = 32;
    }
}

void WebSocket_SendGyroData(int16_t gyroValue)
{
    GetData(gyroValue);
    webSocket.broadcastTXT(data_wifi);
}

void Broadcast()
{
    UDP.broadcastTo(BROADCAST_MESSAGE, CLIENT_PORT);
}

void WebSocket_ConnectToClient()
{
    if (millis() - ConnectionTimer > TIME_BETWEEN_CONNECTION_CALLS)
    {
        Serial.print(".");
        Broadcast();
        ConnectionTimer = millis();
    }
}

bool Is_GyroChange()
{
    for (int i = 0; i < 8; i++)
    {
        if (message[i] != GYROMODE_CHANGE_MESSAGE[i])
            return false;
    }
    return true;
}

void HandleIncomingText()
{
    if (Is_GyroChange())
    {
        char gyroMode = message[GYROMODE_INDEX];
        Gyro_ChangeMode(gyroMode);
    }
}

void WebSocket_Event(uint8_t num, WStype_t type, uint8_t *payload, size_t length)
{
    switch (type)
    {
    case WStype_DISCONNECTED:
    {
        webSocket.disconnect(num);
        WebSocket_Connected = false;
        Serial.print("Disconnected: Try to reconnect to Client");
    }
    break;
    case WStype_CONNECTED:
    {
        webSocket.sendTXT(num, CONNECTION_MESSAGE);
        WebSocket_Connected = true;
        Serial.print("\nConnection established from: ");
        Serial.println(webSocket.remoteIP(num));
    }
    break;
    case WStype_TEXT:
    {
        Serial.print("Client-Message: ");
        Int8ArrayToCharArray(message, payload, length);
        Serial.println(message);
        HandleIncomingText();
    }
    break;
    }
}

void WebSocket_Setup(uint16_t espPort)
{
    webSocket.begin();
    webSocket.onEvent(WebSocket_Event);

    UDP.connect(WiFi.localIP(), espPort);
}