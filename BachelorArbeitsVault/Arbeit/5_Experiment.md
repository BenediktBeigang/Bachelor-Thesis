# 5. Experiment
___

```toc
```

## 5.1 Eingebettetes System zur Messung der Raddaten
Damit der Software bekannt ist, mit welcher Geschwindigkeit sich welches Rad in welche Richtung dreht, ist Hardware notwendig, welche die Rotationsdaten misst und an die Software übermittelt. Dazu wurde für das vorliegende Experiment ein eingebettetes System verwendet. Im Folgenden soll näher beleuchtet und erörtert werden, welche Hardware verwendet und wie die Raddaten gemessen und übertragen wurden. Dazu werden im Folgenden verschiedene Kommunikationsprotokolle verglichen.

### 5.1.1 <mark>Verfahren zur Datenerhebung</mark>
Um die Rotation der Räder des Rollstuhls messen zu können, wird ein Sensor benötigt. Dabei gibt es verschiedene Herangehensweisen, wie ein Sensor die Rotation messen kann. 

<mark>
So arbeiten viele Sensoren mit Lichtschranken. Hierbei wird die Lichtschranke in regelmäßigen Abständen durch ein Hindernis blockiert. Daraus kann dann über die Frequenz, in der dies geschieht, eine Geschwindigkeit errechnet werden. Vorteil ist dabei, dass keine Hardware auf dem Rad befestigt werden muss und deshalb kein mobiles System benötigt wird. Es kann auf Technologien wie WiFi und Akkus verzichtet werden.
Ein großer Nachteil bei diesem Verfahren ist jedoch, dass für besonders kleine Rotationen die Abstände der Hindernisse sehr klein sein müssen. Angewandt auf den Rollstuhl bedeutet dies, dass eine zusätzliche Konstruktion gebaut werden muss, damit die Lichtschranke unterbrochen wird. Da dies sehr unpraktikabel ist, wurde dieses Verfahren nicht verwendet.
Die zweite Möglichkeit ist die Verwendung eines Gyroskops. Dieses kommt ohne zusätzliche Konstruktionen aus, erfordert jedoch, dass die Elektronik mobil ist. Die Datenrate wird folglich durch die Bandbreite des drahtlosen Netzwerks begrenzt, da die Übertragung von Daten den Flaschenhals solcher Systeme darstellt.</mark>

### 5.1.2 MPU-6050
Im Zuge dieser Arbeit habe ich mich für das Motion-Tracking-Device MPU-6050 entschieden. 
Dieser ist klein (mit Pins: 20mm x 15mm x 11mm), kostengünstig zu erwerben (~4€) und verfügt unter anderem über 3-Achsen Gyroskop-Sensoren, mit welchen die Rotation gemessen werden kann. 
Der Chip besitzt folgende 8 Anschlüsse: <mark>Literatur!!!</mark> 

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

Die Daten können per I2C (serieller Datenbus) von einem angeschlossenen Mikrocontroller (ESP32) ausgelesen werden. Jede Achse wird auf zwei 8-Bit Register abgebildet. Zusammen ergibt das einen Wertebereich von 65.536 unterscheidbaren Zuständen <mark>Literatur!!!</mark> . Mit der Drehrichtung rückwärts halbiert sich dieser Wertebereich, da ein Bit für das Verschieben des Wertebereichs ins Negative benötigt wird.
Das Gyroskop des MPU-6050 kann in vier verschiedenen Konfigurationen betrieben werden <mark>Literatur!!!</mark> . Damit wird festgelegt, wie klein der Winkel zwischen zwei verschiedenen Ausgaben ist; mit anderen Worten, wie viele Stufen pro Grad unterschieden werden können. Da der Wertebereich konstant ist, bedeutet eine sensiblere Messung, <mark>dass das Gyroskop früher das Ende des Wertebereichs erreicht.</mark> Angewendet auf den Rollstuhl heißt das, dass das rotierende Rad bei niedrigeren Geschwindigkeiten seine maximal messbare Geschwindigkeit erreicht. In der folgenden Tabelle sind alle Konfigurationen mit ihren resultierenden Eigenschaften aufgelistet.

| Modus | Maximale Gradzahl pro Sekunde | Stufen pro Grad | Maximale Umdrehungszahl pro Sekunde | Maximale Radianten pro Sekunde | Zurückgelegte Distanz pro Stufe in mm* |
| ----- | ----------------------------- | --------------- | ----------------------------------- | ------------------------------ | -------------------------------------- |
| 0     | 250                           | 131             | 0,69                                | 4,36                           | 0,04                                   |
| 1     | 500                           | 65,5            | 1,39                                | 8,73                           | 0,08                                   |
| 2     | 1000                          | 32,8            | 2,78                                | 17,47                          | 0,16                                   |
| 3     | 2000                          | 16,4            | 5,56                                | 34,93                          | 0,32                                   |

\*Werte bei einem Raddurchmesser von 60 cm - <mark>eigene Messung sowie Literatur!!!</mark> 

An dieser Stelle stellt sich die Frage, welcher Modus für das hier entwickelte System das Optimale ist. Um dem Nutzer ein möglichst störungsfreies Erlebnis zu bieten, muss gewährleistet sein, dass das Gyroskop so sensitiv wie möglich eingestellt ist. Das bedeutet, dass der Wertebereich maximal ausgereizt werden muss. Ist der Modus nicht sensitiv genug, so bemerkt der Nutzer möglicherweise das Springen der Bitwerte in Form eines Vorspringens in der Bewegung. Allerdings muss ein Modus gewählt werden, welcher dazu führt, dass der Nutzer nicht schneller als die maximale Gradzahl pro Sekunde drehen kann, da es sonst zu einem Zahlenüberlauf kommt und zu einer fehlerhaften Weiterverarbeitung der Daten führt. Der Modus muss also so sensitiv sein, dass der Nutzer nicht den Übergang von einem Zustand in den nächsten registriert. Gleichzeitig darf er nicht in der Lage sein, die Räder schneller als die maximale Gradzahl pro Sekunde zu drehen. Im Kapitel System-Analyse wird dieser Frage weiter nachgegangen.

