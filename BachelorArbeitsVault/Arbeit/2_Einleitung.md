# Einleitung
___
Mit dem Aufkommen des modernen Computers sind parallel die unterschiedlichsten Geräte und Methoden entwickelt worden, mit denen man Anweisungen an die Rechenmaschine übermitteln kann. Anfangs noch mit Lochkarten, entwickelten sich die unterschiedlichsten Eingabemöglichkeiten, um der Maschine präzise zu übermittelt, was sie tun soll. Mit dem Aufkommen des Bildschirms und der Verbreitung des Computers in der Breite der Bevölkerung, entstanden immer mehr virtuelle Umgebungen und gleichzeitig der Wunsch, in diesen möglichst barrierefrei zu navigieren. So wurden bekannte Eingabegeräte aus anderen Bereichen, in die Welt des Computers übersetzt, wie zum Beispiel die Schreibmaschine hin zur Tastatur. Jedoch wurden auch neue Formen und Funktionen gefunden, um den Ansprüchen einer komplexeren digitalen Umgebung gerecht werden zu können. So entstand zum Beispiel die Computer-Maus, um in einer zweidimensionalen Umgebung navigieren zu können. Der Spielcontroller hingegen entpuppte sich nicht nur als gutes Eingabegerät im zweidimensionalen Raum, sondern auch im dreidimensionalen. Heute erlauben eingebettete Systeme theoretisch jeden Gegenstand in ein Eingabegerät zu verwandeln. Solange ein Gegenstand unterschiedliche Zustände abbilden kann, ist es möglich, über die Manipulation dieser Zustände dem Computer Anweisungen zu übermitteln. Begrenzt wird die Anzahl unterschiedlicher Anweisungen nur durch die Anzahl der verschiedenen Zustände, die das Eingabegerät annehmen kann. Jedoch muss gleichzeitig auch gewährleistet sein, dass jeder Nutzer einfach, präzise und bequem die Eingaben tätigen kann. Ein Gegenstand, der wenig Beachtung als Eingabegerät erhalten hat, ist der Rollstuhl. Dieser besitzt zwei Räder, die, mit unterschiedlicher Geschwindigkeit, in jeweils zwei Richtungen gedreht werden können. Es stellt sich die Frage, inwieweit man mit einem Rollstuhl im virtuellen Raum sich bewegen und mit ihm interagieren kann und was nötig ist, um ein solches System zu entwickeln. Durch die geringe Anzahl von Eingaben, also den zwei Rädern, ist der Nutzer vermutlich eingeschränkter als wenn er zum Beispiel direkt einen Spielcontroller verwenden würde. Jedoch birgt die Reduktion von Eingaben und die sehr mechanische Art und Weise, wie man mit dem Rollstuhl interagiert, auch Chancen. So wäre denkbar, das entwickelte System in der Stadtplanung zu verwenden, um leichter die Barrierefreiheit von zukünftigen Bauprojekten virtuell zu testen. Auch wäre denkbar, das System in virtuellen Welten zu nutzen. Drehen sich die Räder des Rollstuhls frei, so kann der Nutzer sich frei im Raum bewegen. Dabei muss er seine Position in der echten Welt nicht verändern und ist nicht limitiert durch die Größe des Raums, in dem er sich befindet. Diese Arbeit widmet sich also der Frage, mit welchen Mitteln ein Rollstuhl als Eingabegerät umfunktioniert werden kann und wie die Bewegungen der Räder des Rollstuhls in sinnvolle Anweisungen für den Computer abgebildet werden können. Dabei wird ein eingebettetes System entwickelt, dass die Rotationsdaten der Räder an eine Software auf dem Computer übermittelt. Danach werden Verfahren untersucht, mit dem die empfangenen Daten auf Eingaben eines herkömmlichen Eingabegeräts abgebildet werden können, damit die Anweisungen auch von anderer Software gelesen werden kann.

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


