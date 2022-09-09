# 5. Implementierung
___
```toc
```

In diesem Kapitel werden die Mittel und Wege erörtert, wie das zu entwickelnde System dieser Arbeit entworfen wurde. Dazu wird zunächst darauf eingegangen, wie das benötigte eingebettete System designt wurde. Anschließend wird sich der Frage gewidmet, wie die gemessenen Daten mithilfe einer Software, weiterverarbeitet beziehungsweise abgebildet werden, zu Eingaben auf einem Spielcontroller. 
<mark>Zuletzt wird untersucht, wie gut die entwickelten Systeme funktionieren und wie diese verbessert werden können.</mark>

## 5.1 Eingebettetes System zur Messung der Raddaten
Damit der Software bekannt ist, mit welcher Geschwindigkeit sich welches Rad in welche Richtung dreht, ist Hardware notwendig. Diese muss die Rotationsdaten messen und an die Software übermitteln. Um dies zu bewerkstelligen wurde ein eingebettetes System entwicklet. Es soll näher beleuchtet werden, welche Hardware verwendet wird und wie die Raddaten gemessen und übertragen werden. Dazu werden verschiedene Kommunikationsprotokolle verglichen. Im Folgenden wird das eingebettete System, welches die Daten <mark>an einen Clienten der Software</mark> überträgt, als Node bezeichnet. <mark>Node vielleicht ganz raus und unten wo es vorkommt anders formulieren?</mark>

### 5.1.1 Messtechniken und Sensoren
Um die Rotation der Räder des Rollstuhls messen zu können, wird ein Sensor benötigt. Dabei gibt es verschiedene Herangehensweisen, wie dieser die Rotation messen kann. 

**Lichtschranke**
Eine Möglichkeit, die Rotation eines Gegenstandes zu messen, ist mithilfe einer Lichtschranke. Hierbei wird durch die Rotation die Lichtschranke in regelmäßigen Abständen durch ein Hindernis blockiert. Über die Frequenz, in der der Lichtstrahl unterbrochen wird, kann anschließend eine Geschwindigkeit errechnet werden. Würde man eine solche Technik verwenden wollen, so müsste man jeweils einen Sensor neben den Rädern des Rollstuhls befestigen. Zusätzlich wäre ein Bauteil erforderlich, welche an den Rädern befestigt wird und dafür sorgt, dass die Lichtschranken unterbrochen werden, wenn sich ein Rad dreht. Beide Sensoren können anschließend von einem Mikrocontroller ausgelesen werden und mithilfe eines Kabels können die Daten an einen nahen Computer und damit an die empfangende Software verschickt werden. Jedoch ist die Empfindlichkeit abhängig vom Abstand der Hindernisse. Ist der Abstand der Hindernisse nicht klein genug, so können kleine Geschwindigkeiten nicht ausreichend gut gemessen werden. 

**Gyroskop**
<mark>Falsch gilt für acceleration nicht rotation</mark>
<mark>Eine Alternative ist die Verwendung eines Gyroskops. Dieses arbeitet mit einer Resonanzmasse die verschoben wird wenn sich das Gyroskop dreht. Die Bewegung der Masse kann dann in ein elektrisches Signal umgewandelt werden und von einem anderen Mikrocontroller ausgelesen werden. Dazu muss das Gyroskop jedoch am Rad selbst befestigt werden. Da das Rad in Bewegung ist, ist es notwendig, dass die Daten kabellos übertragen werden, was wiederum zur Folge hat, dass keine externe Stromversorgung möglich ist. Wird sich also für ein Gyroskop entschieden wird zusätzlich für jedes Gyroskop ein eigener auslesender Mikrocontroller benötigt. Dieser muss in der Lage sein kabellos Daten zu versenden und seinen Strom aus einem Akku oder einer Batterie erhalten. Alle Komponenten müssen dann in einem Behälter zusammengehalten werden. Dafür können auch kleine Geschwindigkeiten gemessen werden.</mark>

Es wurde sich für die Verwendung eines Gyroskops entschieden, da damit kleinere Geschwindigkeiten gemessen werden können und dies wichtig für das Erlebnis des Nutzers ist. Im schlechtesten Fall würde der Nutzer sonst stotternde Bewegungen bemerken oder fehlerhafte Eingaben tätigen.

### 5.1.2 GY-521 MPU-6050
Im Zuge dieser Arbeit habe ich mich für das Motion-Tracking-Device GY-521 MPU-6050 entschieden. 
Dieses ist klein (mit Pins: 20mm x 15mm x 11mm), kostengünstig zu erwerben (ca. 4 Euro) und verfügt unter anderem über 3-Achsen-Gyroskop-Sensoren, mit denen die Rotation gemessen werden kann. 
Der Chip besitzt folgende 8 Anschlüsse: [[@GY5216AchsenGyroskop]]

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

Es stellt sich die Frage, welcher der optimale Modus für das hier entwickelte System ist. Um dem Nutzer ein möglichst störungsfreies Erlebnis zu bieten, muss gewährleistet sein, dass das Gyroskop so empfindlich wie möglich eingestellt ist. Das bedeutet, dass der Wertebereich maximal ausgereizt werden muss. Ist der Modus nicht empfindlich genug, so bemerkt der Nutzer möglicherweise das Springen der Bitwerte in Form eines Vorspringens in der Bewegung. 
Allerdings muss ein Modus gewählt werden, welcher dazu führt, dass der Nutzer nicht schneller als die maximale Gradzahl pro Sekunde drehen kann, da es sonst zu einem Zahlenüberlauf kommt und zu einer fehlerhaften Weiterverarbeitung der Daten führt. Der Zahlenüberlauf kann zwar abgefangen werden, jedoch sollte bei Bedarf einer maximale Geschwindigkeit diese programmgesteurert festgelegt werden. Dies birgt den Vorteil den maximalen Wert flexibler setzen zu können. 
Der Modus muss also so empfindlich sein, dass der Nutzer nicht den Übergang von einem Zustand in den nächsten registriert. Gleichzeitig darf er nicht in der Lage sein, die Räder schneller als die maximale Gradzahl pro Sekunde zu drehen. Im Kapitel System-Analyse wird dieser Frage weiter nachgegangen.

