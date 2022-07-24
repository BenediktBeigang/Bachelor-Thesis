# WiFi
---

|ESP32|---------------------\
											\
											  |-------------</WiFi-Websocket/>--------------|C#|
											/
|ESP32|---------------------/

___
## Quellen
- [StackOverflow](https://stackoverflow.com/questions/53323463/wifi-communication-between-c-sharp-and-esp8266)
- [ESP_UDP](https://siytek.com/esp8266-udp-send-receive/)
- [C# udp](https://stackoverflow.com/questions/22852781/how-to-do-network-discovery-using-udp-broadcast)

___

## Vor- und Nachteile
- Beide Seiten müssen die Adressen des Anderen kennen

___

## [[UDP]] vs [[TCP]]
- [[UDP]] hat weniger Last da keine Verbindungssicherheit herrscht
- [[UDP]] garantiert nicht die Reihenfolge
	- Die ersten drei HEX-Zeichen könnten Reihenfolge sein. (262144 Zahlen bis wieder bei 0)

___

## [[Web-Sockets]]
HTTP ist zu langsam für den Austausch von Echtzeitdaten. Bei klassischem HTTP wird eine [[TCP]] Verbindung aufgebaut bei der der Client für jede Anfrage den Server einzeln anfragen muss.
[[Web-Sockets]] sind eine Möglichkeit wesentlich schneller Daten zu übertragen da keine Anfrage mitgeschickt wird sondern nach einem Handshake der Server automatisch die Daten sendet. Außerdem wird auf große Header verzichtet. Es werden nur die nötigsten Daten mitgesendet. Dabei können Datentypen wie strings, oder auch binarys verschickt werden.

### NuGet Pakete
- [Websocket.Client](https://www.nuget.org/packages/Websocket.Client)
- [WebSocket4Net](https://www.nuget.org/packages/WebSocket4Net)
- [WebSocketSharp](https://www.nuget.org/packages/WebSocketSharp/1.0.3-rc11)
- [esegece.sgcWebSockets](https://www.nuget.org/packages/esegece.sgcWebSockets)

### Trivia
- Server hört auf Port 81

___

## Verbindungsaufbau
| Software                                 |                                  | ESP                                                    |
| ---------------------------------------- | -------------------------------- | ------------------------------------------------------ |
| Warten auf [[Broadcast]]                 | <---"I am the Node ONE"---       | [[Broadcast]] senden                                   |
|                                          |                                  |                                                        |
|                                          |                                  |                                                        |
|                                          |                                  |                                                        |
| Node und [[Gyro]] anlegen                |                                  |                                                        |
| Verbindung mit [[WebSockerServer]] anfragen  | -------------------------------> | [[Broadcast]] beenden </br>Verbindungsanfrage annehmen |
|                                          |                                  |                                                        |
|                                          |                                  |                                                        |
|                                          |                                  |                                                        |
| [[Gyroskop]] Kalibrierung starten             | <-------"1 Connected"--------    | Bestätigung senden                                    |
|                                          |                                  |                                                        |
|                                          |                                  |                                                        |
|                                          |                                  |                                                        |
| Daten empfangen</br>DatenCounter erhöhen | <---------"-136750"----------    | Daten senden                                           |
|                                          |                                  |                                                        |

___



