# 4. Entwicklung des eingebetteten Systems
___
```toc
```
Damit der Rollstuhl-Software bekannt ist, mit welcher Geschwindigkeit sich welches Rad in welche Richtung dreht, ist Hardware notwendig. Um dies zu bewerkstelligen wurde ein eingebettetes System entwicklet. Dieses muss die Rotationsdaten messen und an die Software übermitteln. Dabei kommt ein ESP32-Mikrocontroller zum Einsatz der ein Gyroskop ausliest und anschließend die Daten mithilfe eines Funkprotokolls an die Rollstuhl-Software überträgt. Näher soll in einem Vergleich beleuchtet werden, ob WiFi und ESP-Now das geeignetere Übertragungsprotokoll ist.

## 4.1 PlatformIO
Zur Entwicklung der eingebetteten Software, die auf den Mikrocontrollern läuft, wurde PlatformIO verwendet. Dies ist eine Framework-Erweiterung für Visual Studio Code, bei der die benötigten Bibliotheken, die für jeden Mikrocontroller und jedes Board notwendig sind, automatisch heruntergeladen und eingerichtet werden. Ebenfalls lassen sich über das UI, Bibliotheken, die für das jeweilige Projekt notwendig sind, hinzufügen. Zusätzlich zur Entwicklungsumgebung von Visual Studio Code gibt es Funktionalitäten einen Chip zu flashen und anschließend im seriellen Monitor die Ausführung zu beobachten. Im Gegensatz zu Umgebungen wie der Arduino IDE wird Zeit gespart, da dort zunächst manuell Treiber heruntergeladen werden müssen. Zusätzlich kann man nicht von den Vorteilen einer modernen IDE profitieren.

## 4.2 Messung der Rad-Daten
Um die Rotation der Räder des Rollstuhls messen zu können, wird ein Sensor benötigt. Dabei wurde sich für ein Gyroskop entschieden, da diese verfügbar, kostengünstig und leicht integrierbar sind. <mark>fußnote: warum keine lichtschranke</mark> Jedoch erfordert die Verwendung eines Gyroskops eine Voreinstellung und Kalibrierung, welche in diesem Unterkapitel erörtert werden.

### 4.2.1 Gyroskop
Im Zuge dieser Arbeit habe ich mich für das Motion-Tracking-Device GY-521 MPU-6050 entschieden. Dieses ist klein (mit Pins: 20mm x 15mm x 11mm), kostengünstig zu erwerben (ca. 4 Euro) und verfügt unter anderem über 3-Achsen-Gyroskop-Sensoren, mit denen die Rotation gemessen werden kann. Der Chip besitzt folgende 8 Anschlüsse: [[@GY5216AchsenGyroskop]]

| Anschluss | Funktion               | Notwendig für das vorliegende Experiment |
| --------- | ---------------------- | ---------------------------------------- |
| VCC       | Power-Supply           | Ja                                       |
| GND       | Ground                 | Ja                                       |
| SCL       | Serial-Clock           | Ja                                       |
| SDA       | Serial-Daten           | Ja                                       |
| XDA       | Auxiliary Serial Data  | Nein                                     |
| XCL       | Auxiliary Serial Clock | Nein                                     |
| ADO       | I2C Address Select     | Ja                                       |
| INT       | Interrupt              | Nein                                     |

Die Daten können mithilfe eines angeschlossenen Mikrocontrollers (ESP32) ausgelesen werden. Jede Achse wird auf zwei 8-Bit-Register abgebildet (S. 31)[[@MPU6000MPU6050Register2013]]. Zusammen ergibt das einen Wertebereich von 65.536 unterscheidbaren Zuständen. Mit der Drehrichtung rückwärts halbiert sich dieser Wertebereich, da ein Bit für das Verschieben des Wertebereichs ins Negative benötigt wird. Das Gyroskop des MPU-6050 kann in vier verschiedenen Konfigurationen betrieben werden (S. 31)[[@MPU6000MPU6050Register2013]]. Damit wird festgelegt, wie klein der Winkel zwischen zwei verschiedenen Zuständen ist; mit anderen Worten, wie viele Stufen pro Grad unterschieden werden können. Da der Wertebereich konstant ist, bedeutet eine empfindlichere Messung, dass das Gyroskop bei einer geringeren Geschwindigkeit das Ende des Wertebereichs erreicht. Angewendet auf den Rollstuhl hat das zur Folge, dass das rotierende Rad bei niedrigeren Geschwindigkeiten seine maximal messbare Geschwindigkeit erreicht.