Die ausgelesenen Werte des Gyroskops sind nicht automatisch kalibriert. Sie besitzen einen konstanten Offset. Deshalb muss beim Start des Systems eine Kalibrierungssequenz gestartet werden. Diese errechnet aus einer Reihe ausgelesener Werte einen Mittelwert, der anschließend von allen zukünftigen Werten abgezogen wird. Dazu dürfen die Räder nicht bewegt werden, da dies das Ergebnis der Kalibrierung unbrauchbar machen würde.

Darüber hinaus hat das Gyroskop-Signal ein Rauschen. Bei hoher Umdrehungszahl ist das Rauschen irrelevant, da es nur einen kleinen Anteil der Gesamtrotation ausmacht. Steht das Rad still, ist das Rauschen jedoch störend, da in diesem Fall nicht erkennbar ist, ob es tatsächlich stillsteht oder eine geringe Rotation gegeben ist. Aus diesem Grund wird ein Schwellenwert bestimmt, welcher der Gyroskop-Wert überschreiten muss, um als Bewegung erkannt werden zu können. Damit ist sichergestellt, dass es sich um eine tatsächliche Rotation handelt.

### 5.1.3 ESP32
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

<mark> Eventuell schmeiße ich 5.1.4 und 5.1.5 raus</mark>
### 5.1.4 PlatformIO
Zur Entwicklung der Software, die auf den Mikrocontrollern läuft, wurde PlatformIO verwendet. Dies ist eine Erweiterung für Visual Studio Code, bei der die benötigten Bibliotheken, die für jeden Mikrocontroller und jedes Board notwendig sind, automatisch heruntergeladen und eingerichtet werden. Ebenfalls lassen sich über das UI Bibliotheken, die für das jeweilige Projekt notwendig sind, hinzufügen. Zusätzlich zur Entwicklungsumgebung von Visual Studio Code gibt es Funktionalitäten einen Chip zu flashen und anschließend im seriellen Monitor die Ausführung zu beobachten. Im Gegensatz zu Umgebungen wie der Arduino IDE wird Zeit gespart, da dort zunächst manuell Treiber heruntergeladen werden müssen. Man kann nicht von den Vorteilen einer modernen IDE profitieren.

### 5.1.5 3D gedruckte Box
Damit Entwicklungsboard, Gyroskop und Akku zusammengehalten werden, geschützt sind und am Rad befestigt werden können, wird eine Box benötigt, die alle Komponenten aufnehmen kann und diese trägt. Aufgrund dessen habe ich für meine Arbeit – mithilfe von Blender – eine entsprechend seinen Anforderungen konstruierte Box entworfen, welche ich mithilfe eines 3D-Druckers drucken ließ.

## 5.2 Transfer der Gyroskop-Daten
Für die Übermittlung der Sensordaten an eine Software auf einem PC stehen, verschiedene Möglichkeiten zur Verfügung. In dieser Arbeit sind zwei verschiedene Protokolle getestet worden: WiFi und ESP-Now. Die Protokolle müssen dabei leicht in das System integrierbar sein. WiFi ist ein weit verbreiteter Standard, sodass entsprechende Bibliotheken schon existieren, um das Protokoll einfach einbinden zu können [[@WiFiArduinoReference]]. ESP-Now ist weniger verbreitet, da aber der Chip vom selben Hersteller kommt, existieren auch hier schon Bibliotheken, beziehungsweise ist die benötigte Bibliothek schon im Entwicklungspaket des Chips schon enthalten [[@EspressifIoTDevelopment2022]]. Ein weiteres verfügbares Protokoll ist Bluetooth. Jenes ist jedoch aufgrund des zeitlichen Rahmens dieser Arbeit nicht getestet worden.

### 5.2.1 WiFi und WebSockets
WiFi ist eine Kommunikationstechnologie, die durch die WiFi-Alliance entstanden ist und bis heute von ihr gepflegt wird [[@WhoWeAre]]. Sie ermöglicht drahtlose Kommunikation mit jedem Gerät, das diese Technologie implementiert. Inzwischen ist WiFi ein weit verbreiteter Standard, welcher von den meisten mobilen Geräten unterstützt wird [[@DiscoverWiFiWiFi]]. Ein solches Gerät ist der ESP32.

Zunächst muss eine Verbindung zwischen dem ESP32 und dem lokalen Netzwerk mittels WiFi aufgebaut werden. Damit die Zugangsdaten nicht fest in den Code geschrieben werden müssen, wird die Bibliothek _WiFi Manager_ verwendet. Diese baut selbstständig eine Verbindung mit einem Netzwerk auf, nachdem die Zugangsdaten über ein Gerät wie zum Beispiel einem Smartphone übergeben wurden. Dazu wird ein Web-Konfigurations-Portal auf dem ESP32 gehostet, auf das ein nahes Gerät zugreifen kann. [[@tzapuWiFiManager2022]]

Für die eigentliche Übertragung der Daten können verschiedene Protokolle verwendet werden. Ein klassischer Vertreter ist HTTP(S). Jedoch ist das Protokoll auf Hypertext ausgelegt. Es wird für jede Abfrage von Daten eine neue TCP-Verbindung mit dem Server aufgebaut, der die Daten nach Eingang der Anfrage zurückschickt. Will der Client neue Daten empfangen, so muss dieser erneut eine TCP-Verbindung mit dem Server aufbauen. (S. 4)[[@ietfRFC6455WebSocket]]. Zusätzlich enthält jedes Paket, welches vom Clienten kommt, viel Overhead, da jede dieser Nachrichten einen HTTP-Header besitzt (S. 4)[[@ietfRFC6455WebSocket]]. Da es sich bei den Gyroskop-Daten jedoch um Echtzeitdaten handelt, wäre dieses Vorgehen ineffizient. Viel Zeit würde für das Übertragen von nicht benötigte Daten verwendet werden.

