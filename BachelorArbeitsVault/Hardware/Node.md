# Node
Dieses Gerät soll die Rotationsdaten der Räder über Funk senden.
Zum Einsatz kommt hierbei ein [[ESP32]], welcher integriertes WLAN und Bluetooth kann, sowie einen eigenen Übertragungsstandard beherrscht, mit dem Namen [ESP-Now](https://randomnerdtutorials.com/esp-now-esp32-arduino-ide/).
Zum Messen der Rotation kommt ein [[#MPU6050]] zum Einsatz, der die Daten direkt an den [[#ESP32]] schickt.

![[Node.PNG|900]]

## Verbindungen
Verbindet man den [[Node]] über [[WiFi]] oder Bluetooth mit dem Rechner, so spart sich einen Chip. In diesem Szenario müsste die Software auf dem Rechner sich den Chips bekannt machen, sodass diese dann senden können. 

| Verbindungsmöglichkeiten | WLAN          | Bluetooth     | ESP-Now                 |
| ------------------------ | ------------- | ------------- | ----------------------- |
| Benötigte Chips          | 2             | 2             | 3                       |
| Benötigte Verbindungen   | 1             | 1             | 2                       |
| Störfaktoren             | Andere Geräte | Andere Geräte | Zwei Verbindungen nötig |
| Netzwerk-Zugang          | ✔             | ❌            | ❌                      |
| Bluetooth-Antenne        | ❌            | ✔             | ❌                      |

## Verkabelung
![[Verkabelung.PNG]]





