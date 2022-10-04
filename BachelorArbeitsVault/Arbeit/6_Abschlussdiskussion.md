# 4. Abschlussdiskussion
___
In dieser Arbeit wurde sich zum Ziel gesetzt, ein kostengünstiges System zu entwickeln, mithilfe es einem Nutzer möglich ist, einen Rollstuhl als Eingabegerät an seinem Heimcomputer zu nutzen.
Dazu musste ein eingebettetes System entworfen werden, welches die Rotationsdaten der Räder misst und an eine auf dem Heimcomputer laufende Rollstuhl-Software übermittelt.
Die Software soll dann die Eingaben des Nutzers auf dem Rollstuhl, auf Eingaben eines Spielcontrollers abbilden, um so fremde Software steuern zu können.

In der vorliegenden Arbeit konnte das zum Ziel gesetzte System erfolgreich entwickelt werden.
Das eingebettete System ist in der Lage, die Rotation der Räder des Rollstuhls mithilfe des Gyroskops MPU-6050 präzise zu messen, ohne dass der Nutzer eine hardwarebedingte maximale Geschwindigkeit zu erreicht.
Für die Übermittlung der Daten haben sich die beiden getesteten Übertragungsprotokolle WiFi und ESP-Now als den Anforderungen entsprechend erwiesen.
Sie erreichen beide ausreichende Bandbreiten, sodass flüssige Eingaben möglich sind.
Die Verbindung mit WiFi ist im Vergleich jedoch ein wenig stabiler.
Es konnten Verfahren entwickelt werden, mit denen die empfangenen Daten auf Eingaben eines Spielcontrollers abgebildet werden konnten.
So ist es dem Nutzer möglich, in einem dreidimensionalen Raum sich fortzubewegen, frei umzuschauen und vier Aktionstasten zu betätigen, durch gezielte Drehungen der Räder.
Dabei konnten vom Nutzer unerwünschte Eingaben, sprich Abweichungen von der eigentlichen Eingabe, minimiert werden.

Es ist jedoch anzuzweifeln, ob die Anzahl möglicher Eingaben ausreichend ist, um in komplexen virtuellen Welten zu navigieren.
Viele Anwendungen erfordern wesentlich mehr unterschiedliche Eingaben, sodass das entwickelte System in seinen Möglichkeiten hier an seine Grenzen stößt.
Darüber hinaus ist kritisch zu betrachten, dass die gemessenen Daten nicht repräsentativ sind.
Es konnten nur wenige Messreihen erstellt werden und beim Testen der Übertragungsprotokolle gab es lediglich eine Testumgebung.
Die Wahrnehmung des Nutzers ist zudem subjektiv und da nur wenige Testpersonen zur Verfügung standen, ist diese nur begrenzt aussagekräftig.
Es bedarf weiterer Forschung, um belastbare Aussagen über die Wirkung auf den Nutzer treffen zu können.

Das entwickelte System ist denkbar als günstigere Alternative des von Crichlow et al. entwickelten Systems, welche beschrieben ist im Paper \textit{A full motion manual  
wheelchair simulator for rehabilitation research}\cite{crichlowFullMotionManual2011}.
So wäre eine mögliche Anwendung im Bereich der Städte oder Gebäudeplanung denkbar, auch wenn auf Features wie die Krafteinwirkung auf den Rollstuhl verzichtet werden müsste.
Auch als Eingabegerät in anderen Anwendungen wie beispielsweise Computerspielen ist das entwickelte System denkbar.
Zukünftige Arbeiten könnten sich mit der Implementierung weiterer Abbildungen widmen.
So wären ergänzende Abbildungen auf einem Spielcontroller oder auch einem anderen Eingabegerät denkbar.
Ein zu nennendes Beispiel ist die Computer-Maus, die mit zwei Achsen, zwei Haupt-Tasten und einem Mausrad ohne Probleme abzubilden wäre.
Ergänzt durch Technologien wie dem Eye-Tracking oder anderen Eingaben, die mit dem Kopf getätigt werden, könnte so die Anzahl an unterschiedlichen Interaktionen erhöht werden, um den zuvor kritisierten Mangel an diesen zu verbessern.
Die Verwendung des Rollstuhls als Mittel zur Fortbewegung in virtuellen Realitäten ist ebenfalls ein Anwendungsgebiet.
\ac{vr}-Nutzer könnten von einer weniger eingeschränkten Fortbewegung profitieren, da der Rollstuhl stationär, mit durchdrehenden Reifen, keinen großen Raum benötigt.
Ferner könnte die Software um die Unterstützung von Bluetooth als Verbindungsprotokoll ergänzt werden.
Auch die Entwicklung einer \ac{gui}, zur einfacheren Einstellung der Rollstuhl-Software ist vorstellbar.