Eine Alternative ist die Verwendung eines WebSockets. Das Protokoll wurde entwickelt, um das oben geschilderte Problem zu lösen und wird heute breit unterstützt. Das WebSocket-Protokoll wurde in seiner finalen Form 2011 von der Internet Engineering Task Force entwickelt und veröffentlicht [[@ietfRFC6455WebSocket]]. Dabei wird analog zu HTTP am Anfang ein TCP Handshake durchgeführt. Der Client stellt an den Server eine Verbindungsanfrage, welcher dieser bestätigt. Ab diesem Zeitpunkt sendet der Server unaufgefordert die vom Clienten abonnierten Daten, bis die Verbindung vom Clienten beendet wird (S. 5)[[@ietfRFC6455WebSocket]]. Somit lassen sich höhere Datenraten erzielen, die für Echtzeitanwendungen notwendig sind. Das für die vorliegende Bachelor-Thesis entwickelte System setzt auf einen vom ESP32 gehosteten WebSocket-Server, der von der Software auf dem PC abonniert wird. Dabei kommt aufseiten des ESP die Bibliothek _arduinoWebSockets_ zum Einsatz, [[@markusWebSocketServerClient2022]] und auf der Client Seite die Bibliothek _websocket-client_ [[@kotasWebsocketclient2022]].

Zusätzlich zur eigentlichen Übertragung der Daten ist es notwendig, dass die Software auf dem PC den IP-Endpunkt des WebSockets auf dem ESP32 kennt. Dazu sendet der Mikrocontroller ebenfalls über WiFi einen UDP-Broadcast ins Netzwerk. Neben dem IP-Endpunkt werden auch Informationen über das Gerät mitgesendet, damit die Software auf dem PC weiß, um welches Gerät es sich handelt. Nach dieser Bekanntmachung kann der WebSocket abonniert und die Daten übertragen werden.

| Vorteile                                         | Nachteile                                           |
| ------------------------------------------------ | --------------------------------------------------- |
| Direkte Kommunikation der Geräte zur PC-Software | Verbindungsaufbau ist aufwändiger zu implementieren |
| Nur zwei Geräte werden benötigt                  |                                                     |
| Zugangsdaten müssen nicht fest-programmiert werden    |                                                     |

### 5.2.2 ESP-Now und Serieller Port
ESP-Now ist ein vom Unternehmen _Espressif_ selbst entwickeltes Übertragungsprotokoll, mit dem Mikrocontroller von _Espressif_ wie zum Beispiel der ESP8266 (Vorgänger des ESP32) und der ESP32 direkt miteinander Daten austauschen können. Dabei verwendet das Protokoll die MAC-Adressen zur Identifikation der Geräte. Es wird jedoch nur eine Verbindung in eine Richtung aufgebaut. Ein großer Vorteil dieses Protokolls ist die einfache Einbindung in das System. Anders als WiFi muss nicht zunächst eine Verbindung zu einem Netzwerk aufgebaut werden, sondern dem Gerät muss lediglich die MAC-Adresse des Zielgeräts vorliegen. Da die Kommunikation jedoch nur unter Mikrocontrollern stattfindet, muss das Gerät, welches die Sensor-Daten entgegennimmt, diese Daten mittels seriellen Ports an die Software auf dem PC übertragen. Damit steigt die Anzahl der Verbindungen, an denen die Übertragung scheitern kann. Jedoch erleichtert es die Verwendung für den Endbenutzer, da dieser kein WiFi-Netzwerk benötigt, um die Geräte mit der Software auf dem PC zu verbinden. Eine Verbindung per USB-Kabel ist ausreichend.

| Vorteile                                                            | Nachteile                                                                                           |
| ------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------- |
| Kein WiFi oder Bluetooth notwendig                                  | Anstelle einer Verbindung müssen zwei aufrechterhalten werden                                       |
| Das empfangende Gerät kann mit einem USB-Kabel angeschlossen werden | Ein zusätzliches Gerät wird benötigt, um die Daten zu empfangen und an die Software weiterzureichen |
|                                                                     | Die Ziel-MAC-Adressen müssen im Code fest-programmiert oder über andere Wege übertragen werden      |

## 5.3 Interface zur Nutzung in externer Software
Die Raddaten, welche die Rollstuhl-Software empfangen hat, müssen nun zu externer Software gelangen. Dazu ist eine vorhandene Schnittstelle notwendig (Tastatur, Maus, Spielcontroller, ...), denn es kann nicht davon ausgegangen werden, dass jede externe Software eine neue Schnittstelle implementiert. Jedoch muss in diesem Fall, in der Rollstuhl-Software eine Abbildung auf eine vorhandene Schnittstelle durchgeführt werden.

### 5.3.1 Tastatur oder Spielcontroller
Tastaturen und Spielcontroller werden von den meisten Anwendungen unterstützt und eignen sich unterschiedlich gut für die Zwecke des hier entwickelten Systems. Tastatureingaben bieten den Vorteil, dass sie von fast jeder erdenklichen Software unterstützt werden. Jedoch lassen nur binäre Eingaben, durch das Drücken von Tasten tätigen. Da die Rollstuhl-Eingaben jedoch unterschiedliche Werte innerhalb eines Wertebereichs darstellen, wird bei einer Tastatureingabe die Interaktionsmöglichkeit stark eingeschränkt. Zudem ist oft das zusätzliche Verwenden einer Maus erforderlich. Dies verkompliziert das Abbilden zusätzlich. 

Die Alternative ist ein Mapping auf eine Spielcontroller-Eingabe. Hier gibt es ebenfalls Knöpfe, beziehungsweise binäre Eingaben, aber auch Eingaben entlang von Achsen innerhalb eines Wertebereichs. Überwiegend werden die Achsen in Form eines Thumb-Sticks oder Knopfes mit mehreren Stufen realisiert. Auf der einen Seite könnten so Rollstuhl-Eingaben, wie das Fortbewegen, einfacher auf das Gerät abgebildet werden. Andererseites unterstützt nicht jede Software Eingaben mittels eines Spielcontrollers. 

Es wurde sich im Rahmen dieser Arbeit für das Abbilden auf, und emulieren von, einem Spielcontroller entschieden. Grund dafür ist, dass die meiste Software in der Fortbewegung eine Rolle spielt (meist Computerspiele oder andere 3D-Räume), diese unterstützen.

