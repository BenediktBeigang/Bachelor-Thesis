# 3. Begriffsklärung
___
Im Folgenden sollen nicht triviale Begriffe erklärt werden die für das Verständis dieser Arbeit notwendig sind.

## Eingebettetes System
Ein eingebettetes System besteht aus einem programmierbaren Mikroprozessor und wird meistens umgeben von Sensoren und Aktoren. Das gesamte System bildet damit eine Schnittstelle zwischen physikalischen Prozessen und einem elektrischen Gerät.

Marwedel definiert ein eingebettetes System wie folgt: 
> „Eingebettete Systeme sind informationsverarbeitende Systeme, die in umgebende Produkte integriert sind.“
> (S. 2) [[@marwedelEingebetteteSystemeGrundlagen2021]]

Gessler definiert eingebettete Systeme wie folgt:
>„Das Eingebettete Systeme sind Rechenmaschinen, die in elektrischen Geräten „eingebettet“ sind, z. B. in Kaffeemaschinen, CD-, DVD-Spielern oder Mobiltelefonen. Unter eingebetteten Systemen verstehen wir alle Rechensysteme außer den Desktop-Computern.“
>(S. 7) [[@gesslerEntwicklungEingebetteterSysteme2014]]

Da für das Steuern von Aktoren und auslesen von Sensoren, sowie das Empfangen und Übertragen von Daten aus heutiger Sicht keine große Rechenleistung benötigt wird, sind die verwendeten Mikroprozessoren verglichen mit modernen Prozessoren nicht Leistungsstark. Das hat zur Folge, dass Randbedingungen entstehen, welche bei der Entwicklung beachtet werden müssen. Dazu zählen Rechenleistung, Verlustleistung und Ressourcenverbrauch (S. 233)[[@gesslerEntwicklungEingebetteterSysteme2014]]. Jedoch sind grade deshalb solche Systeme erschwinglich, da keine Hardware benötigt wird, welche auf maximale Rechenleistung und Speicherverbrauch ausgelegt ist. Heute sind eingebettete Systeme in den unterschiedlichsten Anwendungsgebieten zu finden, wie zum Beispiel: Autos, Schienenfahrzeuge, Flugzeugen, in der Telekommunikation und bei der Fertigungsautomatisierung (S. 2)[[@marwedelEingebetteteSystemeGrundlagen2021]]. Eingebettete Systeme werden meist mit Hardware-nahen Sprachen programmiert wie C und C++, da diese durch ihre Nähe zur Hardware die höchste Effizienz versprechen (S. 159)[[@gesslerEntwicklungEingebetteterSysteme2014]].

___
## Virtueller Raum
Der Begriff des Raums ist in den unterschiedlichsten Fachbereichen zu finden, mit den verschiedensten Bedeutungen.
In der Mathematik gibt es viele Arten von Räumen (algebraischer Raum, euklidischer Raum, Hilbert Raum, etc.), die präzise definiert werden können.
Kulturwissenschaftlich spielt der Raum ebenfalls eine große Rolle, jedoch nicht als mathematisches Konstrukt, sondern bezieht zwischenmenschliche und gesellschaftliche Aspekte mit ein.
Der Begriff des _virtuellen Raums_ ist jedoch nicht klar eingrenzbar.
Da sich die verschiedensten Fachbereiche ihm bedienen, hängt die Definition davon ab, aus welcher Richtung man sich ihm nähert.
Deshalb wird im Zuge dieser Arbeit der Begriff des _virtuellen Raums_ wie folgt definiert:

Ein virtueller Raum ist ein durch einen Computer erzeugte Umgebung, welche auf eine zweidimensionale Projektionsfläche abgebildet werden kann.
Es ist kein im physikalischen Sinne real existierender Raum mit messbaren Abmessungen.
Wie auch in anderen Räumen ist es auch im virtuellen Raum möglich, durch ihn hindurch zu navigieren.
So hat der Nutzer eine Position in diesem, sowie die Möglichkeit diese Position gezielt zu verändern.

In der vorliegenden Arbeit wird vorwiegend mit dreidimensionalen Räumen gearbeitet, auch wenn zweidimensionale Räume denkbar sind.
Ein Beispiel für zweidimensionale Räume ist ein Grafical User Interface; der Nutzer kann innerhalb dieser zwischen verschiedenen Bereichen navigieren.
Dazu werden Hyperlinks und Steuerungselemente wie ein Knopf verwendet.
Ein Beispiel für dreidimensionale Räume sind 3D-Computerspiele.
Für diese wird ein euklidischer Raum berechnet, in dem der Spieler selbstbestimmt umherwandern kann.
Jedoch kann auch eine GUI zusätzlich mit eingebettet sein.

___
## Navigation
Der Duden definiert das Verb _navigieren_ wie folgt: 

>1. „den Standort eines Schiffes oder Flugzeugs bestimmen und es auf dem richtigen Kurs halten“
>2. „(z. B. bei der Suche nach Informationen im Internet) [gezielt] ein Programm oder einen Programmpunkt nach dem anderen aktivieren“
[[@DudenNavigierenRechtschreibung]]

