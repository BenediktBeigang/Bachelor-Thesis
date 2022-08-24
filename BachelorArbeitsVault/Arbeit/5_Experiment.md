# 5. Experiment
___
```toc
```

## 5.1 Eingebettetes System zur Messung der Raddaten
Damit der Software bekannt ist, mit welcher Geschwindigkeit sich welches Rad in welche Richtung dreht, ist Hardware notwendig, welche die Rotationsdaten misst und an die Software übermittelt. Dazu wurde für das vorliegende Experiment ein eingebettetes System verwendet. Im Folgenden soll näher beleuchtet und erörtert werden, welche Hardware verwendet und wie die Raddaten gemessen und übertragen wurden. Dazu werden im Folgenden verschiedene Kommunikationsprotokolle verglichen.

### 5.1.1 Gyroskop
Um die Rotation der Räder des Rollstuhls messen zu können, wird ein Sensor benötigt. Dabei gibt es verschiedene Herangehensweisen, wie ein Sensor die Rotation messen kann. 
So arbeiten viele Sensoren mit Lichtschranken. Hierbei wird die Lichtschranke in regelmäßigen Abständen durch ein Hindernis blockiert. Daraus kann dann über die Frequenz, in der dies geschieht, eine Geschwindigkeit errechnet werden. Vorteil ist dabei, dass keine Hardware auf dem Rad befestigt werden muss und deshalb kein mobiles System benötigt wird. Es kann auf Technologien wie WiFi und Akkus verzichtet werden.
Ein großer Nachteil bei diesem Verfahren ist jedoch XYZ, dass für besonders kleine Rotationen die Abstände der Hindernisse sehr klein sein müssen. Angewandt auf den Rollstuhl bedeutet dies, dass eine zusätzliche Konstruktion gebaut werden muss, damit die Lichtschranke unterbrochen wird. Da dies sehr unpraktikabel ist, wurde dieses Verfahren nicht verwendet.
Die zweite Möglichkeit ist die Verwendung eines Gyroskops. Dieses kommt ohne zusätzliche Konstruktionen aus, erfordert jedoch, dass die Elektronik mobil ist. Die Datenrate wird folglich durch die Bandbreite des drahtlosen Netzwerks begrenzt, da die Übertragung von Daten den Flaschenhals solcher Systeme darstellt.

### 5.1.2 MPU-6050
Im Zuge dieser Arbeit habe ich mich für den <mark>MPU-6050</mark> entschieden. 
Dieser ist klein (mit Pins: 20mm x 15mm x 11mm), kostengünstig zu erwerben (~4€) und verfügt unter anderem über 3-Achsen Gyroskop-Sensoren, mit welchen die Rotation gemessen werden kann. 
Der Chip besitzt folgende 8 Anschlüsse:

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

Die Daten können per <mark>I2C</mark> von einem angeschlossenen <mark>Mikrocontroller ausgelesen</mark> werden. Jede Achse wird auf zwei 8-Bit Register abgebildet. Zusammen ergibt das einen Wertebereich von 65.536 unterscheidbaren Zuständen. Mit der Drehrichtung halbiert sich dieser Wertebereich, <mark>da ein Bit für das Verschieben des Wertebereichs ins Negative benötigt wird.</mark>
Das Gyroskop des MPU-6050 kann in vier verschiedenen Konfigurationen betrieben werden. Damit wird festgelegt, wie klein der Winkel zwischen zwei verschiedenen Ausgaben ist; mit anderen Worten: wie viele Stufen pro Grad unterschieden werden können. Da der Wertebereich konstant ist, bedeutet eine sensiblere Messung, <mark>dass das Gyroskop früher das Ende des Wertebereichs erreicht.</mark> Angewendet auf den Rollstuhl heißt das, dass das rotierende Rad bei niedrigeren Geschwindigkeiten seine maximal messbare Geschwindigkeit erreicht. In der folgenden Tabelle sind alle Konfigurationen mit ihren resultierenden Eigenschaften aufgelistet.