### 5.1.3 ESP32
Um den MPU-6050 betreiben und dessen Daten an eine Software übermitteln zu können, wird ein Mikrocontroller-Board benötigt. Es muss per I2C die entsprechenden Register auslesen und mittels drahtloser Kommunikation versenden. Außerdem muss er das Gyroskop, sowie sich selbst mit Strom versorgen. Auf dem Markt gibt es eine große Anzahl von Produkten für die verschiedensten Anwendungsgebiete. Im Rahmen dieser Arbeit wurde der Mikrocontroller ESP32 verwendet, das aktuellste Modell der Firma _Espressif_. Boards mit diesem Chip sind kostengünstig (~8€) und zudem ist der ESP32 mit WiFi, Bluetooth und _ESP-Now_ Unterstützung ausgestattet <mark>Literatur!!!</mark>  . Verbaut wurde ein Xtensa® 32-bit LX6 Mikroprozessor, mit 240MHz Taktfrequenz, 448 KB ROM und 520 KB SRAM. [[@ESP32Datasheet2022]] <mark>Literatur!!!</mark> 
Als Entwicklungsboard wurde das ESP32 Dev Kit C V4 verwendet.

Der MPU-6050 muss wie folgt an das Entwicklungsboard angeschlossen werden:

| ESP32            | MPU-6050 |
| ---------------- | -------- |
| 3.3V             | VCC      |
| GND              | GND      |
| GPIO_22 (I2C CL) | SDA      |
| GPIO_21 (I2C DA) | SCL      |
| ADO              | GND      |

### 5.1.4 PlatformIO
Zur Entwicklung der Software, die auf den Mikrocontrollern läuft, wurde PlatformIO verwendet. Dies ist eine Erweiterung für Visual Studio Code, bei der die benötigten Bibliotheken, die für jeden Mikrocontroller und jedes Board notwendig sind, automatisch heruntergeladen und eingerichtet werden. Ebenfalls lassen sich über das UI Bibliotheken, die für das jeweilige Projekt notwendig sind, hinzufügen. Zusätzlich zur Entwicklungsumgebung von Visual Studio Code gibt es Funktionalitäten einen Chip zu flashen und anschließend im seriellen Monitor die Ausführung zu beobachten. Im Gegensatz zu Umgebungen wie der Arduino IDE wird Zeit gespart, da dort zunächst manuell Treiber heruntergeladen werden müssen und man nicht von den Vorteilen einer moderneren IDE profitiert.

### 5.1.5 3D gedruckte Box
Damit Entwicklungsboard, Gyroskop und Akku zusammengehalten werden, geschützt sind und am Rad befestigt werden können, wird eine Box benötigt, die alle Komponenten aufnehmen kann und diese trägt. Aufgrund dessen habe ich für meine Arbeit - mithilfe von Blender - eine entsprechend seinen Anforderungen konstruierte Box entworfen, welche ich mithilfe eines 3D-Druckers drucken ließ.

## 5.2 Transfer der Gyroskop-Daten
Für die Übermittlung der Sensordaten an eine Software auf einem PC stehen verschiedene Möglichkeiten zur Verfügung. In dieser Arbeit sind zwei verschiedene Protokolle getestet worden: WiFi und ESP-Now. Die Protokolle müssen dabei leicht in das System integrierbar sein. WiFi ist ein weit verbreiteter Standard, sodass entsprechende Bibliotheken schon existieren, um das Protokoll einfach einbinden zu können. ESP-Now ist weniger verbreitet, da aber der Chip vom selben Hersteller kommt, existieren auch hier schon Bibliotheken. Ein weiteres Protokoll, welches ebenfalls hätte getestet werden können ist Bluetooth, das jedoch aufgrund des zeitlichen Rahmens dieser Arbeit nicht getestet werden konnte.

### 5.2.1 WiFi und WebSockets
>TODO
>warum nicht udp? warum tcp? alternativen?
>TODO

WiFi ist eine Kommunikations-Technologie, die durch die WiFi-Allianz entstanden ist und bis heute von ihr gepflegt wird. Sie ermöglicht drahtlose Kommunikation mit jedem Gerät, das diese Technologie implementiert. Inzwischen ist WiFi ein weit verbreiteter Standard, welches von den meisten mobilen Geräten unterstützt wird. Der ESP32 verwendet das WiFi Protokoll 802.11 und arbeitet im 2,4-2,5 GHz Bereich. <mark>Quelle</mark>
Zunächst muss eine Verbindung zwischen dem ESP32 und dem lokalen Netzwerk mittels WiFi aufgebaut werden. Damit die Zugangsdaten nicht fest in den Code geschrieben werden müssen, wird die Bibliothek _WiFi Manager_ verwendet. Die Bibliothek baut selbstständig eine Verbindung mit einem Netzwerk auf, nachdem die Zugangsdaten über ein Gerät wie zum Beispiel einem Smartphone übergeben wurden. Dazu wird ein Web-Konfigurations-Portal auf dem ESP32 gehostet, auf das ein nahes Gerät zugreifen kann. [[@tzapuWiFiManager2022]]

