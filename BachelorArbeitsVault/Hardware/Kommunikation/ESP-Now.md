# ESP-Now
___

|ESP32|---------------------\
											\
											  |---------------</ESP-Now/>-------------|ESP32|----------------</Serieller-Port/>---------------------|C#|
											/
|ESP32|---------------------/

___

[Wie implementiert man den serieln port?](https://wellsb.com/csharp/iot/control-arduino-csharp-serial-port)

Da die Daten beider Räder über einen seriellen Port kommen, muss in der Nachricht ein Verweis stehen, um welches Rad es sich handelt. 
Der erste char der Nachricht ist entweder 'L' oder 'R'.

___