| Modus | Maximale Gradzahl pro Sekunde | Stufen pro Grad | Maximale Umdrehungszahl pro Sekunde | Maximale Radianten pro Sekunde | Zurückgelegte Distanz pro Stufe in mm* |
| ----- | ----------------------------- | --------------- | ----------------------------------- | ------------------------------ | -------------------------------------- |
| 0     | 250                           | 131             | 0,69                                | 4,36                           | 0,04                                   |
| 1     | 500                           | 65,5            | 1,39                                | 8,73                           | 0,08                                   |
| 2     | 1000                          | 32,8            | 2,78                                | 17,47                          | 0,16                                   |
| 3     | 2000                          | 16,4            | 5,56                                | 34,93                          | 0,32                                   |
\*Werte bei einem Raddurchmesser von 60 cm.

An dieser Stelle stellt sich die Frage welcher Modus für das hier entwickelte System das Richtige ist. Auf der einen Seite möchte der Nutzer ein möglichst <mark>ruckelfreies</mark> Erlebnis haben. Um dies zu gewährleisten muss das Gyroskop so senstiv wie möglich eingestellt werden und der Wertebereich möglichst weit ausgereizt ist. Ist der Modus nicht sensitiv genug so bemerkt der Nutzer möglicherweise das Springen der Bitwerte in Form eines Vorspringens der Bewegung. Jedoch soll der Nutzer nicht schneller als die maximale Gradzahl pro Sekunde drehen können, da es sonst zu einem Zahlenüberlauf kommt und zu einer fehlerhaften Weiterverarbeitung der Daten führt. Der gewählte Modus muss also möglichst niedrig sein, sollange der Nutzer nicht in der Lage ist die maximale Gradzahl pro Sekunde zu erreichen. Im Kapitel System-Analyse wird dieser Frage weiter nachgegangen.

### 5.1.3 ESP32
Um den MPU-6050 betreiben und dessen Daten an eine Software übermitteln zu können, wird ein Mikrocontroller-Board benötigt. Es muss per I2C die entsprechenden Register auslesen und mittels drahtloser Kommunikation versenden. Außerdem muss er das Gyroskop, sowie sich selbst mit Strom versorgen. Auf dem Markt gibt es eine große Anzahl von Produkten, für die verschiedensten Anwendungsgebiete und <mark>mit den verschiedensten Features.</mark> Im Rahmen dieser Arbeit wurde der Mikrocontroller ESP32 verwendet, das aktuellste Modell der Firma _Espressif_. Boards mit diesem Chip sind kostengünstig (~8€) und ermöglichen ein unkompliziertes Arbeiten mit der Hardware, da der Chip verbreitet ist und viel Literatur und Anleitungen existieren. Ausgestattet ist der ESP32 mit WiFi und Bluetooth Unterstützung, Espressif bietet aber auch ein eigenes Verbindungsprotokoll an: _ESP-Now_. Verbaut wurde ein Xtensa® 32-bit LX6 Mikroprozessor, mit 240MHz Taktfrequenz, 448 KB ROM und 520 KB SRAM. [[@ESP32Datasheet2022]]
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
Damit das bis hier entwickelte System die transformierten Raddaten beziehungsweise Interaktionen in externer Software nutzen kann, ist eine Verbindung zwischen Rollstuhl-Software und der externen Software notwendig. Da man nicht davon ausgehen kann, dass jede externe Software eine Schnittstelle implementiert, mit der die beiden Softwares kommunizieren können, müssen die Daten des Rollstuhls auf herkömmliche Eingabegeräte gemappt/abgebildet werden. 

### 5.3.1 Tastatur oder Spielcontroller
Dabei kommen zwei Möglichkeiten infrage: eine Tastatureingabe und eine Spielcontrollereingabe. Beide Eingaben werden von den meisten Anwendungen unterstützt und eigenen sich unterschiedlich gut für die Zwecke des hier entwickelten Systems. Tastatureingaben bieten den Vorteil, dass sie von fast jeder erdenklichen Software unterstützt werden. Jedoch lassen sich damit nur binäre Eingaben tätigen, sprich es lassen sich nur Tasten drücken. Da die Rollstuhl-Eingaben jedoch unterschiedliche Werte innerhalb eines Wertebereichs darstellen, wird bei einer Tastatureingabe die Interaktionsmöglichkeit stark eingeschränkt. 
Die Alternative ist eine Spielcontroller-Eingabe. Hier gibt es ebenfalls Knöpfe, beziehungsweise binäre Eingaben, jedoch auch Eingaben entlang von Achsen innerhalb eines Wertebereichs. Überwiegend werden die Achsen in Form eines Thumb-Sticks oder Knopfes mit mehreren Stufen realisiert. Rollstuhl-Eingaben wie das Fortbewegen könnte so einfacher auf das Gerät abgebildet werden. Allerdings unterstützt nicht jede Software Eingaben mittels eines Spielcontrollers. Da jedoch die meiste Software in der Fortbewegung eine Rolle spielt, Spielcontroller unterstützt, wurde sich im Rahmen dieser Arbeit für diese Eingabeform entschieden.