Für die eigentliche Übertragung der Daten können verschiedene Protokolle verwendet werden. Ein klassischer Vertreter ist HTTP(S). Jedoch ist das Protokoll auf Hypertext ausgelegt. Es wird für jede Abfrage von Daten eine neue TCP-Verbindung mit dem Server aufgebaut, der die Daten nach Eingang der Anfrage zurückschickt. Will der Client auf dem Rechner neue Daten empfangen, so muss dieser erneut eine Verbindung aufbauen und eine neue Anfrage an den Server stellen. Zusätzlich ist in jedem Paket viel Overhead. Bei HTTP werden in der Praxis verschiedene Header und Cookie-Daten mitgesendet, die zusammen ein paar Kilobyte groß sind.
Im Szenario dieser Arbeit würde der ESP32 einen Server hosten. Eine Software auf einem externen Rechner würde bei diesem Server neue Daten abfragen. Da es sich bei diesem System jedoch um Echtzeitdaten handelt, wäre dieses Vorgehen ineffizient, da viel Zeit für das Übertragen von nicht benötigte Daten verwendet werden würde.

Eine Alternative ist die Verwendung eines WebSockets. Es wurde entwickelt, um das oben geschilderte Problem zu lösen und wird heute breit unterstützt. Das Web-Socket-Protokoll wurde in seiner finalen Form 2011 von der Internet Engineering Task Force entwickelt und veröffentlicht. [[@ietfRFC6455WebSocket]]
Dabei wird analog zu HTTP am Anfang ein TCP Handshake durchgeführt. Der Clienten stellt an den Server eine Verbindungsanfrage, den der Server bestätigt. Ab diesem Zeitpunkt sendet der Server unaufgefordert die vom Clienten abonnierten Daten, bis die Verbindung vom Clienten beendet wird. Somit lassen sich höhere Datenraten erzielen, die für Echtzeitanwendungen notwendig sind.
Das für die vorliegende Bachelor-Thesis entwickelte System setzt auf einen vom ESP32 gehosteten WebSocket-Server, der von einer Software auf dem PC abonniert wird und die Daten weiter verarbeitet.

Zusätzlich zur eigentlichen Übertragung der Daten ist es notwendig, dass die Software auf dem PC den IP-Endpunkt des WebSockets auf dem ESP32 kennt. Dazu sendet der Mikrocontroller ebenfalls über WiFi einen UDP-Broadcast ins Netzwerk. Neben dem IP-Endpunkt, werden auch Informationen über das Gerät mitgesendet, damit die Software auf dem PC weiß, um welches Gerät es sich handelt. Nach dieser Bekanntmachung kann der WebSocket abonniert und die Daten übertragen werden.

| Vorteile                                         | Nachteile                                           |
| ------------------------------------------------ | --------------------------------------------------- |
| Direkte Kommunikation der Geräte zur PC-Software | Verbindungsaufbau ist aufwändiger zu implementieren |
| Nur zwei Geräte werden benötigt                  |                                                     |
| Zugangsdaten müssen nicht fest-gecodet werden    |                                                     |

### 5.2.2 ESP-Now und Serieller Port
ESP-Now ist ein vom Unternehmen _Espressif_ selbst entwickeltes Übertragungsprotokoll, mit dem Mikrocontroller von _Espressif_ wie zum Beispiel der ESP8266 (Vorgänger des ESP32) und der ESP32 direkt miteinander Daten austauschen können. Dabei verwendet das Protokoll die MAC-Adressen zur Identifikation der Geräte. Es wird jedoch nur eine Verbindung in eine Richtung aufgebaut. Ein großer Vorteil dieses Protokolls ist die einfache Einbindung in das System. Anders als WiFi muss nicht zunächst eine Verbindung zu einem Netzwerk aufgebaut werden, sondern dem Gerät muss lediglich die MAC-Adresse des Zielgeräts vorliegen. Da die Kommunikation jedoch nur unter Mikrocontrollern stattfindet, muss das Gerät, welches die Sensor-Daten entgegennimmt, diese Daten mittels seriellen Ports an die Software auf dem PC übertragen. Damit steigt die Anzahl der Verbindungen, an denen die Übertragung scheitern kann. Jedoch erleichtert es die Verwendung für den Endbenutzer, da dieser kein WiFi-Netzwerk benötigt, um die Geräte mit der Software auf dem PC zu verbinden. Eine Verbindung per USB-Kabel ist ausreichend.

| Vorteile                                                                    | Nachteile                                                                                                   |
| --------------------------------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| Kein WiFi oder Bluetooth notwendig                                          | Zwei statt einer Verbindung muss aufrecht erhalten werden                                                   |
| Das empfangende Gerät kann einfach mit einem USB-Kabel angeschlossen werden | Ein zusätzliches Gerät wird benötigt, um die Daten zu empfangen und an die Software weiterzureichen         |
|                                                                             | Die Ziel-MAC-Adressen müssen im Code hartkodiert werden oder über andere Wege übertragen werden |

___

## 5.3 Interface für Nutzung in Spielen und Software
Damit das im Rahmen dieser Arbeit entwickelte System die transformierten Raddaten beziehungsweise Interaktionen in externer Software, zum Beispiel XYZ...  nutzen kann, ist eine Verbindung zwischen Rollstuhl-Software und der externen Software notwendig. Da nicht davon ausgegangen werden kann, dass jede externe Software eine Schnittstelle implementiert, mit der die beiden Softwares kommunizieren können, müssen die Daten des Rollstuhls auf herkömmliche Eingabegeräte, wie zum Beispiel XYZ ... gemappt/abgebildet werden. 