### 5.3.2 Virtual Gamepad Emulation Framework
Um die Eingaben des Rollstuhls in tatsächliche Spielcontroller-Eingaben umzuwandeln, die vom Betriebssystem auch als Controller-Eingabe verstanden werden, ist eine Emulation eines Controllers notwendig. Ziel ist es, programmgesteuert Controller-Eingaben an den Rechner zu senden. Um sich den Aufwand des Schreibens eines neuen Treibers zu ersparen, wird an dieser Stelle auf das _Virtual Gamepad Emulation Framework_ zurückgegriffen. Dies ist eine Bibliothek, welche in bestehende Software integriert werden kann und einen virtuellen Controller mit dem Rechner verbindet. Über Befehle lassen sich anschließend Controller-Eingaben tätigen. Das Framework unterstützt Xbox 360-, sowie DualShock 4-Controller [[@VirtualGamepadEmulationa]].

Eine von der Internet-Vertriebsplattform Steam, welche hauptsächlich Computerspiele vertreibt, veröffentlichte Umfrage hat ergeben, dass $45\ \%$ der Nutzer auf ihrer Plattform über einen Xbox 360 Controller verfügen, sowie $19\ \%$ über das neuere Xbox One Modell, welches seinem Vorgänger sehr ähnlich ist. Damit sind Xbox Controller mit großem Abstand am verbreitetsten. Aufgrund dieser Annahme wurde sich im Rahmen der vorliegenden Arbeit für die Emulation eines Xbox 360 Controllers entschieden.

![[steamControllerStatistik.jpg|600]]
(Abb.<mark>?</mark>, Verteilung von Besitz von verschiedenen Spielcontrollern auf der Plattform Steam)

## 5.4 Algorithmen zur Abbildung der Raddaten in Controller-Eingaben
Die Sensor-Daten der Gyroskope liefern die Winkelgeschwindigkeiten der Räder des Rollstuhls. Diese sollen – wie bereits in Kapitel 5.3.1 beschrieben – auf die Eingabemöglichkeiten eines Spielcontrollers abgebildet werden, um sich im virtuellen Raum bewegen oder andere Eingaben tätigen zu können. Die Abbildung erfolgt dabei auf einen Xbox360 Controller. Somit sind die abgebildeten Eingaben von jeder Software lesbar, die eine Xbox360 Controllerunterstützung implementiert haben.

### 5.4.1 Abbildung auf einen Thumbstick
Der direkte Weg die Raddaten in eine Eingabe umzuwandeln, ist diese auf jeweils eine Achse eines Thumbsticks abzubilden. Dabei wird die x-Achse mit dem einen, die y-Achse mit dem anderen Rad dargestellt. Vorteilig ist dabei, dass beide Achsen gleichzeitig angesprochen werden können. Jedoch ist es schwieriger, die x-Achse zu bewegen, da sie anders ausgerichtet ist als das Rad, das gedreht wird. Alternativ kann das Ansprechen einer Achse auch durch beide Räder passieren. Dabei wird die x-Achse dann angesprochen, wenn sich die Räder gegeneinander drehen und die y-Achse, wenn sich die Räder miteinander drehen. Damit wird eine intuitive Nutzung angestrebt. Jedoch ist es dabei nicht mehr möglich, gleichzeitig den Cursor entlang beider Achsen zu bewegen, da sich die Räder nicht gleichzeitig mit- und gegeneinander drehen können.

### 5.4.2 Abbildung auf einen simulierten Rollstuhl
Da das im Rahmen dieser Arbeit entwickelte System darauf abzielt, in einem dreidimensionalen virtuellen Raum, auf einer Ebene zu navigieren, wird eine Abbildung benötigt, die die Position des Nutzers im virtuellen Raum verändert. Da die Daten ohnehin von einem Rollstuhl kommen, liegt die Abbildung auf einen simulierten Rollstuhl nahe. Um die Raddaten der zwei Räder auf eine Bewegung und Rotation eines Rollstuhls umzurechnen, muss erst festgestellt werden, welche Drehbewegungen zu welchen Rollstuhlbewegungen führt. Dabei können vier vereinfachte Fälle unterschieden werden:

![[wheelchairCases.PNG|600]]
(Abb.<mark>?</mark> Die Bewegungs-Fälle des Rollstuhl aus der Vogelperspektive)

**Fall 1:** Drehen sich die Räder mit gleicher Geschwindigkeit in dieselbe Richtung, so ruft dies eine Bewegung nach vorne oder hinten aus.
**Fall 2:** Drehen sich die Räder mit gleicher Geschwindigkeit gegeneinander, so ruft dies eine Rotation um die eigene Achse hervor.
**Fall 3:** Dreht sich nur ein Rad, so dreht sich dieses um das Stehende.
**Fall 4:** Drehen sich die Räder unterschiedlich schnell, so muss die Bewegung zusammengesetzt werden aus den Bewegungskomponenten einer der ersten beiden Fälle und dem dritten Fall.

Im Folgenden wird die Berechnung der Bewegungsanteile aufgezeigt, bestehend aus Bewegung nach vorne/hinten und der Rotation um die eigene Achse:

$$
\begin{align}
vl :&\ \mathrm{Bahngeschwindigkeit\ des\ linken\ Rades}\\
vR :&\ \mathrm{Bahngeschwindigkeit\ des\ rechten\ Rades}\\
m :&\ \mathrm{Bahngeschwindigkeit\ Minimum}\\
o :&\ \mathrm{Overshoot}\\
d :&\ \mathrm{Abstand\ Der\ Räder}\\
\vec{f} :&\ \mathrm{Fortbewegungsvektor}\\
\vec{r} :&\ \mathrm{Rotationsvektor}\\
\vec{f_{1,2}} :&\ \mathrm{Fortbewegungsvektor\ Fall1\ oder\ Fall2}\\
\vec{r_{1,2}} :&\ \mathrm{Rotationsvektor\ Fall1\ oder\ Fall2}\\
\vec{f_{3}} :&\ \mathrm{Fortbewegungsvektor\ Fall3}\\
\vec{r_{3}} :&\ \mathrm{Rotationsvektor\ Fall3}\\
\end{align}
$$

![[WheelchairMath.PNG|400]]
(Abb.<mark>?</mark> Skizze des Rollstuhls aus der Vogelperspektive)

Zunächst müssen die Rotationen der Räder dekonstruiert werden. Dabei lässt sich die Rotation eines Rades in zwei Komponenten aufspalten. Zum einen in den Minimum-Anteil $m$, den sich beide Räder drehen,

