#include <Arduino.h>

String ToHex(int16_t value)
{
    return String(value, HEX);
}

String ToHex_3Digits(int16_t value)
{
    String hex = ToHex(value);

    switch (hex.length())
    {
    case 1:
        return "  " + hex;
    case 2:
        return " " + hex;
    case 3:
        return hex;
    default:
        return "   ";
    }
}

String ToHex_5Digits(int16_t value)
{
    String hex = ToHex(value);

    switch (hex.length())
    {
    case 0:
        return "     ";
    case 1:
        return "    " + hex;
    case 2:
        return "   " + hex;
    case 3:
        return "  " + hex;
    case 4:
        return " " + hex;
    case 5:
        return hex;
    default:
        return "ovflw";
    }
}