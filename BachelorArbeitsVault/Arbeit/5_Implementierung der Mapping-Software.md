# 5. Implementierung der Mapping-Software
___
```toc
```

In diesem Kapitel werden die Mittel und Wege erörtert, wie das zu entwickelnde System dieser Arbeit entworfen wurde. Dazu wird zunächst darauf eingegangen, wie das benötigte eingebettete System designt wurde. Anschließend wird sich der Frage gewidmet, wie die gemessenen Daten mithilfe einer Software, weiterverarbeitet beziehungsweise abgebildet werden, zu Eingaben auf einem Spielcontroller. 
<mark>Zuletzt wird untersucht, wie gut die entwickelten Systeme funktionieren und wie diese verbessert werden können.</mark>

## 5.1 Interface zur Nutzung der Rad-Daten in externer Software
Die Raddaten, welche die Rollstuhl-Software empfangen hat, müssen nun zu externer Software gelangen. Dazu ist eine vorhandene Schnittstelle notwendig (Tastatur, Maus, Spielcontroller, ...), denn es kann nicht davon ausgegangen werden, dass jede externe Software eine neue Schnittstelle implementiert. Jedoch muss in diesem Fall, in der Rollstuhl-Software eine Abbildung auf eine vorhandene Schnittstelle durchgeführt werden.

### 5.1.1 Tastatur oder Spielcontroller
Tastaturen und Spielcontroller werden von den meisten Anwendungen unterstützt und eignen sich unterschiedlich gut für die Zwecke des hier entwickelten Systems. Tastatureingaben bieten den Vorteil, dass sie von fast jeder erdenklichen Software unterstützt werden. Jedoch lassen nur binäre Eingaben, durch das Drücken von Tasten tätigen. Da die Rollstuhl-Eingaben jedoch unterschiedliche Werte innerhalb eines Wertebereichs darstellen, wird bei einer Tastatureingabe die Interaktionsmöglichkeit stark eingeschränkt. Zudem ist oft das zusätzliche Verwenden einer Maus erforderlich. Dies verkompliziert das Abbilden zusätzlich. 

Die Alternative ist ein Mapping auf eine Spielcontroller-Eingabe. Hier gibt es ebenfalls Knöpfe, beziehungsweise binäre Eingaben, aber auch Eingaben entlang von Achsen innerhalb eines Wertebereichs. Überwiegend werden die Achsen in Form eines Thumb-Sticks oder Knopfes mit mehreren Stufen realisiert. Auf der einen Seite könnten so Rollstuhl-Eingaben, wie das Fortbewegen, einfacher auf das Gerät abgebildet werden. Andererseites unterstützt nicht jede Software Eingaben mittels eines Spielcontrollers. 

Es wurde sich im Rahmen dieser Arbeit für das Abbilden auf, und emulieren von, einem Spielcontroller entschieden. Grund dafür ist, dass die meiste Software in der Fortbewegung eine Rolle spielt (meist Computerspiele oder andere 3D-Räume), diese unterstützen.

### 5.1.2 Virtual Gamepad Emulation Framework
Um die Eingaben des Rollstuhls in tatsächliche Spielcontroller-Eingaben umzuwandeln, die vom Betriebssystem auch als Controller-Eingabe verstanden werden, ist eine Emulation eines Controllers notwendig. Ziel ist es, programmgesteuert Controller-Eingaben an den Rechner zu senden. Um sich den Aufwand des Schreibens eines neuen Treibers zu ersparen, wird an dieser Stelle auf das _Virtual Gamepad Emulation Framework_ zurückgegriffen. Dies ist eine Bibliothek, welche in bestehende Software integriert werden kann und einen virtuellen Controller mit dem Rechner verbindet. Über Befehle lassen sich anschließend Controller-Eingaben tätigen. Das Framework unterstützt Xbox 360-, sowie DualShock 4-Controller [[@VirtualGamepadEmulationa]].

