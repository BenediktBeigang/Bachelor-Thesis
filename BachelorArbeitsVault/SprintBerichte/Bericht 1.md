# Bericht 1 (Woche 1/2)
---

### Box drucken
Zunächst habe ich mich vorwiegend mit dem Fertigstellen und und 3D-Drucken der Boxen für die Räder beschäftigt. Dabei musste ich immer wieder kleinere Änderungen vornehmen, da der Haltemechanismus mit dem der Deckel auf der Box gehalten wird nicht optimal funktioniert hat. Unter anderem waren die kleinen Harken mit denen beide Teile zusammengehalten werden zu dünn und schlecht gedruckt, da für das Drucken Stützen notwendig waren. Mein derzeitges Design von dem ich mir viel verspreche kann ich in den kommenden Tagen abholen. 

![[Box_Hardware.jpg|300]] ![[Box_Zu.jpg|300]] ![[Box_Mängel.jpg|300]]

### Gyro
Bevor ich mich mit der Datentransfer beschäftige habe ich mich erst vergewissert, dass das Gyroskop und dessen Werte richtig ausgelesen werden. Dabei habe ich noch einmal die verschiedenen Bibliotheken, die es gibt angeschaut und verglichen. Dabei habe ich gleichzeitig viel Neues über die Funktionsweise des Gyroskops erfahren und auch das beim vorangegangen Projekt annahmen getroffen wurden, die sich als falsch herausgestellt haben. Es lässt sich ein schönes Kapitel für die Arbeit ableiten, in dem ich aufschlüssele welche Einstellungen ich für das Gyroskop verwende und warum.


### Wi-Fi
Ich stand vor der Frage mit welcher Verbindungsmethode ich anfangen möchte und entschied mich für WiFi, da diese vermutlich schwieriger zu implementieren ist. Zunächst habe ich mich darum gekümmert, dass der ESP32 sich mit dem lokalen Netzwerk verbinden kann. Dabei hat mir eine sehr hilfreiche Bibliothek viel Arbeit angenommen. 
Im nächsten Schritt habe ich darüber nachgedacht welches Protokoll (UDP/TCP) das sinnvollste für die Übertragung der Daten ist und wie ich das am besten umsetzte. Zunächst habe ich versucht eigenständig mithilfe von UDP verbindungslos die Daten zu übertragen. Dabei hatte ich aber Schwierigkeiten weil nicht jede Bibliothek die ich nutzen wollte, ich zum laufen gebracht habe. Nach weiterer Recherche kam ich zum Entschluss, dass das sinnvollste ein Web-Socket ist der darauf ausgelegt ist Echtzeitdaten an Clienten zu schicken die vorher den Socket abboniert hatten. 
Meine jetztige Architektur sieht vor, dass ein nach dem Verbinden mit dem Heimnetzwerk der ESP32 einen Web-Socket-Server hostet und die Daten des Gyros an alle Clienten die abboniert haben sendet. Vorher schickt der ESP32 einen UDP Broadcast ins Netzwerk um sich bekannt zu machen. Die Software auf dem Rechner wartet auf diesen Broadcast und baut mit dessen IPEndPoint Informationen dann eine Verbindung mit dem Web-Socket auf.
Ich hatte jedoch etwas zu kämpfen mit den Eigenheiten von c/c++ und veralteteten bzw. nicht funktionierenden Bibliotheken, weshalb der Prozess des Codens recht mühsam war und dadurch Zeit gefressen hat.
Inzwischen funktioniert aber theoretisch der Datenaustausch wie gewünscht. Leider funktioniert das Gyro im Zusammenspiel mit der Übertragung der Daten noch nicht und ich bin dran rauszukriegen warum nicht.