### 5.3.2 Virtual Gamepad Emulation Framework
Um die Eingaben des Rollstuhls in tatsächliche Spielcontroller-Eingaben umzuwandeln, die von einem <mark>Rechner</mark> auch als Controller-Eingabe verstanden werden, ist eine Emulation eines Controllers notwendig. Ziel ist es, programmatisch Controller-Eingaben an den Rechner zu senden. Um sich den Aufwand des Schreibens eines neuen Treibers zu ersparen, wird an dieser Stelle auf das _Virtual Gamepad Emulation Framework_ zurückgegriffen. Dies ist eine Bibliothek, welche in bestehende Software integriert werden kann und einen virtuellen Controller mit dem Rechner verbindet. Über Befehle lassen sich anschließend Controller-Eingaben tätigen. Das Framework unterstützt Xbox 360-, sowie DualShock 4-Controller.

Eine von der Internet-Vertriebsplattform Steam, welche hauptsächlich Computerspiele vertreibt, veröffentlichte Umfrage hat ergeben, dass $45\%$ der Nutzer auf ihrer Plattform über einen Xbox 360 Controller verfügen, sowie $19\%$ über das neuere Xbox One Modell, welches seinem Vorgänger sehr ähnlich ist. 

![[steamControllerStatistik.jpg|600]]
(Abb.<mark>?</mark>, Verteilung von Besitz von verschiedenen Spielcontrollern auf der Plattform Steam)

Damit sind Xbox Controller mit großem Abstand am verbreitetsten. Es ist davon auszugehen, dass Software im Allgemeinen am wahrscheinlichsten einen Xbox Controller unterstützt, um möglichst vielen Kunden das Nutzen eines Controllers zu ermöglichen. <mark>Quelle</mark>
Aufgrund dieser Annahme wurde sich für die Emulation eines Xbox 360 Controllers entschieden.

___

## 5.4 Algorithmen zur Abbildung der Raddaten in Eingaben
Die Sensor-Daten der Gyroskope liefern die Winkelgeschwindigkeiten der Räder des Rollstuhls. Es sollen <mark>verschiedene Abbildungen auf Eingaben</mark> getestet werden, um sich im virtuellen Raum bewegen zu können oder andere Eingaben tätigen zu können. Die Abbildung erfolgt dabei, wie im vorangegangenen Kapitel evaluiert auf einen Xbox360 Controller. Somit sind die abgebildeten Eingaben von jeder Software lesbar, die eine Controllerunterstützung dieses Controllers implementiert hat.

### 5.4.1 Abbildung auf einen Cursor
Die einfachste Art und Weise, wie die Raddaten in Eingaben abgebildet werden können, ist die Steuerung eines Cursors. Dabei wird ein Rad genutzt, um die x-Achse abzubilden und das andere Rad bildet die y-Achse ab. Vorteil dabei ist, dass beide Achsen gleichzeitig angesprochen werden können. <mark>Jedoch ist es schwieriger, die x-Achse zu bewegen, da diese anders ausgerichtet ist als das Rad, das gedreht wird.</mark> 

<mark>Eine Alternative ist, dass jede Achse von beiden Rädern gesteuert wird. Die x-Achse wird dabei dann angesprochen, wenn sich die Räder gegeneinander drehen. Drehen sich die Räder miteinander, so wird die y-Achse angesprochen. Jedoch ist es dabei nicht mehr möglich, gleichzeitig den Cursor entlang beider Achsen zu bewegen, da sich die Räder nicht gleichzeitig mit und gegeneinander drehen können.</mark>

