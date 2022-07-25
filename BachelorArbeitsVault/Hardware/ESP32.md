# [[ESP-32]] (Dev Kit C V4)
___
Chip der Firma AZ-Delivery

## IO Pins
![[ESP32_IO_Pins.PNG|700]]

___

## MAC-Adressen

| ESP   | Mac-Adresse       |
| ----- | ----------------- |
| ONE   | 30:C6:F7:30:53:1C |
| TWO   | 30:C6:F7:30:3C:FC |
| THREE | 30:C6:F7:30:29:C0 |

___

## Onboard Button

- EN: Reset des ESP
- BOOT: Nach Booten ist dieser der GPIO-0 Pin, der normal genutzt werden kann. Drückt man ihn geht er auf low.

___

## Treiber
Mein Chip von AZ-Delivery verwendet einen **CP2102** USB to UART Bridge Controller. Bevor man den Chip flashen kann, muss erst der Treiber auf dem Chip installiert werden.

### Links
[Tutorial](https://techexplorations.com/guides/esp32/begin/cp21xxx/)
[Download der Treiber](https://www.silabs.com/developers/usb-to-uart-bridge-vcp-drivers)
[Meine Bridge](https://www.silabs.com/interface/usb-bridges/classic/device.cp2102)

___

## [[I2C]]
- [Wie setzt man die SCL/SDA Pins](https://randomnerdtutorials.com/esp32-i2c-communication-arduino-ide/)
- [SCL/SDA erklärt](https://deepbluembedded.com/esp32-i2c-tutorial-change-pins-i2c-scanner-arduino/)

___