Navigation ist also die Gesamtheit der Mittel, die nötig sind, um navigieren zu können. Ursprünglich kommt das Wort aus der Seefahrt. Früher wurden „_Landmarken, die Küste, Meeresströmungen, Lotungen der Wassertiefen, jahreszeitliche regelmäßige Winde, Wolkenansammlungen und der Flug der Zugvögel_“ (S. 17)[[@wolfschmidtNavigareNecesseEst2008]] genutzt, um die aktuelle Position des eigenen Schiffes bestimmen zu können. Somit konnte sichergestellt werden, dass das Schiff sein Ziel erreicht. Im Laufe der Zeit kamen immer mehr Methoden und Werkzeuge zum Einsatz, um immer präziser die eigene Position ermitteln und den optimalen Weg bestimmen zu können. Jedoch beschränkt sich das Wort nicht auf das Herausfinden der eigenen Position, sondern schließt die Tätigkeit der Wegfindung, sowie das Kurshalten eines vorher festgelegten Pfades mit ein. 
Mit dem Aufkommen neuer Verkehrsmittel wie Flugzeuge und neuen Technologien wie GPS wurde der Begriff der Navigation weiter gefasst. So definiert der Medienwissenschaftler Florian Sprenger Navigation wie folgt: „_… eine Praxis des Umgangs mit Relationen._“ (S. 1)[[@sprengerNavigationenUndRelationen2022]]. Das navigierende Individuum oder Objekt entscheidet auf Grundlage von „_medial ver- oder kulturtechnisch ermittelten Verhältnis zu anderen Objekten \[…] oder durch Repräsentationen dieser Relationen auf imaginären, geographischen oder digitalen Karten_“ in welche Richtung sich bewegt werden muss (S. 1)[[@sprengerNavigationenUndRelationen2022]].
Im Kontext dieser Arbeit navigiert der Nutzer auf einer zweidimensionalen Ebene, in einem dreidimensionalen virtuellen Raum. Er steuert dabei gezielt seine Fortbewegung und interagiert mit der virtuellen Umgebung, indem die Räder eines Rollstuhls, in dem er sitzt, gedreht werden.
Es soll im Rahmen dieser Arbeit zusätzlich hervorgehoben werden, dass Navigation im Gegensatz zum reinen Fortbewegen nicht passiv ist. Das Objekt oder Individuum, das navigiert, hat einen Einfluss auf die Richtung der Fortbewegung.

___
## Gyroskop
Ein Gyroskop ist ein Sensor, der genutzt wird, um „_die Winkelgeschwindigkeit um eine feste Achse_“ zu messen (S. 1)[[@armeniseAdvancesGyroscopeTechnologies2010]]. Es gibt verschiedene Arten von Gyroskopen, die Messungen mit unterschiedlichen Methoden vornehmen. So wird zwischen drei Arten von Gyroskopen unterschieden: optische, vibrierende und welche, bei denen eine Masse rotiert. Mithilfe von MEMS (Micro Eelectro Mechanical Systems) konnten Trägheitssensoren miniaturisiert und in Massen produziert werden. In der Elektrotechnik sind diese beliebt, da sie klein und kostengünstig sind [[@maenakaMEMSInertialSensors2008]]. Diese messen meist den Coriolis-Effekt. Die Kraft, die der Effekt wirken lässt, entsteht durch die Rotation um eine der vorher festgelegten Achsen. Der Sensor kann diese Kraft messen und als elektrisches Signal weitergeben, sodass anschließend das Signal digitalisiert werden kann [[@nairCompleteGuideMPU60502021]]. Heute sind sie in durch ihre Verfügbarkeit in den verschiedensten Anwendungsgebieten zu finden, wie dem Auto, der Medizin oder der Unterhaltungselektronik (S. 1)[[@armeniseAdvancesGyroscopeTechnologies2010]].

___
## Eingabegerät
Im Buch _Virtual und Augmented Reality (VR/AR)_ von 2013 werden Eingabegeräte wie folgt beschrieben:

>„_Eingabegeräte dienen der sensorischen Erfassung von Nutzerinteraktionen._“
>(S. 97)[[@doernerVirtualUndAugmented2013]]

Das heißt, dass Eingabegeräte eine Schnittstelle von Mensch zu Maschine darstellen. 
Jedes benötigt dazu einen Sensor der eine physikalische Zustandsveränderung, meistens eine Bewegung in ein elektrisches Signal umwandelt. 
Ein Computer kann das erzeugte Signal auslesen und interpretiert aus diesem eine auszuführende Anweisung. 
Je nach Anwendungsgebiet sind unterschiedliche Eingabegeräte notwendig.
So ist für das Schreiben eine Tastatur nutzerfreundlicher im Vergleich zu einem Spielcontroller.
Ein Eingabegerät kann aus mehreren Eingabetypen bestehen.
Die gängigsten sind binäre und stufenlose Tasten, sowie welche entlang von Achsen wie beispielsweise Thumbsticks oder Touchpads.
Darüber hinaus gibt es noch weitere Eingabetypen, diese zu nennen, jedoch den Rahmen ausreizen würde.
Allgemein lässt sich festhalten:
Ist ein Mensch fähig die Messwerte eines Sensors zu manipulieren, kann dieser als Eingabegerät verwendet werden kann.