### 5.3.1 Tastatur oder Spielcontroller
Dabei kommen zwei Möglichkeiten infrage: eine Tastatureingabe und eine Spielcontrollereingabe. Beide Eingaben werden von den meisten Anwendungen unterstützt und eigenen sich unterschiedlich gut für die Zwecke des hier entwickelten Systems. Tastatureingaben bieten den Vorteil, dass sie von fast jeder erdenklichen Software unterstützt werden. Jedoch lassen sich damit nur binäre Eingaben tätigen, sprich es lassen sich nur Tasten drücken. Da die Rollstuhl-Eingaben jedoch unterschiedliche Werte innerhalb eines Wertebereichs darstellen, wird bei einer Tastatureingabe die Interaktionsmöglichkeit stark eingeschränkt. 
Die Alternative ist eine Spielcontroller-Eingabe. Hier gibt es ebenfalls Knöpfe, beziehungsweise binäre Eingaben, jedoch auch Eingaben entlang von Achsen innerhalb eines Wertebereichs. Überwiegend werden die Achsen in Form eines Thumb-Sticks oder Knopfes mit mehreren Stufen realisiert. Rollstuhl-Eingaben wie das Fortbewegen könnte so einfacher auf das Gerät abgebildet werden. Allerdings unterstützt nicht jede Software Eingaben mittels eines Spielcontrollers. Da jedoch die meiste Software in der Fortbewegung eine Rolle spielt, Spielcontroller unterstützt, wurde sich im Rahmen dieser Arbeit für diese Eingabeform entschieden.

### 5.3.2 Virtual Gamepad Emulation Framework
Um die Eingaben des Rollstuhls in tatsächliche Spielcontroller-Eingaben umzuwandeln, die von einem <mark>Rechner</mark> auch als Controller-Eingabe verstanden werden, ist eine Emulation eines Controllers notwendig. Ziel ist es, programmgesteuert Controller-Eingaben an den Rechner zu senden. Um sich den Aufwand des Schreibens eines neuen Treibers zu ersparen, wird an dieser Stelle auf das _Virtual Gamepad Emulation Framework_ zurückgegriffen. Dies ist eine Bibliothek, welche in bestehende Software integriert werden kann und einen virtuellen Controller mit dem Rechner verbindet. Über Befehle lassen sich anschließend Controller-Eingaben tätigen. Das Framework unterstützt Xbox 360-, sowie DualShock 4-Controller.

Eine von der Internet-Vertriebsplattform Steam, welche hauptsächlich Computerspiele vertreibt, veröffentlichte Umfrage hat ergeben, dass $45\%$ der Nutzer auf ihrer Plattform über einen Xbox 360 Controller verfügen, sowie $19\%$ über das neuere Xbox One Modell, welches seinem Vorgänger sehr ähnlich ist. Damit sind Xbox Controller mit großem Abstand am verbreitetsten. Aufgrund dieser Annahme wurde sich im Rahmen der vorliegenden Arbeit für die Emulation eines Xbox 360 Controllers entschieden.

![[steamControllerStatistik.jpg|600]]
(Abb.<mark>?</mark>, Verteilung von Besitz von verschiedenen Spielcontrollern auf der Plattform Steam)



<mark>Xbox360 Grafik mit Tastenbeschriftung</mark>

___

## 5.4 Algorithmen zur Abbildung der Raddaten in Controller-Eingaben
Die Sensor-Daten der Gyroskope liefern die Winkelgeschwindigkeiten der Räder des Rollstuhls. Diese sollen – wie bereits in Kapitel 5.3.1 beschrieben – auf die Eingabemöglichkeiten eines Spielcontrollers abgebildet werden, um sich im virtuellen Raum bewegen oder andere Eingaben tätigen zu können. Die Abbildung erfolgt dabei auf einen Xbox360 Controller. Somit sind die abgebildeten Eingaben von jeder Software lesbar, die eine Xbox360 Controllerunterstützung implementiert hat. <mark>was bedeutet abbildung hääää???</mark>

### 5.4.1 Gyroskop-Werte bereinigen
Die ausgelesenen Werte des Gyroskops sind nicht automatisch kalibriert. Sie besitzen einen konstanten Offset. Aus diesem Grund wird nach dem Verbindungsaufbau zwischen Node und Software eine Kalibrierungssequenz gestartet. Diese errechnet aus einer Reihe ausgelesener Werte einen Mittelwert, der anschließend von allen zukünftigen Werten abgezogen wird. Dazu dürfen die Räder nicht bewegt werden, da dies das Ergebnis der Kalibrierung unbrauchbar machen würde.

Darüber hinaus hat das Gyroskop-Signal ein Rauschen. Bei hoher Umdrehungszahl ist das Rauschen irrelevant, da es nur einen kleinen Anteil der Gesamtrotation ausmacht. Bei geringer Umdrehungszahl ist das Rauschen jedoch störend, da in diesem Fall nicht erkennbar ist, ob ein Rad still steht oder eine geringe Rotation gegeben ist. Aus diesem Grund wird ein Schwellenwert bestimmt, welcher der Gyroskop-Wert überschreiten muss, um als tatsächliche Bewegung erkannt werden zu können. Damit ist sichergestellt, dass es sich um eine tatsächliche Rotation handelt.

### 5.4.1 Abbildung auf einen Thumb-Stick
Die einfachste Art und Weise der Abbildung von Raddaten auf eine Eingabe, ist die Steuerung durch einen Thumb-Stick. Dabei wird die x-Achse mit dem einen, die y-Achse mit dem anderen Rad dargestellt. Vorteilig ist dabei, dass beide Achsen gleichzeitig angesprochen werden können. Jedoch ist es schwieriger, die x-Achse zu bewegen, da diese anders ausgerichtet ist als das Rad, das gedreht wird. Alternativ kann das Ansprechen einer Achse auch durch beide Räder passieren. Dabei wird die x-Achse dann angesprochen, wenn sich die Räder gegeneinander drehen und die y-Achse, wenn sich die Räder miteinander drehen. Damit wird eine intuitive Nutzung angestrebt. Jedoch ist es dabei nicht mehr möglich, gleichzeitig den Cursor entlang beider Achsen zu bewegen, da sich die Räder nicht gleichzeitig mit und gegeneinander drehen können.