Einstellbare Modi des Gyroskops mit ihren resultierenden Eigenschaften:

| Modus | Maximale Gradzahl pro Sekunde | Stufen pro Grad | Maximale Umdrehungszahl pro Sekunde | Maximale Radianten pro Sekunde | Zurückgelegte Distanz pro Stufe in mm* |
| ----- | ----------------------------- | --------------- | ----------------------------------- | ------------------------------ | -------------------------------------- |
| 0     | 250                           | 131             | 0,69                                | 4,36                           | 0,04                                   |
| 1     | 500                           | 65,5            | 1,39                                | 8,73                           | 0,08                                   |
| 2     | 1000                          | 32,8            | 2,78                                | 17,47                          | 0,16                                   |
| 3     | 2000                          | 16,4            | 5,56                                | 34,93                          | 0,32                                   |

\*Werte bei einem Raddurchmesser von 60 cm – (S. 31) [[@MPU6000MPU6050Register2013]] ergänzt um eigene Werte


|                                                 | Modus 0 | Modus 1 | Modus 2 | Modus 3 |
| ----------------------------------------------- | ------- | ------- | ------- | ------- |
| Maximale Gradzahl pro Sekunde                   | 250     | 500     | 1000    | 2000    |
| Stufen pro Grad                                 | 131     | 65,5    | 32,8    | 16,4    |
| Maximale Umdrehungszahl pro Sekunde             | 0,69    | 1,39    | 2,78    | 5,56    |
| Maximale Radianten pro Sekunde                  | 4,36    | 8,73    | 17,47   | 34,93   |
| Zurückgelegte Distanz pro Stufe in Millimetern* | 0,04    | 0,08    | 0,16    | 0,32    |



Es stellt sich die Frage, welcher der optimale Modus für das hier entwickelte System ist. Um dem Nutzer ein möglichst störungsfreies Erlebnis zu bieten, muss gewährleistet sein, dass das Gyroskop so empfindlich wie möglich eingestellt ist. Das bedeutet, dass der Wertebereich maximal ausgereizt werden muss. Ist der Modus nicht empfindlich genug, so bemerkt der Nutzer möglicherweise das Springen der Bitwerte in Form eines Vorspringens in der Bewegung. 
Allerdings muss ein Modus gewählt werden, welcher dazu führt, dass der Nutzer nicht schneller als die maximale Gradzahl pro Sekunde drehen kann, da es sonst zu einem Zahlenüberlauf kommt und zu einer fehlerhaften Weiterverarbeitung der Daten führt. Der Zahlenüberlauf kann zwar abgefangen werden, jedoch sollte bei Bedarf einer maximale Geschwindigkeit diese programmgesteurert festgelegt werden. Dies birgt den Vorteil den maximalen Wert flexibler setzen zu können. 
Der Modus muss also so empfindlich sein, dass der Nutzer nicht den Übergang von einem Zustand in den nächsten registriert. Gleichzeitig darf er nicht in der Lage sein, die Räder schneller als die maximale Gradzahl pro Sekunde zu drehen. Im Kapitel System-Analyse wird dieser Frage weiter nachgegangen.

### 4.2.2 Verbesserung der Rohdaten
Die ausgelesenen Werte des Gyroskops sind nicht automatisch kalibriert. Sie besitzen einen konstanten Offset. Deshalb muss beim Start des Systems eine Kalibrierungssequenz gestartet werden. Diese errechnet aus einer Reihe ausgelesener Werte einen Mittelwert, der anschließend von allen zukünftigen Werten abgezogen wird. Dazu dürfen die Räder nicht bewegt werden, da dies das Ergebnis der Kalibrierung unbrauchbar machen würde.

Darüber hinaus hat das Gyroskop-Signal ein Rauschen. Bei hoher Umdrehungszahl ist das Rauschen irrelevant, da es nur einen kleinen Anteil der Gesamtrotation ausmacht. Steht das Rad still, ist das Rauschen jedoch störend, da in diesem Fall nicht erkennbar ist, ob es tatsächlich stillsteht oder eine geringe Rotation gegeben ist. Aus diesem Grund wird ein Schwellenwert bestimmt, welcher der Gyroskop-Wert überschreiten muss, um als Bewegung erkannt werden zu können. Damit ist sichergestellt, dass es sich um eine tatsächliche Rotation handelt.

