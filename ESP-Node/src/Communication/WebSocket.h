#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <WebSocketsServer.h>
// #include <WiFiUdp.h>
#include <AsyncUDP.h>

#define USE_SERIAL Serial1
#define CLIENT_PORT 11000

// WiFiUDP UDP;
AsyncUDP UDP;
const IPAddress BROADCAST_ADDRESS = IPAddress(196, 168, 255, 255);
// const uint8_t BROADCAST_MESSAGE[] =
//     {'I', ' ', 'a', 'm', ' ', 't', 'h', 'e', ' ', 'l', 'e', 'f', 't', ' ', 'N', 'o', 'd', 'e'};
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

void PrintData()
{
    for (uint8_t i = 0; i < 8; i++)
    {
        int16_t v = data[i];
        Serial.print(toStr(v));
    }
    Serial.println();
}

void SendGyroData(char wheelside, int16_t gyroValue)
{
    // Serial.println("---SendData---");
    GetData(wheelside, gyroValue);
    // PrintData();

    // Serial.println(data);

    // char hi = gyroValue >> 8;
    // char lo = gyroValue & 0xFF;

    // char hi = '0';
    // char lo = '1';

    // const char data[4] = {wheelside, '0', '1', 0};
    // const char data[4] = {wheelside, hi, lo, 0};

    // Serial.print("Gyro: ");
    // Serial.println(data);

    webSocket.broadcastTXT(data);
    // Serial.print("--------------\n");
}

// void SendGyroData2(uint8_t wheelside, uint8_t hi, uint8_t lo)
// {
//     // Serial.print(wheelside);
//     // Serial.print(hi);
//     // Serial.println(lo);

//     // Serial.println("---SendData---");
//     // char data[3] = {wheelside, hi, lo};
//     // const uint8_t data[3] = {wheelside, hi, lo};

//     // Serial.print("Gyro: ");
//     // Serial.println(data);

//     webSocket.broadcastTXT("data");
//     Serial.print("--------------\n");
// }

void Broadcast()
{
    UDP.broadcastTo(BROADCAST_MESSAGE, CLIENT_PORT);
}

// void hexdump(const void *mem, uint32_t len, uint8_t cols = 16)
// {
//     const uint8_t *src = (const uint8_t *)mem;
//     USE_SERIAL.printf("\n[HEXDUMP] Address: 0x%08X len: 0x%X (%d)", (ptrdiff_t)src, len, len);
//     for (uint32_t i = 0; i < len; i++)
//     {
//         if (i % cols == 0)
//         {
//             USE_SERIAL.printf("\n[0x%08X] 0x%08X: ", (ptrdiff_t)src, i);
//         }
//         USE_SERIAL.printf("%02X ", *src);
//         src++;
//     }
//     USE_SERIAL.printf("\n");
// }

void ConnectToClient()
{
    // Serial.print("Try to connect to Client every ");
    // Serial.print((int16_t)(TIME_BETWEEN_CONNECTION_CALLS / 1000));
    // Serial.println(" seconds");
    // ConnectionTimer = millis();

    // UDP.connect(WiFi.localIP(), espPort);

    // Serial.println("---Broadcast---");
    // while (webSocket.connectedClients() == 0)
    // {
    //     Serial.print(".");
    //     Serial.print(webSocket.connectedClients());
    //     Serial.print(".");
    //     Broadcast();
    //     delay(TIME_BETWEEN_CONNECTION_CALLS);
    // }

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
        // case WStype_TEXT:
        //     USE_SERIAL.printf("[%u] get Text: %s\n", num, payload);

        //     // send message to client
        //     // webSocket.sendTXT(num, "message here");

        //     // send data to all connected clients
        //     // webSocket.broadcastTXT("message here");
        //     break;
        // case WStype_BIN:
        //     USE_SERIAL.printf("[%u] get binary length: %u\n", num, length);
        //     hexdump(payload, length);

        //     // send message to client
        //     webSocket.sendBIN(num, payload, length);
        //     break;
        // case WStype_ERROR:
        // case WStype_FRAGMENT_TEXT_START:
        // case WStype_FRAGMENT_BIN_START:
        // case WStype_FRAGMENT:
        // case WStype_FRAGMENT_FIN:
        //     break;
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