### 5.4.2 Abbildung auf einen realistisch simulierten Rollstuhl
Da das im Rahmen dieser Arbeit entwickelte System darauf abzielt, im virtuellen Raum zu navigieren, wird eine Abbildung benötigt, die die Position des Nutzers im virtuellen Raum verändert. Die naheliegendste Methode ist dabei die Abbildung auf einen simulierten Rollstuhl, da die Daten ursprünglich von einem realen Rollstuhl gekommen sind und die Bewegungsmuster einfach aufeinander abgebildet werden können. Um die Raddaten der zwei Räder auf eine Bewegung und Rotation eines Rollstuhls umzurechnen, muss erst festgestellt werden, welche Radbewegungen zu welchen Rollstuhlbewegungen führt. Dabei können drei idealisierte Fälle unterschieden werden:

**Fall 1:** Drehen sich die Räder mit gleicher Geschwindigkeit in dieselbe Richtung, so ruft dies eine Bewegung nach vorne oder hinten aus.
**Fall 2:** Drehen sich die Räder mit gleicher Geschwindigkeit gegeneinander, so ruft dies eine Rotation um die eigene Achse hervor.
**Fall 3:** Dreht sich nur ein Rad, so dreht sich dieses um das Stehende.

<mark>fall 4 einfügen von unten und idealisisert rausnehmen</mark>

Im Folgenden wird die Berechnung der Bewegungsanteile aufgezeigt, bestehend aus Bewegung nach vorne/hinten und Rotation um die eigene Achse:

<mark>alle variablen hier einfügen</mark>
$$
\begin{align}
Bahngeschwindigkeit\ des\ linken\ Rades: vL \\
Bahngeschwindigkeit\ des\ rechten\ Rades: vR \\
Bahngeschwindigkeit\ Minimum: m \\
Overshoot: o \\
Abstand\ Der\ Räder: d \\
Fortbewegungsvektor: s \\
Rotationvektor: r \\
\end{align}
$$

![[WheelchairMath.PNG|500]]
(Abb.<mark>?</mark> Skizze des Rollstuhls aus der Vogelperspektive)

Da in der Realität die Bewegung nicht idealisiert ist, ist festzustellen, dass die tatsächliche Bewegung zusammengesetzt ist aus einer der ersten beiden Fälle und dem dritten Fall:

$$
\begin{align}
s = s_{1,2} + s_3 \\
r = r_{1,2} + r_3
\end{align}
$$

Um bestimmen zu können, ob es sich um Fall 1 oder Fall 2 handelt, muss folgende Bedingung geprüft werden, die gilt, wenn sich die Räder gegeneinander drehen:

$$
(vL > 0) \oplus (vR > 0)
$$

Die Rotation beider Räder lässt sich in zwei Komponenten aufspalten. Zum einen den Minimum-Anteil $m$, den sich beide Räder drehen.

$$m = min(\left| vL \right|,\left| vR\right|)$$

Zum anderen der Overshoot-Anteil $o$ den sich ein Rad schneller dreht als das andere.

$$o = \left|\left| vL \right|-\left| vR\right| \right|$$

**Fall 1:**
Die Bewegung nach vorne (oder hinten) ergibt sich in diesem Fall aus dem Anteil der Geschwindigkeit, mit denen sich beide Räder drehen.

$$s_1 = m$$

**Fall 2:**
Um die Rotation um die eigene Achse errechnen zu können, wird zunächst der Wendekreis $w_1$ bestimmt. Dieser Wendekreis ist abhängig vom Abstand der beiden Räder $d$ und dessen Mittelpunkt liegt im Mittelpunkt dieses Abstandes. Anschließend wird mithilfe des RadMinimums $m$, das Verhältnis von $m$ zu $w_1$ errechnet. Dieses Verhältnis muss zum Schluss mit $360$ multipliziert werden, um den resultierenden Winkel $r_{1,2}$ zu berechnen.

$$
\begin{align}
w_1 = d \cdot π \\
r_{1,2} = (\frac {m} {w_1}) \cdot 360
\end{align}
$$

**Fall 3:**
Bei diesem Fall gibt es eine Bewegungs- und eine Rotationskomponente. Da sich nur ein Rad bewegt, hat sich der Wendekreis vergrößert zu $w_2$. Der Durchmesser von $w_2$ ist nun doppelt so groß wie von $w_1$, da das stehende Rad nun der Mittelpunkt des Wendekreises ist. Jetzt wird der Overshoot $o$ (also der Anteil der Bewegung des Rades, das sich mehr als das andere dreht) ins Verhältnis gesetzt mit $w_2$ und erhält dadurch $Θ$. Verrechnet man $Θ$ mit dem inneren Wendekreis $w_1$, so erhält man die Bewegungskomponente $s_3$.

$$
\begin{align}
w_2 = 2 \cdot d \cdot π \\
Θ = \frac {o} {w_2} \\
s_3 = Θ \cdot w_1
\end{align}
$$

Um die Rotationskomponente $r_3$ berechnen zu können, muss $Θ$ mit $360$ multipliziert werden.

$$
\begin{align}
r_3 = Θ \cdot 360
\end{align}
$$

Da die Bewegungskomponenten mit den absoluten Rotationswerten errechnet wurden, ist es notwendig anhand der Drehrichtungen beider Räder zu bestimmen, ob sich der Rollstuhl vor- oder zurückbewegt und ob er sich dabei nach links oder rechts dreht. Die Drehrichtung ist immer dann links, wenn:

$$
vL < vR
$$

Es handelt sich um eine Vorwärtsbewegung, wenn:

$$
vL + vR > 0
$$