$$m = \min{(\left| vL \right|,\left| vR\right|)}$$

zum anderen in den Overshoot-Anteil $o$, den sich ein Rad schneller dreht als das andere.

$$o = \left|\left| vL \right|-\left| vR\right| \right|$$

**Fall 1:**
Die Bewegung nach vorne oder hinten ($Fortbewegungsvektor\ Fall1\ oder\ Fall2\ f_{1,2}$) ergibt sich in diesem Fall aus dem Anteil der Geschwindigkeit, mit denen sich beide Räder drehen. Dabei dreht sich jedoch der Rollstuhl nicht.

$$
\begin{align}
\vec{f_{1,2}} =&\ m \\
\vec{r_{1,2}} =&\ 0
\end{align}
$$

![[wendekreis.PNG|300]]
(Abb.<mark>?</mark> Skizze des Rollstuhls aus der Vogelperspektive mit Wendekreisen)

**Fall 2:**
Zur Berechnung der Rotation um die eigene Achse wird zunächst der $Wendekreis\ w_1$ benötigt. Dieser Wendekreis ist abhängig vom $Abstand\ der\ Räder\ d$ und dessen Mittelpunkt liegt im Mittelpunkt zwischen den Rädern. Anschließend wird mithilfe des $Minimums\ m$, das Verhältnis von $m$ zu $w_1$ errechnet, also wie viel vom Wendekreis gedreht wird. Dieses Verhältnis muss zum Schluss mit $360$ multipliziert werden, um den resultierenden Winkel, beziehungsweise $Rotationsvektor\ Fall1\ oder\ Fall2\ r_{1,2}$, zu berechnen. Bei dieser Bewegung verändert der Rollstuhl jedoch nicht seine Position.

$$
\begin{align}
w_1 =&\ d \cdot π \\
\vec{r_{1,2}} =&\ \left(\frac {m} {w_1}\right) \cdot 360 \\
\vec{f_{1,2}} =&\ 0
\end{align}
$$

**Fall 3:**
Bei diesem Fall gibt es einen $Fortbewegungs-$ und einen $Rotationsvektor$ ungleich null. Da sich nur ein Rad bewegt, hat sich der Wendekreis vergrößert zu $w_2$. Der Durchmesser von $w_2$ ist nun doppelt so groß wie von $w_1$, da das stehende Rad nun der Mittelpunkt des Wendekreises ist. Jetzt wird der $Overshoot\ o$ (also der Anteil der Bewegung des Rades, das sich mehr als das andere dreht) ins Verhältnis gesetzt mit $w_2$ und erhält dadurch $Θ$. Verrechnet man $Θ$ mit dem inneren Wendekreis $w_1$, so erhält man den $Fortbewegungsvektor\ Fall3\ f_3$.

$$
\begin{align}
w_2 =&\ 2 \cdot d \cdot π \\
Θ =&\ \frac {o} {w_2} \\
\vec{f_3} =&\ Θ \cdot w_1
\end{align}
$$

Um den $Rotationsvektor\ Fall3\ r_3$ berechnen zu können, muss $Θ$ mit $360$ multipliziert werden.

$$
\begin{align}
\vec{r_3} = Θ \cdot 360
\end{align}
$$

Da die Berechnungen mit den absoluten Rotationswerten errechnet wurden, ist es notwendig anhand der Drehrichtungen beider Räder zu bestimmen, ob sich der Rollstuhl vor- oder zurückbewegt und ob er sich dabei nach links oder rechts dreht. Die Drehrichtung ist immer dann links, wenn:

$$
vL < vR
$$

Es handelt sich um eine Vorwärtsbewegung, wenn:

$$
vL + vR > 0
$$

**Fall 4:**
Bevor die zusammengehörenden Vektoren zusammengerechnet werden können, muss überprüft werden, ob $s_{1,2}$ und $r_{1,2}$ mithilfe des ersten oder zweiten Falls berechnet werden müssen. Gilt folgende Bedingung, drehen sich die Räder gegeneinander und es wird Fall 2 benötigt. Andernfalls gilt Fall 1.

$$
(vL > 0) \oplus (vR > 0)
$$

Anschließend können die Bewegungskomponenten addiert werden:

$$
\begin{align}
\vec{s} =&\ \vec{f_{1,2}} + \vec{f_3} \\
\vec{r} =&\ \vec{r_{1,2}} + \vec{r_3}
\end{align}
$$

Berechnet man auf Grundlage der oben genannten Formeln die Bewegung des Rollstuhls, so stößt man auf folgendes Problem: Da die Räder bei einer beabsichtigten Bewegung nach vorne meist mit leicht unterschiedlicher Geschwindkeit drehen, erfährt der simulierte Rollstuhl eine Ablenkung nach rechts oder links. Eine Ausrichtung des Rollstuhls, die der Nutzer geziehlt vorgenommen hat, um ein bestimmtes Ziel im virtuellen Raum zu erreichen, ist damit nichtig, da der Rollstuhl sein Ziel verfehlt. Bildet man den $Durchschnitt\ v$ der Rotationen beider Räder und nutzt ihn anstelle vom $Minimum\ m$, so verhindert man die unbeabsichtigte Ablenkung. 

$$
\begin{align}
\vec{v} = \frac {(vL + vR)} {2} 
\end{align}
$$

Verwendet man den $Durchschnitt\ v$ müssen die verschiedenen Fälle distinkt sein, da sonst das Wenden mit einem Rad nicht mehr möglich wäre. Grund dafür ist der zusammenaddierter Mittelwert der Geschwindigkeiten des stillstehenden und des sich drehenden Rades. Dieser würde anstelle einer Drehung zu einer Vorwärtsbegung führen. Das Zusammenrechnen von Fall 1 oder Fall 2, mit Fall 3 darf also nicht geschehen. Um dies zu realisieren, sind Bewegungszustände notwendig. Auf Basis dieser kann entschieden werden, welcher Fall genutzt werden muss, um die resultierende Bewegung zu errechnen. Im nächsten Kapitel wird darauf eingegangen welche Bewegungszustände es gibt und wie diese erkannt werden können.