### 4.2.3 Idealer Gyroskop-Modus
Als Erstes wird der Frage nachgegangen, in welchem Modus das Gyroskop betrieben werden sollte. Hierfür wurde eine Datenreihe gemessen, mit der Gradzahl pro Sekunde im Verlauf der Zeit. Eine Testperson hat dabei versucht, ein Rad so schnell wie möglich zu drehen. 

![[gyroMax.PNG|700]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad, mit der dominanten Hand, so schnell wie möglich dreht)

Das Diagramm zeigt bei einer Roation nach vorne einen maximal erreichten Ausschlag um $800\ °/s$. Bei Rotationen nach hinten ist der Ausschlag etwas höher, übersteigt jedoch nicht $-900\ °/s$. Daraus folgt, dass Gyroskop-Modus 2 für dieses Szenario der Ideale ist. Bei diesem Modus ist die $Maximale\ Gradzahl\ pro\ Sekunde=1000\ °/s$. Der Nutzer erreicht nicht die maximal Geschwindigkeit und reizt trotzdem den Wertebereich ca. $80\ \%-90\ \%$ aus. Deshalb wird dieser Modus im System verwendet.

In vielen Anwendungen ist es von Vorteil oder <mark>angenehmer</mark> für den Nutzer, sich schnellstmöglich mit der maximalen Fortbewegungsgeschwindigkeit zu bewegen. Auf Dauer ist es ermüdend, die Räder möglichst schnell drehen zu müssen. Um den Nutzer zu entlasten, kann entweder der $Fortbewegungsvektor\ f$ immer auf einen maximalen Thumbstick-Ausschlag abgebildet werden oder es wird ein weiterer Schwellenwert eingeführt, ab dem alle Eingaben als maximaler Thumbstick-Ausschlag abgebildet werden.

## 4.3 ESP32
Um den MPU-6050 betreiben und dessen Daten an eine Software übermitteln zu können, wird ein Mikrocontroller-Board benötigt. Es muss die entsprechenden Register auslesen und mittels drahtloser Kommunikation versenden. Auf dem Markt ist eine große Anzahl von Produkten für die verschiedensten Anwendungsgebiete erhältlich. Im Rahmen dieser Arbeit wurde der Mikrocontroller ESP32 verwendet, das aktuelle Modell der Firma _Espressif_. Boards mit diesem Chip sind kostengünstig (ca. 8 Euro). Zudem ist der ESP32 mit WiFi (802.11 b/g/n), Bluetooth (v4.2) und _ESP-Now_ Unterstützung ausgestattet (S. 8-9) [[@ESP32Datasheet2022]]. Verbaut wurde ein Xtensa® 32-bit LX6 Mikroprozessor, mit 240MHz Taktfrequenz, 448 KB ROM und 520 KB SRAM. (S. 32) [[@ESP32Datasheet2022]]
Als Entwicklungsboard wurde das ESP32 Dev Kit C V4 verwendet.

Der MPU-6050 muss wie folgt an das Entwicklungsboard angeschlossen werden:

| ESP32            | MPU-6050 |
| ---------------- | -------- |
| 3.3V             | VCC      |
| GND              | GND      |
| GPIO_22 (I2C CL) | SDA      |
| GPIO_21 (I2C DA) | SCL      |
| ADO              | GND      |

## 4.4 3D gedruckte Box
Damit Entwicklungsboard, Gyroskop und Akku zusammengehalten werden, geschützt sind und am Rad befestigt werden können, wird eine Box benötigt, die alle Komponenten aufnehmen kann und diese trägt. Aufgrund dessen wurde – mithilfe von Blender – eine entsprechend seinen Anforderungen konstruierte Box entworfen und mittels 3D-Druckers gedrucht.

## 4.5 Vergleich zwischen WiFi und ESP-Now
Für die Übermittlung der Sensordaten an eine Software auf einem PC stehen, verschiedene Möglichkeiten zur Verfügung. In dieser Arbeit sind zwei verschiedene Protokolle getestet worden: WiFi und ESP-Now. Die Protokolle müssen dabei leicht in das System integrierbar sein. WiFi ist ein weit verbreiteter Standard, sodass entsprechende Bibliotheken schon existieren, um das Protokoll einbinden zu können [[@WiFiArduinoReference]]. ESP-Now ist weniger verbreitet, da aber der Chip vom selben Hersteller kommt, existieren auch hier schon Bibliotheken, beziehungsweise ist die benötigte Bibliothek schon im Entwicklungspaket des Chips schon enthalten [[@EspressifIoTDevelopment2022]]. Ein weiteres verfügbares Protokoll ist Bluetooth. Jenes muss jedoch aufgrund des zeitlichen Rahmens dieser Arbeit, an anderer Stelle beleuchtet werden.