### 5.4.2 Abbildung auf einen realistisch simulierten Rollstuhl
Da das im Rahmen dieser Arbeit entwickelte System darauf abzielt im virtuellen Raum zu navigieren, wird eine Abbildung benötigt, die die Position des Nutzers im virtuellen Raum verändert. Die naheliegendste Methode ist dabei die Abbildung auf einen simulierten Rollstuhl, da die Daten ursprünglich von einem realen Rollstuhl gekommen sind und die Bewegungsmuster einfach aufeinander abgebildet werden können. Um die Raddaten der zwei Räder auf eine Bewegung und Rotation eines Rollstuhls umzurechnen, muss erst festgestellt werden, welche Radbewegungen zu welchen Rollstuhlbewegungen führt. Dabei können drei idealisierte Fälle unterschieden werden:

**Fall 1:** Drehen sich die Räder mit gleicher Geschwindigkeit in dieselbe Richtung, so ruft dies eine Bewegung nach vorne oder hinten aus.
**Fall 2:** Drehen sich die Räder mit gleicher Geschwindigkeit gegeneinander, so ruft dies eine Rotation um die eigene Achse hervor.
**Fall 3:** Dreht sich nur ein Rad, so dreht sich dieses um das Stehende.

Im Folgenden wird die Berechnung der Bewegungsanteile aufgezeigt, bestehend aus Bewegung nach vorne/hinten und Rotation um die eigene Achse:
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
(Abb.<mark>?</mark> Skizze eines Rollstuhls, mit den relevanten Größen)

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
$$m = min(\left| vL \right|-\left| vR\right|)$$
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
Um die Rotationskomponente $r_3$ berechnen zu können muss $Θ$ mit $360$ multipliziert werden.
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
Bei der Verwendung der Abbildung hin zu einem realistischen Rollstuhl hat sich gezeigt, dass ein schnelles Vorankommen gestört wird. Ursache dafür ist, dass die Räder sich in leicht unterschiedlicher Geschwindigkeit drehen und so automatisch die Bewegung nach vorne <mark>Drall</mark> nach links oder rechts bekommt. 

Dieses Problem kann gelöst werden, wenn mit den Rotationen beider Räder ein Mittelwert $v$ errechnet wird der für die Kalkulation von Fall 1 und Fall 2 statt dem Minimum $m$ verwendet wird. 
$$
\begin{align}
v = \frac {(vL + vR)} {2} 
\end{align}
$$
Jedoch müssen die Fälle in diesem Fall distinkt sein, da sonst das Wenden mit einem Rad nicht mehr möglich wäre. Grund dafür ist, dass die Geschwindigkeiten des stillstehende Rades und das sich Drehenden zu einem Mittelwert zusammengerechnet werden würde und der Rollstuhl sich nur nach vorne bewegt, statt sich zu drehen. Das Zusammenrechnen von Fall 1 oder Fall 2 mit Fall 3 darf also nicht geschehen. Realisiert man die Bewegung des Rollstuhls indem der Rollstuhl in verschiedenen Bewegungszuständen sein kann, so kann die Bewegung für jeden Fall einzeln richtig errechnet werden.

>Um dieses Problem zu lösen, können die drei verschiedenen Fälle als distinkt angenommen werden. 
>So ist die Abbildungskalkulation in einem von drei Zuständen, die die Fälle repräsentieren. Für Fall 1 und Fall 2 kann statt der Verwendung des Minimums $m$ stattdessen ein interpolierter Wert $v$ verwendet werden.

### 5.4.4 Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen
Werden wie oben erläutert die Eingaben direkt auf eine Rollstuhlbewegung abgebildet, so ist der Nutzer eingeschränkt in seinen Interaktionsmöglichkeiten. Aktionen wie einen Knopfdruck sind in diesen Fällen nicht möglich. Jedoch können bestimmte Bewegungsmuster, die nicht zwangsläufig notwendig sind, weggelassen werden, um Interaktionen wie zum Beispiel das Drücken eines Knopfes abzubilden. Um diese Bewegungsmuster erkennen zu können, muss die Bewegung der Räder als diskret verstanden werden. So kann sich ein Rad in drei Zuständen befinden: Still stehend, nach vorne drehend und nach hinten drehend. Zwei Räder mit jeweils drei Zuständen ergibt dabei $3^2 = 9$ Bewegungsmuster.

![[WheelchairStates.PNG|500]]
(Abb.3, Bewegungszustände eines Rollstuhls)