### 5.4.3 Abbildung auf einen idealisierten simulierten Rollstuhl
<mark>eventuell reinziehen in oberes kapitel</mark>
Bei der Verwendung der Abbildung hin zu einem realistischen Rollstuhl hat sich im vorangegangenen Abschnitt gezeigt, dass ein zielgerichtetes Vorankommen gestört wird. Ursache dafür ist, dass die Räder sich in leicht unterschiedlicher Geschwindigkeit drehen und so automatisch die Bewegung nach vorne, eine Ablenkung nach links oder rechts erfährt.

Dieses Problem kann gelöst werden, wenn mit den Rotationen beider Räder ein Mittelwert $v$ errechnet. Dieser Mittelwert wird dann die Kalkulation von Fall 1 und Fall 2, statt dem Minimum $m$ verwendet. 

$$
\begin{align}
v = \frac {(vL + vR)} {2} 
\end{align}
$$

Jedoch müssen die Fälle in diesem Fall distinkt sein, da sonst das Wenden mit einem Rad nicht mehr möglich wäre. Grund dafür ist, dass die Geschwindigkeiten des stillstehende Rades und das sich Drehenden zu einem Mittelwert zusammengerechnet werden würde und der Rollstuhl sich nur nach vorne bewegt, statt sich zu drehen. Das Zusammenrechnen von Fall 1 oder Fall 2 mit Fall 3 darf also nicht geschehen. Realisiert man die Bewegung des Rollstuhls, indem der Rollstuhl in verschiedenen Bewegungszuständen sein kann, so kann die Bewegung für jeden Fall einzeln richtig errechnet werden.

### 5.4.4 Bewegungszustände
Um alle Bewegungszustand-Permutationen ermitteln zu können, muss die Rotation der Räder als diskret und nicht als kontinuierliche Bewegung verstanden werden. So kann sich ein Rad in drei Zuständen befinden: Still stehend, nach vorne drehend und nach hinten drehend. Zwei Räder mit jeweils drei Zuständen ergibt dabei $3^2 = 9$ Bewegungsmuster.

![[WheelchairStates.PNG|500]]
(Abb.<mark>?</mark>, Die neun Bewegungszustände eines Rollstuhls)

 Jedoch lassen sich die Bewegungsmuster in folgende Untergruppen teilen:
- Ruhezustand: kein Rad dreht sich (5)
- Rotation um die eigene Achse: Räder drehen sich gegeneinander (4, 6)
- Einzelradbewegung: ein Rad steht still und ein Rad dreht sich (1, 3, 7, 9)
- Sichtachsenbewegung: Räder drehen sich in dieselbe Richtung (2, 8)

Im Folgenden wird darauf eingegangen, wie diese Untergruppen-Zustände erkannt werden:
<mark>Schwellwert\ für\ \textbf{Einzelradbewegung}: s_1 \\
Schwellwert\ für\ Teilung\ des\ Wertebereichs: s_2 \\</mark>

$$
\begin{align}
Bahngeschwindigkeit\ des\ linken\ Rades: vL \\
Bahngeschwindigkeit\ des\ rechten\ Rades: vR \\
Schwellenwert : s\\
\end{align}
$$

**Ruhezustand**
Der _Ruhezustand_ wird erreicht, wenn kein anderer Zustand erreicht wird oder sich kein Rad dreht. Da das oben beschriebene Rauschen abgeschnitten wurde, gilt der Ruhezustand wenn:

$$
\begin{align}
(vL = 0) \land (vR = 0) 
\end{align}
$$

**Einzelradbewegung**
Wie beim Ruhezustand ist es auch hier möglich darauf zu prüfen, dass ein Wert 0 ist. Die einfachste Art und Weise dies zu prüfen, ist folgende Bedingung:

$$
\begin{align}
(vL = 0) \oplus (vR = 0) 
\end{align}
$$

Der Nutzer hat jedoch Schwierigkeiten ein Rad vollständig ruhig zu halten. Die Gyroskop-Werte überschreiten selbst bei kleinen Handbewegungen den Schwellenwert. Dies führt zu unbeabsichtigten Eingaben. Deshalb ist diese Methode unzureichend. Führt man einen Schwellenwert $s$ ein, wird eine vom Nutzer unbeabsichtigte _Einzelradbewegung_ zuverlässiger unterdrückt:

$$
\begin{align}
(|vL| < s) \oplus (|vR| < s) 
\end{align}
$$

**Rotation um die eigene Achse**
Die Bedingung die gelten muss, wenn sich beide Räder gegeneinander drehen ist identisch mit der Bedingung, welche schon im Kapitel _Abbildung auf einen realistisch simulierten Rollstuhl_ aufgestellt wurde. Diese gilt nur, wenn _Ruhezustand_ und _Einzelradbewegung_ ausgeschlossen werden konnte, da nicht die Fälle abgedeckt werden, wenn $vL$ oder $vR$ 0 sind:

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
Werden, wie in Kapitel _Abbildung auf einen realistisch/idealisierten simulierten Rollstuhl_ erläutert, die Eingaben ausschließlich auf eine Rollstuhlbewegung abgebildet, so ist der Nutzer eingeschränkt in seinen Interaktionsmöglichkeiten. Aktionen wie einen Knopfdruck sind in diesen Fällen nicht möglich. Jedoch können bestimmte Bewegungsmuster, die nicht zwangsläufig notwendig sind, genutzt werden, um weitere Interaktionen abzubilden. Werden für das Drehen des Rollstuhls nur die Zustände genutzt, bei denen sich die Räder gegeneinander drehen (in Abb.<mark>?</mark> Zustand 4 und 6), so bleiben vier Zustände übrig, die mit anderen Interaktionen belegt werden können (in Abb.<mark>?</mark> Zustand 1, 3, 7, 9). Bei diesen vier Mustern handelt es sich, um die Einzelradbewegungs-Zustände. Diese können dann beispielsweise für das Drücken einer der vier <mark>Hauptknöpfe des Spielcontrollers</mark> genutzt werden.