Eine von der Internet-Vertriebsplattform Steam, welche hauptsächlich Computerspiele vertreibt, veröffentlichte Umfrage hat ergeben, dass $45\ \%$ der Nutzer auf ihrer Plattform über einen Xbox 360 Controller verfügen, sowie $19\ \%$ über das neuere Xbox One Modell, welches seinem Vorgänger sehr ähnlich ist [[@SteamSteamNews2018]]. Damit sind Xbox Controller mit großem Abstand am verbreitetsten. Aufgrund dieser Annahme wurde sich im Rahmen der vorliegenden Arbeit für die Emulation eines Xbox 360 Controllers entschieden.

![[steamControllerStatistik.jpg|600]]
(Abb.<mark>?</mark>, Verteilung von Besitz von verschiedenen Spielcontrollern auf der Plattform Steam)

## 5.2 Algorithmen zur Abbildung der Raddaten in Controller-Eingaben
Die Sensor-Daten der Gyroskope liefern die Winkelgeschwindigkeiten der Räder des Rollstuhls. Diese sollen – wie bereits in Kapitel 5.3.1 beschrieben – auf die Eingabemöglichkeiten eines Spielcontrollers abgebildet werden, um sich im virtuellen Raum bewegen oder andere Eingaben tätigen zu können. Die Abbildung erfolgt dabei auf einen Xbox360 Controller. Somit sind die abgebildeten Eingaben von jeder Software lesbar, die eine Xbox360 Controllerunterstützung implementiert haben.

### 5.2.1 Abbildung auf einen Thumbstick
Der direkte Weg die Raddaten in eine Eingabe umzuwandeln, ist diese auf jeweils eine Achse eines Thumbsticks abzubilden. Dabei wird die x-Achse mit dem einen, die y-Achse mit dem anderen Rad dargestellt. Vorteilig ist dabei, dass beide Achsen gleichzeitig angesprochen werden können. Jedoch ist es schwieriger, die x-Achse zu bewegen, da sie anders ausgerichtet ist als das Rad, das gedreht wird. Alternativ kann das Ansprechen einer Achse auch durch beide Räder passieren. Dabei wird die x-Achse dann angesprochen, wenn sich die Räder gegeneinander drehen und die y-Achse, wenn sich die Räder miteinander drehen. Damit wird eine intuitive Nutzung angestrebt. Jedoch ist es dabei nicht mehr möglich, gleichzeitig den Cursor entlang beider Achsen zu bewegen, da sich die Räder nicht gleichzeitig mit- und gegeneinander drehen können.

### 5.2.2 Abbildung auf einen simulierten Rollstuhl
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

### 5.2.4 Bewegungszustände
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


### 5.2.5 Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen
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

## 5.3 Optimierung der Detektion von Bewegungszuständen
Für den Nutzer ist die korrekte Detektion von Bewegungszuständen entscheidend. Werden unerwünschte Zustände ermittelt, führt dies zu fehlerhaften Eingaben, welche der Nutzer als störend empfindet. Je mehr verschiedene Bewegungszustände voneinander unterschieden werden müssen, desto höher ist die Gefahr der Missinterpretation. Abgesehen davon ist es nicht immer sinnvoll für alle Bewegungsmuster den Wertebereich zu teilen (wie in Kapitel _5.4.5 Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen_). Bewegungen, wie die _Rotation um die eigene Achse_, wollen vom Nutzer entweder langsam und präzise oder schnell durchgeführt werden. Nur in bestimmten Fällen, wie bei der Fortbewegung, kann es sinnvoll sein, den Wertebereich zu teilen. So ist die Anzahl der tatsächlich sinnvollen Bewegungsmuster kleiner als die theoretisch denkbaren. Es muss bei jedem Zustand und jeder Interaktion abgewogen werden, ob eine Teilung des Wertebereichs sinnvoll ist, oder den Nutzer behindert. Trotz der verringerten Anzahl an Bewegungsmustern, sind beim Testen der _Abbildung auf einen simulierten Rollstuhl mit zusätzlichen Interaktionen_ zwei primäre Probleme beobachtet worden, bei denen fehlerhafte Eingaben getätigt werden. 

### 5.3.1 Unbeabsichtigtes Betätigen von Interaktionstasten
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

### 5.3.2 Unbeabsichtigtes Neigen beim Anfahren
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