### 5.4.4 Bewegungszustände
Um alle Bewegungszustand-Permutationen ermitteln zu können, muss die Rotation der Räder als diskret und nicht als kontinuierliche Bewegung verstanden werden. So kann sich ein Rad in drei Zuständen befinden: Still stehend, nach vorne drehend und nach hinten drehend. Zwei Räder mit jeweils drei Zuständen ergeben dabei neun Bewegungsmuster ($3^2 = 9$).

![[WheelchairStates.PNG|500]]
(Abb.<mark>?</mark>, Die neun Bewegungszustände eines Rollstuhls)

 Jedoch lassen sich die Bewegungsmuster in folgende disjunkten Teilmengen unterteilen:
- _Ruhezustand_: kein Rad dreht sich (5)
- _Rotation um die eigene Achse_: Räder drehen sich gegeneinander (4, 6)
- _Einzelradbewegung_: ein Rad steht still und ein Rad dreht sich (1, 3, 7, 9)
- _Sichtachsenbewegung_: Räder drehen sich in dieselbe Richtung (2, 8)

Im Folgenden wird darauf eingegangen, wie diese Teilmengen-Zustände erkannt werden:

$$
\begin{align}
vl :&\ \mathrm{Bahngeschwindigkeit\ des\ linken\ Rades}\\
vR :&\ \mathrm{Bahngeschwindigkeit\ des\ rechten\ Rades} \\
s :&\ \mathrm{Schwellenwert}\\
\end{align}
$$

**Ruhezustand**
Der _Ruhezustand_ wird erreicht, wenn kein anderer Zustand zutrifft oder sich kein Rad dreht. Da das oben beschriebene Rauschen abgeschnitten wurde, gilt der Ruhezustand wenn:

$$
\begin{align}
(vL = 0) \land (vR = 0) 
\end{align}
$$

**Einzelradbewegung**
Wie beim Ruhezustand ist es auch hier möglich, darauf zu prüfen, dass ein Wert 0 ist. Die einfachste Art und Weise dies zu prüfen, ist folgende Bedingung:

$$
\begin{align}
(vL = 0) \oplus (vR = 0) 
\end{align}
$$

Der Nutzer hat jedoch Schwierigkeiten, ein Rad vollständig ruhig zu halten. Die Gyroskop-Werte überschreiten selbst bei kleinen Handbewegungen den Schwellenwert für die Rauschunterdrückung. Dies führt zu unbeabsichtigten Eingaben. Deshalb ist diese Methode unzureichend. Führt man einen Schwellenwert $t$ ein, wird eine vom Nutzer unbeabsichtigte _Einzelradbewegung_ zuverlässiger unterdrückt:

$$
\begin{align}
(|vL| < s) \oplus (|vR| < s) 
\end{align}
$$

**Rotation um die eigene Achse**
Die Bedingung, die gelten muss, wenn sich beide Räder gegeneinander drehen, ist identisch mit der Bedingung, welche schon im Kapitel _Abbildung auf einen realistisch simulierten Rollstuhl_ aufgestellt wurde. Diese gilt nur, wenn _Ruhezustand_ und _Einzelradbewegung_ ausgeschlossen werden konnten, da nicht die Fälle abgedeckt werden, wenn $vL$ oder $vR$ 0 sind:

$$
\begin{align}
(vL > 0) \oplus (vR > 0)
\end{align}
$$

**Sichtachsenbewegung**
Der Zustand der Sichtachsenbewegung gilt dann, wenn sich die Räder in dieselbe Richtung drehen. Damit ist dieser Zustand das logische Gegenteil der Bedingung _Rotation um die eigene Achse_, vorausgesetzt es wurden _Ruhezustand_ und _Einzelradbewegung_ ausgeschlossen:

$$
\begin{align}
(vL > 0) \Leftrightarrow (vR > 0) 
\end{align}
$$


### 5.4.5 Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen
Werden, wie in Kapitel _Abbildung auf einen realistisch/idealisierten simulierten Rollstuhl_ erläutert, die Eingaben ausschließlich auf eine Rollstuhlbewegung abgebildet, so ist der Nutzer eingeschränkt in seinen Interaktionsmöglichkeiten. Aktionen, wie ein Knopfdruck sind in diesen Fällen nicht möglich. Jedoch können bestimmte Bewegungsmuster, die nicht zwangsläufig notwendig sind, genutzt werden, um weitere Interaktionen abzubilden. Werden für das Drehen des Rollstuhls nur die Zustände genutzt, bei denen sich die Räder gegeneinander drehen (in Abb.<mark>?</mark> Zustand 4 und 6), so bleiben vier Zustände übrig, die mit anderen Interaktionen belegt werden können (in Abb.<mark>?</mark> Zustand 1, 3, 7, 9). Bei diesen vier Mustern handelt es sich, um die Einzelradbewegungs-Zustände. Diese können dann beispielsweise für das Drücken eines Knopfes genutzt werden.

Will man weitere Interaktionen abbilden, so ist dies nur noch möglich über die Kodierung der Radgeschwindigkeit durch den Nutzer. Entweder werden bestimmte Bewegungen der Räder unterschieden <mark>(Rad laufen lassen, Rad ruckartig bewegen und/oder über Bewegungen ähnlich zu Morsecode Information codieren)</mark> oder der Wertebereich wird geteilt mithilfe von Schwellwerten. Aufgrund des zeitlichen Rahmens dieser Arbeit wurde sich auf das Testen der zweiten Methode beschränkt.

Dazu wurde der Wertebereich zunächst mithilfe des Schwellenwertes geteilt. Von einer weiteren Teilung ist abzuraten, da es sonst für den Nutzer schwierig wird, die Räder mit den gewünschten beziehungsweise notwendigen Geschwindigkeiten zu drehen. Die Unterscheidung zwischen langsamer und schneller Rotation ist jedoch intuitiv von jedem Nutzer umsetzbar und verstehbar. Mit der Aufteilung in schneller und langsamer Geschwindigkeit ist die Anzahl der Bewegungsmuster theoretisch verdoppelt worden. 
Im Wertebereich der langsamen Bewegungen können nun Bewegungen, wie das Neigen des Kamerawinkels, abgebildet werden. Dabei wurde sich für die Bewegungszustände 2 (Neigung nach oben) und 8 (Neigung nach unten) entschieden. Für das Detektieren dieses neuen Teilmengen-Bewegungszustandes wird folgende Bedingung benötigt:

**Neigen**

$$
\begin{align}
((vL > 0) \Leftrightarrow (vR > 0))  \land (|vL| < s) \land (|vR| < s) 
\end{align}
$$

Unweigerlich geht dabei die Möglichkeit verloren, seinen $Fortbewegungsvektor\ s$ feiner einzustellen. Es sind also keine langsamen Bewegungen nach vorne und hinten möglich. Dafür hat der Nutzer die Möglichkeit, sich frei im Raum umschauen zu können. 

## 5.5 System-Analyse
Um das vorher beschriebene System zu testen, wurden verschiedene Datenreihen gemessen und Beobachtungen observiert. Im Folgenden sollen diese analysiert werden. 

### 5.5.1 Idealer Gyroskop-Modus und Fortbewegung
Als Erstes wird der Frage nachgegangen, in welchem Modus das Gyroskop betrieben werden sollte. Hierfür wurde eine Datenreihe gemessen, mit der Gradzahl pro Sekunde im Verlauf der Zeit. Eine Testperson hat dabei versucht, ein Rad so schnell wie möglich zu drehen. 

![[gyroMax.PNG|700]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)

Das Diagramm zeigt bei einer Roation nach vorne einen maximal erreichten Ausschlag um $800\ °/s$. Bei Rotationen nach hinten ist der Ausschlag etwas höher, übersteigt jedoch nicht -900. Daraus folgt, dass Gyroskop-Modus 2 für dieses Szenario der Ideale ist. Bei diesem Modus ist die $Maximale\ Gradzahl\ pro\ Sekunde=1000\ °/s$. Der Nutzer erreicht nicht die maximal Geschwindigkeit und reizt trotzdem den Wertebereich ca. $80\ \%-90\ \%$ aus. Deshalb wird dieser Modus im System verwendet.

In vielen Anwendungen ist es von Vorteil oder angenehmer für den Nutzer, sich schnellstmöglich mit der maximalen Fortbewegungsgeschwindigkeit zu bewegen. Auf Dauer ist es ermüdend, die Räder möglichst schnell drehen zu müssen. Um den Nutzer zu entlasten, kann entweder der $Fortbewegungsvektor\ f$ immer auf einen maximalen Thumbstick-Ausschlag abgebildet werden oder es wird ein weiterer Schwellenwert eingeführt, ab dem alle Eingaben als maximaler Thumbstick-Ausschlag abgebildet werden.

### 5.5.2 Datenrate der Übertragungsprotokolle
<mark>specs hinzufügen (router etc.)</mark>
Damit der Nutzer eine präzise Eingabe tätigen kann, ist es notwendig, dass möglichst schnell und kontinuierlich neue Pakete empfangen werden. Um die Datenrate zu ermitteln, mit der beide Nodes die Gyroskop-Werte verschicken, wird clientseitig alle 250ms die derzeitige Datenrate errechnet. Dazu zählt die Software seit der letzten Messung die eingegangenen Pakete und multipliziert diese mit 4, um die Datenrate pro Sekunde zu erhalten. Zusätzlich wird bei jedem Datensatz ermittelt, wie viel Zeit zwischen den letzten beiden Paketen vergangen ist. Auf die Messung der Latenz wurde aus Zeitgründen verzichtet, da dies erfodert hätte beide Seiten der Verbindung zu synchronisieren.

![[wifiStats.PNG|600]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)
![[espnowStats.PNG|600]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)

Messungen der getesteten Verbindungsprotokolle:

|                                   | Pakete pro Sekunde</br>Durchschnitt | Pakete pro Sekunde</br>Minimum | Pakete pro Sekunde</br>Maximum | Latenz in Millisekunden</br>Durchschnitt | Latenz in Millisekunden</br>Minimum | Latenz in Millisekunden</br>Maximum |
| --------------------------------- | ----------------------------------- | ------------------------------ | ------------------------------ | ---------------------------------------- | ----------------------------------- | ----------------------------------- |
| WiFi </br>(mit WebSocket)         | 250,16                              | 220                            | 284                            | 3,09                                     | < 1                                 | 26                                  |
| ESP-Now </br>(mit seriellem Port) | 330,43                              | 204                            | 440                            | 2,52                                     | < 1                                 | 30                                  |

Die Messungen haben ergeben, dass die Zeit zwischen zwei Paketen im Schnitt im niedrigen einstelligen Millisekundenbereich sind. _ESP-Now_ mit seriellem Port schafft dabei im Durchschnitt 80 Pakete mehr als WiFi mit einem WebSocket. Die Verbindung mit WiFi ist jedoch deutlich störungsfreier. So ist aus dem Diagramm <mark>2</mark> abzulesen, dass entweder _ESP-Now_ oder der serielle Port regelmäßiger und höhere Ausreißer erzeugt, bei denen die Zeit zwischen zwei Paketen über 15 Millisekunden ist. WiFi hingegen hat im kompletten Datensatz nur einen deutlichen Ausreißer und hat ansonsten selten Zeiten über 15 Millisekunden. Es kann geschlussfolgert werden, dass WiFi vorzuziehen ist. Jedoch kann auf _ESP-Now_ mit seriellem Port zurückgegriffen werden, wenn nur ein USB-Anschluss und kein WiFi Netzwerk verfügbar ist.
Anzumerken ist jedoch, dass die Messungen stark abhängig davon sind, welche USB-Anschlüsse und -Protokolle verwendet wurden, sowie welche Datenraten der Router unterstützt. Ein weiterer Einflussfaktor ist die Verbindung zwischen Client und dem Router. Sind diese kabellos verbunden, erhöht sich zusätzlich die Zeit, die ein Paket zur Übertragung benötigt, im Gegensatz zur kabelgebundener Übertragung. Deshalb sind die erhobenen Messwerte nur begrenzt aussagekräftig. Trotzdem lässt sich erkennen, dass beide Methoden genug Pakete verschicken können, um eine flüssige Bewegung nativ aus den Daten berechnen zu können. Es sind keine Interpolationstechniken notwendig, um die Bewegung flüssig erscheinen zu lassen.

