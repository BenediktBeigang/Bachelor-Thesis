# Einleitung
___
Mit dem Aufkommen von Computern entwickelten sich parallel die unterschiedlichsten Geräte und Methoden, mit denen Anweisungen an die Rechenmaschine übermitteln werden können.
Anfangs noch mit Schaltern und Lochkarten, entwickelten sich nach und nach, die unterschiedlichsten Eingabegeräte und Methoden, um den Maschinen präzise zu vermitteln, was diese tun sollen.
Das Entstehen der verschiedensten virtuellen Umgebungen führte zum Wunsch, in diesen möglichst barrierefrei zu navigieren.
So setzten sich beispielsweise Computermäuse durch, mit der Verbreitung erster Mikrocomputer mit graphischer Benutzeroberfläche.
Grund dafür ist, dass die Navigation in einer zweidimensionalen Umgebung mit ihr vereinfacht wurde.
Manche Eingabegeräte wurden aus anderen Bereichen, in die Welt des Computers übersetzt, wie zum Beispiel die Schreibmaschine hin zur Tastatur.
Ein völlig neues Design hingegen war der Spielcontroller, der durch die Entwicklung von Videospielen und Spielkonsolen hervorkam.
Dieser stellte sich nicht nur als geeignetes Eingabegerät im zweidimensionalen Raum raus, sondern auch im dreidimensionalen.

Heute erlauben eingebettete Systeme theoretisch jeden Gegenstand in ein Eingabegerät zu verwandeln.
Solange ein Gegenstand unterschiedliche Zustände abbilden kann, ist es möglich, über die Manipulation dieser Zustände dem Computer Anweisungen zu übermitteln.
Begrenzt wird die Anzahl unterschiedlicher Anweisungen nur durch die Anzahl der verschiedenen Zustände, die das Eingabegerät annehmen kann.
Jedoch muss gleichzeitig auch gewährleistet sein, dass jeder Nutzer einfach, präzise und bequem die Eingaben tätigen kann.

Ein Gegenstand, der nur vereinzelt als Eingabegerät in Betracht gezogen wurde, ist der Rollstuhl.
Durch die geringe Anzahl von Eingaben, bedingt durch die zwei Räder, ist der Nutzer eingeschränkter als bei anderen Eingabegeräten.
So kann die Frage entwickelt werden, inwieweit man sich mit einem Rollstuhl im virtuellen Raum bewegen und mit ihm interagieren kann und was nötig ist, um ein solches System zu entwickeln.
In der Vergangenheit wurde mithilfe von aufwändigen Konstruktionen Simulationen eines Rollstuhls entwickelt, welche mit einem echten Rollstuhl gesteuert werden können.
Jedoch beschränkten sich diese auf die möglichst exakte Simulation der Bewegung, inklusive Krafteinwirkung auf die Räder, wenn in der virtuellen Welt der Boden uneben ist.
Für einen privaten Endkunden, der seinen Rollstuhl als Eingabegerät an seinem Heimrechner nutzen möchte, sind die bislang entwickelten Systeme ungeeignet.

Diese Arbeit widmet sich der Frage, mit welchen Mitteln ein Rollstuhl als Eingabegerät umfunktioniert werden kann und wie die Bewegungen der Räder des Rollstuhls in geeignete Anweisungen für den Computer abgebildet werden können.
Möglichst viele Anwendungen sollen dabei theoretisch vom Rollstuhl gesteuert werden können.
Das System soll dabei klein und kostengünstig sein, sodass es für einen privaten Endkunden praktikabel und erschwinglich ist.
Dafür werden im Rahmen dieser Arbeit zunächst einige Begriffe erläutert, die Bachelor-Thesis im Kontext aktueller Forschung verortet, sowie anschließend ein eingebettetes System entwickelt und analysiert.
Dieses System übermittelt die Rotationsdaten der Räder an eine externe Software (im Folgenden Rollstuhl-Software genannt), die auf dem selben Betriebssystem läuft wie die zu steuernde Anwendung.
Zuletzt werden Verfahren implementiert und untersucht, mit dem die empfangenen Daten auf Eingaben eines herkömmlichen Eingabegeräts abgebildet werden können, damit die Anweisungen auch von anderer Software gelesen werden kann.
Dabei soll dem Nutzer eine möglichst hohe Bewegungs- und Interaktionsfreiheit geboten sein.

___

In dieser Arbeit soll untersucht werden, ob mit einem [[Rollstuhl]] die Fortbewegung im [[virtuellen Raum]] besser funktioniert?
Im virtuellen Raum ist man oft eingeschränkt durch verschiedene Art und Weisen. 
Die Spielfläche ist begrenzt durch den Raum, in dem man ist. 
Mögliche Lösungen sind:
1. Der Benutzer ist stationär (sitzen/stehen)
2. Der Benutzer kann sich teleportieren
3. Der Benutzer bewegt sich mit dem [[Joystick]] eines [[Controller]]s umher (=> Übelkeit)
4. Der Benutzer ist in eine Apparatur aufgehängt, in der er laufen kann (=> teuer, umständlich, unpraktiabel, funktioniert nur so mäßig)

Alle Varianten haben Vor- und Nachteile. Verwendet man den [[Rollstuhl]] senkt das z.B. die Übelkeit, ist aber eingeschränkter in der Interaktion mit der Spielwelt. 

>Ziel dieser Arbeit ist es, dem Nutzer möglichst viele Steuerungselemente an die Hand zu geben, um sich möglichst frei im Raum bewegen, sowie mit der Umgebung interagieren zu können.

___

## Thesen
Ich möchte ein System entwickeln, mit dem man Software steuern kann; mit einem [[Rollstuhl]] – ähnlich zu einem Spielcontroller; (dargestellt über VR).
1. Ein Rollstuhl kann als Peripheriegerät genutzt werden zur Steuerung von Software.
2. Mithilfe eines Rollstuhls kann man sich virtuell fortbewegen, und leidet dabei verminderter bis gar nicht an Motion-Sickness.