Werden für das Drehen des Rollstuhls nur die Zustände genutzt, bei denen sich die Räder gegeneinander drehen (in Abb.3 zweite Reihe, erste und dritte Spalte), so bleiben vier Zustände übrig, die mit anderen Interaktionen belegt werden können. Bei diesen vier Mustern handelt es sich, um solche, bei denen ein Rad still steht. Diese können dann beispielsweise für das Drücken einer der vier <mark>Hauptknöpfe des Spielcontrollers</mark> genutzt werden. 

Will man weitere Interaktionen abbilden, so ist dies nur noch möglich über das geziehlte Drehen der Räder in einer bestimmten Geschwindigkeit durch den Nutzer. Wird beispielsweise der Wertebereich der Raddaten geteilt, so könnten schnelle Bewegungen anders interpretiert werden als langsame Bewegungen. In diesem Szenario lässt sich so die Anzahl der Bewegungsmuster verdoppeln.
Ein Ansatz kann es dann sein, bei langsamen Bewegungen die Rotation nicht auf eine Drehung auf der Stelle zu mappen, sondern stattdessen auf eine Neigung des Kopfes. Somit wäre der Nutzer in der Lage, sich frei im Raum umschauen zu können.

___

## 5.5 System-Analyse
Um das vorher beschriebende System zu testen, wurden verschiedene Datenreihen gemessen. Im Folgenden sollen diese analysiert werden. 

### 5.5.1 Idealer Gyroskop-Modus
Als erstes wird der Frage nach gegangen in welchem Modus das Gyroskop betrieben werden sollte. Hierfür wurde eine Datenreihe gemessen mit der Gradzahl pro Sekunde im Verlauf der Zeit. Eine Testperson hat dabei versucht ein Rad so schnell wie möglich zu drehen. 

<mark>Bild einfügen
(Abb.? Gradzahl pro Sekunde im Verlauf der Zeit bei dem Testperson ein Rad so schnell wie möglich dreht)</mark>

Der Graph zeigt, dass der maximal erreicht Ausschlag um die $800$ beziehungsweise $-800$ herum ist. Daraus folgt dass Gyroskop-Modus 2 für dieses Szenario der ideale ist. Der Nutzer erreicht nicht die maximal Geschwindigkeit und reizt trotzdem den Wertebereich ca. $80\%$ aus. 
Jedoch hat sich beim Testen herausgestellt, dass es für den Nutzer in bestimmten Szenarien störend sein kann wenn er nicht die maximale mögliche Geschwindkeit mit niedriger Umdrehungszahl erreicht. Dies ist zwar nicht realistisch, da aber in vielen Anwendungen meist der maximale Ausschlag angestrebt wird um sich fortzugebewegen, ist es sehr anstrengend für den Nutzer lange viel Kraft und Energie in die Fortbewegung stecken zu müssen. Denkbar wäre hier eine neue Skaliereung des Wertebereichts während des Mappings, damit schneller der maximale Wert erreicht wird, ohne einen Zahlenüberlauf zu riskieren, wenn man den Gyromodus stattdessen runtersetzt.

### 5.5.2 Datenrate der Übertragungsprotokolle
Zunächst soll der Frage nachgegangen werden ob hektische Bewegungen eines Rades zu einer schlechteren Übertragung der Daten führen, und ob schnellere Bewegungen dazu führen ob weniger Daten-Pakete den Clienten erreichen.

<mark>Geplotteter Graph</mark>
<mark>Blablabal zu graph</mark>

Bei gewöhnlicher Nutzung des Systems ergeben sich folgende Werte bei der Übertragung: 

| Pakete pro Sekunde         | Durchschnitt | Minimum | Maximum | Delay |
| -------------------------- | ------------ | ------- | ------- | ----- |
| WiFi mit WebSocket         |              |         |         |       |
| ESP-Now mit seriellem Port |              |         |         |       |

Aus der Tabelle ist zu entnehmen das im normalen Betrieb mindestens <mark>Zahl</mark> Daten-Packete pro Sekunde erreicht werden. Es hat sich gezeigt, dass im hier untersuchten Echtzeit-Szenario die Datenrate ausreichen ist um eine angenehme Nutzererfahrung zu generieren. 
<mark>blablabla</mark>

### 5.5.3 
- verschiedene Bewegungen plotten 
	- Knopf drücken im Vergleich zu Bewegung
	- Neigen und bewegen (Threshold oder filter?)
- Kommunikation vergleichen 
___

## 5.6 Anwendbarkeit in Software

___