### 5.5.3 Detektieren von Bewegungszuständen
Für den Nutzer ist die korrekte Detektion von Bewegungszuständen entscheidend. Werden unerwünschte Zustände ermittelt, führt dies zu fehlerhaften Eingaben, welche der Nutzer als störend empfindet. Je mehr verschiedene Bewegungszustände voneinander unterschieden werden müssen, desto höher ist die Gefahr der Missinterpretation. Abgesehen davon ist es nicht immer sinnvoll für alle Bewegungsmuster den Wertebereich zu teilen (wie in Kapitel _5.4.5 Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen_). Bewegungen, wie die _Rotation um die eigene Achse_, wollen vom Nutzer entweder langsam und präzise oder schnell durchgeführt werden. Nur in bestimmten Fällen, wie bei der Fortbewegung, kann es sinnvoll sein, den Wertebereich zu teilen. So ist die Anzahl der tatsächlich sinnvollen Bewegungsmuster kleiner als die theoretisch denkbaren. Es muss bei jedem Zustand und jeder Interaktion abgewogen werden, ob eine Teilung des Wertebereichs sinnvoll ist, oder den Nutzer behindert. Trotz der verringerten Anzahl an Bewegungsmustern, sind beim Testen der _Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen_ zwei primäre Probleme beobachtet worden, bei denen fehlerhafte Eingaben getätigt werden. 

**Unbeabsichtigtes Betätigen von Interaktionstasten**
In den ersten Testreihen wurde für die Detektion von einer _Einzelradbewegung_ und dem Teilen des Wertebereichs in schnelle und langsame Bewegungen derselbe $Schwellwert\ s = 100$ verwendet. Unter Verwendung dieser Methode kommt es beim Anfahren oder Bremsen (_Sichtachsenbewegung_) zum unbeabsichtigten Betätigen von Interaktionstasten. Da sich die Räder nicht mit derselben Geschwindigkeit drehen, gibt es ein kurzes Zeitintervall, in dem ein Rad unter dem Schwellwert und ein Rad über dem Schwellenwert liegt. Für dieses Zeitintervall gilt die Bedingung der _Einzelradbewegung_, sodass eine Interaktionstaste betätigt wird.

Dieses Problem lässt sich über das Einführen eines neuen _Einzelradbewegung_ Schwellenwertes beheben. Wählt man für diesen einen geringeren Wert $s_1 = 25$ als für den Schwellwert für das Teilen des Wertebereichs $s_2$, entsteht eine Pufferzone. Beim Beschleunigen überschreiten die Gyroskop-Werte zunächst nacheinander $s_1$. Anschließend überschreiten die Werte nacheinander $s_2$. Solange beide Werte in der Pufferzone sind, kann weder eine _Einzelradbewegung_ noch eine _Sichtachsenbewegung_ detektiert werden.

![[1threshold.PNG|700]] 
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)
![[2threshold.PNG|700]]
(Abb.<mark>?</mark> Bahngeschwindigkeiten im Verlauf der Zeit, bei dem die Testperson ein Rad so schnell wie möglich dreht)


Durch das Einführen der neuen Schwellenwerte müssen folgende Teilmengen-Bewegungszustände erweitert werden:

**Einzelradbewegung**

$$
\begin{align}
((|vL| < s_1) \oplus (|vR| < s_1)) \land ((|vL| > s_2) \oplus (|vR| > s_2))
\end{align}
$$

**Sichtachsenbewegung**

$$
\begin{align}
((vL > 0) \Leftrightarrow (vR > 0)) \land ((|vL| \geq s_2) \land (|vR| \geq s_2))
\end{align}
$$

**Neigen**

$$
\begin{align}
((vL > 0) \Leftrightarrow (vR > 0)) \land (|vL| < s_2) \land (|vR| < s_2) 
\end{align}
$$

**Unbeabsichtigtes Neigen beim Anfahren**
Beim Anfahren oder Bremsen (_Sichtachsenbewegung_) wurde beobachtet, dass für ein kurzes Zeitintervall der Kamerawinkel unbeabsichtigt geneigt wird. Ähnlich wie beim vorangegangenen Problem wird auch hier beim Übergang von einem Zustand zum Nächsten ein unerwünschter Zwischenzustand erreicht. Da die Fehldetektion immer dann auftritt, wenn sich die Geschwindigkeit der Räder ändert, ist der hier verwendete Lösungsansatz, die Bedingung des Neigungs-Zustandes zu erweitern. In dieser wird nun auch abgefragt, ob die Summe der Änderungsraten $a$ der Gyroskope unter einem neuen Schwellenwert $s_3$ liegt. Haben die Räder ihre Zielgeschwindigkeit erreicht, fällt die Änderungsrate unter $s_3$, sodass der nächste korrekte Zustand detektiert werden kann. In den Tests hat sich $s_3 = 15$ als ein akzeptabler Wert herausgestellt. Für die Berechnung der Änderungsrate wird folgende Berechnung verwendet:

$$
\begin{align}
a = |(vL_{[1]} - vL_{[0]})| + |((vR_{[1]} - vR_{[0]})|
\end{align}
$$

Die Bedingung des Teilmengen-Zustands _Neigen_ muss wie folgt ergänzt werden:

**Neigen**

$$
\begin{align}
((vL > 0) \Leftrightarrow (vR > 0)) \land (|vL| < s_2) \land (|vR| < s_2) \land (a < s_3)
\end{align}
$$

![[ohneAcc.PNG|600]] 
(Abb.<mark>?</mark> Bewegungs-Zustände mit unerwünschtem Neigen)
![[mitAcc.PNG|600]] 
(Abb.<mark>?</mark> Bewegungs-Zustände unter Verwendung eines Schwellenwertes für die Änderungsrate)

In den Daten ist zu erkennen, dass zwischen dem _Sichtachsenbewegung_-Zustand und dem _Neigen_-Zustand nun ein kurzer Ruhezustand existiert. Diese Verzögerung bei der Fortbewegung ist vom Nutzer kaum bis gar nicht wahrnehmbar. Im gezeigten Beispiel beträgt diese nur etwa 70ms. Jedoch ist anzumerken, dass diese Methode vor allem bei ruckartigen Bewegungen funktioniert, da besonders dann die Änderungsrate schnell einen registrierbaren Ausschlag hat. Verändert sich die Geschwindigkeit nicht schnell genug, kann es immer noch zu registrierbaren Neigungen kommen.

___