### 4.5.1 WiFi und WebSockets
WiFi ist eine Kommunikationstechnologie, die durch die WiFi-Alliance entstanden ist und bis heute von ihr gepflegt wird [[@WhoWeAre]]. Sie ermöglicht drahtlose Kommunikation mit jedem Gerät, welches diese Technologie <mark>implementiert</mark>. Inzwischen ist WiFi ein weit verbreiteter Standard, welcher von den meisten mobilen Geräten unterstützt wird [[@DiscoverWiFiWiFi]]. Ein solches Gerät ist der ESP32.

Zunächst muss eine Verbindung zwischen dem ESP32 und dem lokalen Netzwerk mittels WiFi aufgebaut werden. Damit die Zugangsdaten nicht fest in den Code geschrieben werden müssen, wird die Bibliothek _WiFi Manager_ verwendet. Diese baut selbstständig eine Verbindung mit einem Netzwerk auf, nachdem die Zugangsdaten über ein Gerät wie zum Beispiel einem Smartphone übergeben wurden. Dazu wird ein Web-Konfigurations-Portal auf dem ESP32 gehostet, auf das ein <mark>nahes</mark> WiFi-fähiges Gerät zugreifen kann. [[@tzapuWiFiManager2022]]

Für die eigentliche Übertragung der Daten können verschiedene Protokolle verwendet werden. Ein klassischer Vertreter ist HTTP(S). Dieses wurde für die vorliegende Arbeit jedoch nicht verwendet, da das Protokoll auf Hypertext ausgelegt ist. Es wird für jede Abfrage von Daten eine neue TCP-Verbindung mit dem Server aufgebaut, der die Daten nach Eingang der Anfrage zurückschickt. Will der Client neue Daten empfangen, so muss dieser erneut eine TCP-Verbindung mit dem Server aufbauen. (S. 4)[[@ietfRFC6455WebSocket]]. Zusätzlich enthält jedes Paket, welches vom Clienten kommt, viel Overhead, da jede dieser Nachrichten einen HTTP-Header besitzt (S. 4)[[@ietfRFC6455WebSocket]]. Da es sich bei den Gyroskop-Daten jedoch um Echtzeitdaten handelt, wäre dieses Vorgehen ineffizient. Viel Zeit und Bandbreite würde für das Übertragen von nicht benötigte Daten verwendet werden.

Eine Alternative ist die Verwendung von einem WebSocket. Das Protokoll wurde entwickelt, um die Nachteile von HTTP(s) bei Echtzeitdaten zu umgehen und wird heute breit unterstützt. Das WebSocket-Protokoll wurde in seiner finalen Form 2011 von der Internet Engineering Task Force entwickelt und veröffentlicht [[@ietfRFC6455WebSocket]]. Dabei wird analog zu HTTP zu Beginn ein TCP Handshake durchgeführt. Der Client stellt an den Server eine Verbindungsanfrage, welcher dieser bestätigt. Ab diesem Zeitpunkt sendet der Server unaufgefordert die vom Client abonnierten Daten, bis die Verbindung vom Client beendet wird (S. 5)[[@ietfRFC6455WebSocket]]. Somit lassen sich höhere Datenraten erzielen, die für Echtzeitanwendungen notwendig sind. Das für die vorliegende Bachelor-Thesis entwickelte System setzt auf einen vom ESP32 gehosteten WebSocket-Server, der von der Software auf dem PC abonniert wird. Dabei kommt aufseiten des ESP die Bibliothek _arduinoWebSockets_ zum Einsatz, [[@markusWebSocketServerClient2022]] und auf der Client Seite die Bibliothek _websocket-client_ [[@kotasWebsocketclient2022]].

