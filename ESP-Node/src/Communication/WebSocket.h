#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <WebSocketsServer.h>
#include <AsyncUDP.h>
#include "Helper/Conversion.h"

#define USE_SERIAL Serial1
#define CLIENT_PORT 11000

AsyncUDP UDP;
const IPAddress BROADCAST_ADDRESS = IPAddress(196, 168, 255, 255);

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

bool IsConnected = false;
WebSocketsServer webSocket = WebSocketsServer(81);

char data[7];

char message[32];

void GetData(int16_t value)
{
    sprintf(data, "%6d", value);
}

void ClearMessage()
{
    for (int i = 0; i < 32; i++)
    {
        message[i] = 32;
    }
}

void SendGyroData(int16_t gyroValue)
{
    GetData(gyroValue);
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
    {
        webSocket.disconnect(num);
        IsConnected = false;
        Serial.print("Disconnected: Try to reconnect to Client");
    }
    break;
    case WStype_CONNECTED:
    {
        webSocket.sendTXT(num, CONNECTION_MESSAGE);
        IsConnected = true;
        Serial.print("\nConnection established from: ");
        Serial.println(webSocket.remoteIP(num));
    }
    break;
    case WStype_TEXT:
    {
        Serial.print("Client-Message: ");
        Int8ArrayToCharArray(message, payload, length);
        Serial.println(message);
    }
    break;
    }
}

void WebSocket_Setup(uint16_t espPort)
{
    webSocket.begin();
    webSocket.onEvent(WebSocketEvent);

    UDP.connect(WiFi.localIP(), espPort);
}