Will man weitere Interaktionen abbilden, so ist dies nur noch möglich über die Kodierung der Radgeschwindigkeit durch den Nutzer. Entweder werden bestimmte Bewegungen der Räder unterschieden <mark>(Rad laufen lassen, Rad ruckartig bewegen und/oder über Bewegungen ähnlich zu Morsecode Information codieren)</mark> oder der Wertebereich wird geteilt mithilfe von Schwellwerten. Aufgrund des zeitlichen Rahmens dieser Arbeit wurde sich auf das Testen der zweiten Methode beschränkt.

Dazu wurde der Wertebereich zunächst mithilfe des Schwellenwertes geteilt. Von einer weiteren Teilung ist abzuraten, da es sonst für den Nutzer schwierig wird, die Räder mit den gewünschten beziehungsweise notwendigen Geschwindigkeiten zu drehen. Die Unterscheidung zwischen langsame Rotation und schnelle Rotation ist jedoch intuitiv von jedem Nutzer umsetzbar und verstehbar. Mit der Aufteilung in schnelle Geschwindigkeit und langsame Geschwindigkeit ist die Anzahl der Bewegungsmuster theoretisch verdoppelt worden. 
Im Wertebereich der langsamen Bewegungen können nun Bewegungen wie das Neigen des Kamerawinkels zusätzlich abgebildet werden. Dabei wurde sich für die Bewegungszustände 2 (Neigung nach oben) und 8 (Neigung nach unten) entschieden. Für das Detektieren dieses neuen Untergruppen-Bewegungszustandes wird folgende Bedingung benötigt:

**Neigen**

$$
\begin{align}
(|vL| < s) \land (|vR| < s) \land ((vL > 0) \Leftrightarrow (vR > 0))
\end{align}
$$

Unweigerlich geht dabei die Möglichkeit verloren, seinen <mark>Fortbewegungsvektor</mark> feiner einzustellen. Es sind also keine langsamen Bewegungen nach vorne und hinten möglich. Dafür hat der Nutzer jetzt die Möglichkeit, sich frei im Raum umschauen zu können.
Da sich jedoch herausgestellt hat, dass der Nutzer in vielen Anwendungen, ohnehin die maximale Fortbewegungsgeschwindigkeit anstrebt, ist diese Umbelegung der Interaktion sinnvoll. Aus diesem Grund wird bei dieser Abbildung bei jeder _schnellen Sichtachsenbewegung_ immer der maximale ThumbStick-Ausschlag abgebildet, um den Nutzer bei der Fortbewegung zu entlasten. Es ist nicht mehr notwendig, konstant mit der maximalen Geschwindigkeit die Räder zu drehen, damit man sich schnellstmöglich fortbewegt. 

In der Praxis hat sich gezeigt, dass nicht jeder zusätzliche Bewegungszustand für neue Interaktionen genutzt werden kann, da sonst die Präzision der Eingaben sich verschlechtert. Ein Beispiel dafür ist die Rotation um die eigene Achse. Wird die Rotation um die eigene Achse nur noch bei hohen oder niedrigen Geschwindigkeiten registriert, so ist es dem Nutzer entweder nicht mehr möglich sich schnell umzudrehen oder kleine Anpassungen seines Blickwinkels zu machen. Die tatsächlich sinnvolle Anzahl an Bewegungsmustern ist deshalb kleiner. Es muss bei jedem Zustand und jeder Interaktion abgewägt werden, ob eine Teilung des Wertebereichs sinnvoll ist.

___

## 5.5 System-Analyse
Um das vorher beschriebene System zu testen, wurden verschiedene Datenreihen gemessen. Im Folgenden sollen diese analysiert werden. 

### 5.5.1 Idealer Gyroskop-Modus
Als Erstes wird der Frage nach gegangen, in welchem Modus das Gyroskop betrieben werden sollte. Hierfür wurde eine Datenreihe gemessen, mit der Gradzahl pro Sekunde im Verlauf der Zeit. Eine Testperson hat dabei versucht, ein Rad so schnell wie möglich zu drehen. 

<mark>Bild einfügen
(Abb.? Gradzahl pro Sekunde im Verlauf der Zeit bei dem Testperson ein Rad so schnell wie möglich dreht)</mark>

Der Graph zeigt, dass der maximal erreicht Ausschlag um die $800$ beziehungsweise $-800$ herum ist. Daraus folgt, dass Gyroskop-Modus 2 für dieses Szenario der Ideale ist. Der Nutzer erreicht nicht die maximal Geschwindigkeit und reizt trotzdem den Wertebereich ca. $80\%$ aus. 
Jedoch hat sich beim Testen herausgestellt, dass es für den Nutzer in bestimmten Szenarien störend sein kann, wenn er nicht die maximale mögliche Geschwindigkeit mit niedriger Umdrehungszahl erreicht. Dies ist zwar nicht realistisch, da aber in vielen Anwendungen meist der maximale Ausschlag angestrebt wird, um sich fortzubewegen, ist es sehr anstrengend für den Nutzer lange viel Kraft und Energie in die Fortbewegung stecken zu müssen. Denkbar wäre hier eine neue Skalierung des Wertebereichs während des Mappings, damit schneller der maximale Wert erreicht wird, ohne einen Zahlenüberlauf zu riskieren, wenn man den Gyroskop-Modus stattdessen heruntersetzt.

### 5.5.2 Datenrate der Übertragungsprotokolle
Zunächst soll der Frage nachgegangen werden, ob hektische Bewegungen eines Rades zu einer schlechteren Übertragung der Daten führen, und ob schnellere Bewegungen dazu führe, obb weniger Daten-Pakete den Clienten erreichen.

