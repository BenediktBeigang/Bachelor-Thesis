#include <Arduino.h>

void StringToInt8Array(String data, u_int16_t size, uint8_t *intArray)
{
    // char charArray[size];
    // data.toCharArray(charArray, size);
    for (int i = 0; i < size; i++)
    {
        intArray[i] = (uint8_t)data.charAt(i);
    }
}