Zusätzlich zur eigentlichen Übertragung der Daten ist es notwendig, dass die Software auf dem PC den IP-Endpunkt des WebSockets auf dem ESP32 kennt. Dazu sendet der Mikrocontroller ebenfalls über WiFi einen UDP-Broadcast ins Netzwerk. Neben dem IP-Endpunkt werden auch Informationen über das Gerät mitgesendet, damit die Software auf dem PC weiß, um welches Gerät es sich handelt. Nach dieser Bekanntmachung kann der WebSocket abonniert und die Daten übertragen werden.

| Vorteile                                         | Nachteile                                           |
| ------------------------------------------------ | --------------------------------------------------- |
| Direkte Kommunikation der Geräte zur PC-Software | Verbindungsaufbau ist aufwändiger zu implementieren |
| Nur zwei Geräte werden benötigt                  |                                                     |
| Zugangsdaten müssen nicht fest-programmiert werden    |                                                     |

### 4.5.2 ESP-Now und Serieller Port
ESP-Now ist ein vom Unternehmen _Espressif_ selbst entwickeltes Übertragungsprotokoll, mit dem Mikrocontroller von _Espressif_ wie zum Beispiel der ESP8266 (Vorgänger des ESP32) und der ESP32 direkt miteinander Daten austauschen können. Dabei verwendet das Protokoll die MAC-Adressen zur Identifikation der Geräte. Es wird jedoch nur eine Verbindung in eine Richtung aufgebaut. Ein großer Vorteil dieses Protokolls ist die unkomplizierte Einbindung in das System. Anders als WiFi muss nicht zunächst eine Verbindung zu einem Netzwerk aufgebaut werden, sondern dem Gerät muss lediglich die MAC-Adresse des Zielgeräts vorliegen. Da die Kommunikation jedoch nur unter Mikrocontrollern stattfindet, muss das Gerät, welches die Sensor-Daten entgegennimmt, diese Daten mittels seriellen Ports an die Software auf dem PC übertragen. Damit steigt die Anzahl der Verbindungen, an denen die Übertragung scheitern kann. Jedoch erleichtert es die Verwendung für den Endbenutzer, da dieser kein WiFi-Netzwerk benötigt, um die Geräte mit der Software auf dem PC zu verbinden. Eine Verbindung per USB-Kabel ist ausreichend.

| Vorteile                                                            | Nachteile                                                                                           |
| ------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| Kein WiFi oder Bluetooth notwendig                                  | Anstelle einer Verbindung müssen zwei aufrechterhalten werden                                       |
| Das empfangende Gerät kann mit einem USB-Kabel angeschlossen werden | Ein zusätzliches Gerät wird benötigt, um die Daten zu empfangen und an die Software weiterzureichen |
|                                                                     | Die Ziel-MAC-Adressen müssen im Code fest-programmiert oder über andere Wege übertragen werden      |

### 4.5.3 Analyse der Messungen
<mark>specs hinzufügen (router etc.)</mark>
Damit der Nutzer eine präzise Eingabe tätigen kann, ist es notwendig, dass möglichst schnell und kontinuierlich neue Pakete empfangen werden. Um die Datenrate zu ermitteln, mit der beide Nodes (eingebettete Systeme an den Rädern) die Gyroskop-Werte verschicken, wird clientseitig alle 250ms die aktuelle Datenrate errechnet. Dazu zählt die Software seit der letzten Messung die eingegangenen Pakete und multipliziert diese mit 4, um die Datenrate pro Sekunde zu erhalten. Zusätzlich wird bei jedem Datensatz ermittelt, wie viel Zeit zwischen den letzten beiden Paketen vergangen ist. Auf die Messung der Latenz wird im Hinblick auf den Umfang der Arbeit verzichtet, da dies erfodert hätte beide Seiten der Verbindung zu synchronisieren.

![[wifiStats.PNG|600]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)
![[espnowStats.PNG|600]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)

Messungen der getesteten Verbindungsmethoden:

|                                   | Pakete pro Sekunde</br>Durchschnitt | Pakete pro Sekunde</br>Minimum | Pakete pro Sekunde</br>Maximum | Latenz in Millisekunden</br>Durchschnitt | Latenz in Millisekunden</br>Minimum | Latenz in Millisekunden</br>Maximum |
| --------------------------------- | ----------------------------------- | ------------------------------ | ------------------------------ | ---------------------------------------- | ----------------------------------- | ----------------------------------- |
| WiFi </br>(mit WebSocket)         | 250,16                              | 220                            | 284                            | 3,09                                     | < 1                                 | 26                                  |
| ESP-Now </br>(mit seriellem Port) | 330,43                              | 204                            | 440                            | 2,52                                     | < 1                                 | 30                                  |

