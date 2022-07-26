# ESP-Now
___

|[[ESP32]]|---------------------\
											 \
											  |>--------------</[[ESP-Now]]/>-------------|[[ESP32]]|----------------</[[Serieller Port]]/>---------------------|[[C#]]|
											 /
|[[ESP32]]|---------------------/

___

## Implementierung

Da die Daten beider Räder über einen seriellen Port kommen, muss in der Nachricht ein Verweis stehen, um welches Rad es sich handelt. 
Der erste char der Nachricht ist entweder '1' oder 'R'.

Die Verbindung zwischen den [[ESP32]] wird durch [[ESP-Now]] realisiert. Hier wird ein struct bei jedem Senden verschickt:

```c++
typedef struct struct_message
{
  uint8_t side;
  uint8_t hi;
  uint8_t lo;
} struct_message;
```

Eine Nachricht wie **49 2 111** ist wie folgt codiert worden: 

|      | DeviceNumber | High-Byte     | Low-Byte |
| ---- | ------------ | ------------- | -------- |
| dec  | 49           | 2             | 111      |
| char | '1'          | NOT PRINTABLE | 'o'      |

Nachdem die Nachricht über [[ESP-Now]] zum [[Node-Hub]] gesendet wurde, wird sie über den seriellen Port an [[C#]] gesendet.
- Baudrate: 115200
- COM: COM4

Danach wird, werden Hi- und Lo-Byte mit wieder zu einem short zusammen gesetzt.

___

## Trivia
- [[ESP-Now]] benötigt die [[MAC-Adresse]]. Konkret bedeutet das, dass die [[MAC-Adresse]] des [[Node-Hub]]s den restlichen [[Node]]s bekannt sein muss.

___

## Links

[Tutorial ESP-Now](https://randomnerdtutorials.com/esp-now-esp32-arduino-ide/)
[Tutorial Serial-Port](https://wellsb.com/csharp/iot/control-arduino-csharp-serial-port)

[C#](https://stackoverflow.com/questions/51684895/c-sharp-wpf-serial-port-reading-data-continuously)

___
