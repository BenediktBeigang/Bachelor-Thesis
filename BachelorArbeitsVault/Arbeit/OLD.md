### 4.1.1 Messtechniken und Sensoren
Um die Rotation der Räder des Rollstuhls messen zu können, wird ein Sensor benötigt. Dabei gibt es verschiedene Herangehensweisen, wie dieser die Rotation messen kann. 

**Lichtschranke**
Eine Möglichkeit, die Rotation eines Gegenstandes zu messen, ist mithilfe einer Lichtschranke. Hierbei wird durch die Rotation die Lichtschranke in regelmäßigen Abständen durch ein Hindernis blockiert. Über die Frequenz, in der der Lichtstrahl unterbrochen wird, kann anschließend eine Geschwindigkeit errechnet werden. Würde man eine solche Technik verwenden wollen, so müsste man jeweils einen Sensor neben den Rädern des Rollstuhls befestigen. Zusätzlich wäre ein Bauteil erforderlich, welche an den Rädern befestigt wird und dafür sorgt, dass die Lichtschranken unterbrochen werden, wenn sich ein Rad dreht. Beide Sensoren können anschließend von einem Mikrocontroller ausgelesen werden und mithilfe eines Kabels können die Daten an einen nahen Computer und damit an die empfangende Software verschickt werden. Jedoch ist die Empfindlichkeit abhängig vom Abstand der Hindernisse. Ist der Abstand der Hindernisse nicht klein genug, so können kleine Geschwindigkeiten nicht ausreichend gut gemessen werden. 

**Gyroskop**
<mark>Falsch gilt für acceleration nicht rotation</mark>
<mark>Eine Alternative ist die Verwendung eines Gyroskops. Dieses arbeitet mit einer Resonanzmasse die verschoben wird wenn sich das Gyroskop dreht. Die Bewegung der Masse kann dann in ein elektrisches Signal umgewandelt werden und von einem anderen Mikrocontroller ausgelesen werden. Dazu muss das Gyroskop jedoch am Rad selbst befestigt werden. Da das Rad in Bewegung ist, ist es notwendig, dass die Daten kabellos übertragen werden, was wiederum zur Folge hat, dass keine externe Stromversorgung möglich ist. Wird sich also für ein Gyroskop entschieden wird zusätzlich für jedes Gyroskop ein eigener auslesender Mikrocontroller benötigt. Dieser muss in der Lage sein kabellos Daten zu versenden und seinen Strom aus einem Akku oder einer Batterie erhalten. Alle Komponenten müssen dann in einem Behälter zusammengehalten werden. Dafür können auch kleine Geschwindigkeiten gemessen werden.</mark>

Es wurde sich für die Verwendung eines Gyroskops entschieden, da damit kleinere Geschwindigkeiten gemessen werden können und dies wichtig für das Erlebnis des Nutzers ist. Im schlechtesten Fall würde der Nutzer sonst stotternde Bewegungen bemerken oder fehlerhafte Eingaben tätigen.

___

In diesem Kapitel werden die Mittel und Wege erörtert, wie das zu entwickelnde System dieser Arbeit entworfen wurde. Dazu wird zunächst darauf eingegangen, wie das benötigte eingebettete System designt wurde. Anschließend wird sich der Frage gewidmet, wie die gemessenen Daten mithilfe einer Software, weiterverarbeitet beziehungsweise abgebildet werden, zu Eingaben auf einem Spielcontroller. 
<mark>Zuletzt wird untersucht, wie gut die entwickelten Systeme funktionieren und wie diese verbessert werden können.</mark>

___

Will man weitere Interaktionen abbilden, so ist dies nur noch möglich über die Kodierung der Radgeschwindigkeit durch den Nutzer. Entweder werden bestimmte Bewegungen der Räder unterschieden <mark>(Rad laufen lassen, Rad ruckartig bewegen und/oder über Bewegungen ähnlich zu Morsecode Information codieren)</mark> oder der Wertebereich wird geteilt mithilfe von Schwellwerten. Aufgrund des zeitlichen Rahmens dieser Arbeit wurde sich auf das Testen der zweiten Methode beschränkt.

___