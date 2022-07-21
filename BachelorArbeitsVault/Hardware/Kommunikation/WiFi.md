# WiFi
## Quellen
- [StackOverflow](https://stackoverflow.com/questions/53323463/wifi-communication-between-c-sharp-and-esp8266)
- [ESP_UDP](https://siytek.com/esp8266-udp-send-receive/)
- [C# udp](https://stackoverflow.com/questions/22852781/how-to-do-network-discovery-using-udp-broadcast)

## Vor und Nachteile
- Beide Seiten müssen die Adressen des Anderen kennen


## UDP vs TCP
- UDP hat weniger Last da keine Verbindungssicherheit herscht
- UDP garantiert nicht die Reihenfolge
	- Die ersten drei HEX-Zeichen könnten Reihenfolge sein. (262144 Zahlen bis wieder bei 0)

## Web-Sockets
HTTP ist zu langsam für den austausch von Echtzeitdaten. Bei klassischem HTTP wir eine TCP Verbindung aufgebaut bei der der Client für jede Anfrage den Server einzeln anfragen muss.
Web-Sockets sind eine Möglichkeit wesentlich schneller Daten zu übertragen da keine Anfrage mitgeschickt wird sondern nach einem Handshake der server automatisch die Daten sendet. Außerdem wird auf große Header verzichtet. Es werden nur die nötigsten Daten mitgesendet. Dabei können Datentypen wie strings, oder auch binarys verschickt werden.

- Server hört auf Port 81


## Verbindungsaufbau
| Software                                 |                                  | ESP                                                |
| ---------------------------------------- | -------------------------------- | -------------------------------------------------- |
| Warten auf Broadcast                     | <---"I am the Node ONE"---       | Broadcast senden                                   |
|                                          |                                  |                                                    |
|                                          |                                  |                                                    |
|                                          |                                  |                                                    |
| Node und Gyro anlegen                    |                                  |                                                    |
| Verbindung mit WebSockerServer anfragen  | -------------------------------> | Broadcast beenden </br>Verbindungsanfrage annehmen |
|                                          |                                  |                                                    |
|                                          |                                  |                                                    |
|                                          |                                  |                                                    |
| Gyro Calibrirung starten                 | <-------"1 Connected"--------    | Bestätigung sendern                                |
|                                          |                                  |                                                    |
|                                          |                                  |                                                    |
|                                          |                                  |                                                    |
| Daten empfangen</br>DatenCounter erhöhen | <---------"-136750"----------    | Daten senden                                       |
|                                          |                                  |                                                    |