|                                      | WiFi (mit WebSocket) | ESP-Now (mit seriellem Port) |
| ------------------------------------ | -------------------- | ---------------------------- |
| Pakete pro Sekunde Durchschnitt      | 250,16               | 330,43                       |
| Pakete pro Sekunde Minimum           | 220                  | 204                          |
| Pakete pro Sekunde Maximum           | 284                  | 440                          |
| Paketintervall in Millisekunden Durchschnitt | 3,09                 | 2,52                         |
| Paketintervall in Millisekunden Minimum      | <1                   | <1                           |
| Paketintervall in Millisekunden Maximum      | 26                   | 30                           |


Die Messungen haben ergeben, dass die Zeit zwischen zwei Paketen im Schnitt im niedrigen einstelligen Millisekundenbereich sind. _ESP-Now_ mit seriellem Port schafft dabei im Durchschnitt 80 Pakete mehr als WiFi mit einem WebSocket. Die Verbindung mit WiFi ist jedoch deutlich störungsfreier. So ist aus dem Diagramm <mark>2</mark> abzulesen, dass entweder _ESP-Now_ oder der serielle Port regelmäßiger und höhere Ausreißer erzeugt, bei denen die Zeit zwischen zwei Paketen über 15 Millisekunden ist. WiFi hingegen hat im kompletten Datensatz nur einen deutlichen Ausreißer und hat ansonsten selten Zeiten über 15 Millisekunden. Es kann geschlussfolgert werden, dass WiFi vorzuziehen ist. Jedoch kann auf _ESP-Now_ mit seriellem Port zurückgegriffen werden, wenn nur ein USB-Anschluss und kein WiFi Netzwerk verfügbar ist.
Anzumerken ist jedoch, dass die Messungen stark abhängig davon sind, welche USB-Anschlüsse und -Protokolle verwendet wurden, sowie welche Datenraten der Router unterstützt. Ein weiterer Einflussfaktor ist die Verbindung zwischen Client und dem Router. Sind diese kabellos verbunden, erhöht sich zusätzlich die Zeit, die ein Paket zur Übertragung benötigt, im Gegensatz zur kabelgebundener Übertragung. Deshalb sind die erhobenen Messwerte nur begrenzt aussagekräftig. Trotzdem lässt sich erkennen, dass beide Methoden genug Pakete verschicken können, um eine flüssige Bewegung nativ aus den Daten berechnen zu können. Es sind keine Interpolationstechniken notwendig, um die Bewegung flüssig erscheinen zu lassen.

Die Verbindungsmethoden im Vergleich:

 |                                   | Anzahl Mikrocontroller | Daten-Pfad                                             | "Hardcoding" von Verbindungsinformationen | Vorteile                                                                                                         |
 | --------------------------------- | ---------------------- | ------------------------------------------------------ | ----------------------------------------- | ---------------------------------------------------------------------------------------------------------------- |
 | WiFi </br>(mit WebSocket)         | 2                      | ESP32 -> Router -> Rollstuhl-Software                  | nicht notwendig                           | Stabilere Verbindung</br>Nur eine Übertragungstechnologie notwendig                                              |
 | ESP-Now </br>(mit seriellem Port) | 3                      | ESP32 -> ESP32 -> Serieller-Port -> Rollstuhl-Software | Ziel-MAC-Adressen                         | Kein Verbindungsaufbau mit lokalem Netzwerk notwendig</br>Kein WiFi-Netzwerk notwendig, ein USB-Anschluss genügt |

|                                           | WiFi (mit WebSocket)                                            | ESP-Now (mit seriellem Port)                                                                                 |
| ----------------------------------------- | --------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------ |
| Anzahl Mikrocontroller                    | 2                                                               | 3                                                                                                            |
| Daten-Pfad                                | ESP32 -> Router -> Rollstuhl-Software                           | ESP32 -> ESP32 -> Serieller-Port -> Rollstuhl-Software                                                       |
| "Hardcoding" von Verbindungsinformationen | nicht notwendig                                                 | Ziel-MAC-Adressen                                                                                            |
| Vorteile                                  | Stabilere Verbindung Nur eine Übertragungstechnologie notwendig | Kein Verbindungsaufbau mit lokalem Netzwerk notwendig Kein WiFi-Netzwerk notwendig, ein USB-Anschluss genügt |