<mark>Geplotteter Graph</mark>
<mark>Blablabal zu graph</mark>

Bei gewöhnlicher Nutzung des Systems ergeben sich folgende Werte bei der Übertragung: 

| Pakete pro Sekunde         | Durchschnitt | Minimum | Maximum | Delay |
| -------------------------- | ------------ | ------- | ------- | ----- |
| WiFi mit WebSocket         |              |         |         |       |
| ESP-Now mit seriellem Port |              |         |         |       |

Aus der Tabelle ist zu entnehme, dass im normalen Betrieb mindestens <mark>Zahl</mark> Daten-Pakete pro Sekunde erreicht werden. Es hat sich gezeigt, dass im hier untersuchten Echtzeit-Szenario die Datenrate ausreichen ist, um eine angenehme Nutzererfahrung zu generieren. 
<mark>blablabla</mark>

### 5.5.3 Detektieren von Bewegungszuständen
Für den Nutzer ist die korrekte Detektion von Bewegungszuständen entscheidend. Werden ungewünschte Zustände detektiert, führt dies zu fehlerhaften Eingaben, welche der Nutzer als störend empfindet. Beim Testen der _Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen_ haben sich zwei primäre Probleme herausgestellt, bei denen fehlerhafte Eingaben getätigt werden. 

**Unbeabsichtigtes Betätigen von Interaktionstasten**
In den ersten Testreihen wurde für die Detektion von einer _Einzelradbewegung_ und dem Teilen des Wertebereichs in schnelle und langsame Bewegungen derselbe Schwellwert verwendet. Unter Verwendung dieser Methode kommt es beim Anfahren oder Bremsen (_Sichtachsenbewegung_) zum unbeabsichtigten Betätigen von Interaktionstasten. Da sich die Räder nicht mit derselben Geschwindigkeit drehen, gibt es ein kurzes Zeitintervall, in dem ein Rad unter dem Schwellwert und ein Rad über dem Schwellwert liegt. Für dieses Zeitintervall gilt die Bedingung der _Einzelradbewegung_, sodass eine Interaktionstaste betätigt wird.

<mark>Plot mit Problem</mark>

Dieses Problem lässt sich über das Einführen eines neuen Schwellenwertes beheben. Wählt man für den Schwellenwert der _Einzelradbewegung_ einen geringeren Schwellwert $s_1$ als für den Schwellwert für das Teilen des Wertebereichs $s_2$, entsteht eine Pufferzone. Beim Beschleunigen überschreiten die Gyroskop-Werte zunächst nacheinander den Schwellenwert $s_1$. Anschließend überschreiten die Werte nacheinander den zweiten Schwellenwert $s_2$. Solange beide Werte in der Pufferzone sind, kann weder eine _Einzelradbewegung_ noch eine _Sichtachsenbewegung_ detektiert werden.

<mark>Plot mit Lösung</mark>

Durch das Einführen der neuen Schwellenwerte müssen folgende Untergruppen-Bewegungszustände erweitert werden:

**Einzelradbewegung**

$$
\begin{align}
((|vL| < s_1) \oplus (|vR| < s_1)) \land ((|vL| > s_2) \oplus (|vR| > s_2))
\end{align}
$$

**Sichtachsenbewegung**

$$
\begin{align}
((vL > 0) \Leftrightarrow (vR > 0)) \land ((vL > s_2) \land (vR > s_2))
\end{align}
$$

**Unbeabsichtigtes Neigen beim Anfahren**
Beim Anfahren oder Bremsen (_Sichtachsenbewegung_) wurde beobachtet, dass für ein kurzes Zeitintervall der Kamerawinkel unbeabsichtigt geneigt wird. Ähnlich wie beim vorangegangenen Problem wird auch hier beim Übergang von einem Zustand zum Nächsten, ein unerwünschter Zwischenzustand erreicht.

<mark>Plot mit Problem</mark>

Da die Fehldetektion immer dann auftritt, wenn sich die Geschwindigkeit der Räder ändert, ist ein Lösungsansatz immer dann in den Ruhemodus (Bewegungszustand 5) zu wechseln, wenn die Änderungsrate $a$ der Gyroskop-Werte einen vorher definierten Schwellenwert $s_3$ überschreitet. Somit werden beim Übergang von einem Zustand in den nächsten, im Zeitfenster des Übergangs alle anderen Untergruppen-Bewegungszustände unterdrückt. Haben die Räder ihre Zielgeschwindigkeit erreicht, fällt die Änderungsrate unter den Schwellenwert, sodass der nächste korrekte Zustand detektiert werden kann. Jedoch ist anzumerken, dass diese Methode vor allem bei ruckartigen Bewegungen funktioniert, da besonders dann die Änderungsrate ein gut registrierbaren Ausschlag hat. Für die Berechnung der Änderungsrate wird folgende Berechnung verwendet:

$$
\begin{align}
a = |(vL_{[1]} - vL_{[0]})| + |((vR_{[1]} - vR_{[0]})| > s_3
\end{align}
$$

Der _Ruhezustand_ muss wie folgt ergänzt werden:

$$
\begin{align}
(vL = 0) \land (vR = 0) \land (a > s_3)
\end{align}
$$

<mark>Plot mit Problem</mark>

In den Daten ist zu erkennen, dass zwischen dem _Sichtachsenbewegung_-Zustand und dem _Neigen_-Zustand ein kurzer Ruhezustand exisitert. Dieser ist vom Nutzer wahrnehmbar, da diese Methode in eine leichten Verzögerung der Eingabe resultiert. Sie ist jedoch nicht so groß, dass die Verzögerung als zu störend empfunden wird.   

___
