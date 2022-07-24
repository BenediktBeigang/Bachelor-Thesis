# Controller Emulation

## [ViGEm](https://forums.vigem.org/topic/273/vigem-net-feeder-example-step-by-step)
C# Bibliothek zum emulieren eines XBOX-Controllers.

## [xInput Controller Tester](https://thatsmytrunks.itch.io/xinput-controller-tester)
Diese kleine Software ermöglicht es Controller zu testen.


## Implementierung

Da der [[Controller]] nur einen endlichen Wertebereich für alle seine Eingaben ermöglicht, 
muss eine maximale Geschwindigkeit ermittelt/gesetzt werden, bei dem der [[Controller]] seinen maximalen Wert übermittelt. 

Da der Wertebereich einer [[Joystick-Achse]] und die Ausgabe des [[Gyroskops]] beides 16-Bit sind, wäre es möglich, 
die Rohdaten direkt durch den [[ValueTransform]] zu schicken und anschließend als Rohdaten an den [[Controller]] zu schicken. 
Dabei könnte man sich die Umrechnung in GradProSekunde sparen.

Jedoch muss auch folgendes bedacht werden:
Setzt man den [[Gyro-Modus]] zu hoch, muss unter Umständen der Benutzer viel Kraft/Energie aufwenden, um die maximale Geschwindigkeit zu erreichen.
Dies kann als störend und anstrengend wahrgenommen werden. Setzt man den [[Gyro-Modus]] zu niedrig, ist die resultierende maximale Geschwindigkeit zu niedrig, 
was ebenfalls störend vom Benutzer wahrgenommen werden könnte.

Wenn man einen Maximalwert außerhalb der möglichen Gyro-Modi (250, 500, 1000, 2000) verwenden, muss man die Ausgabe zusätzlich skalieren.

**Beispiel:**
[[Gyro-Modus]]: 1000
Maximalwert: 750
| Gyro  | Wert | Controller 16 Bit |
| ----- | ---- | ----------------- |
| 1000  | 750  | 32768             |
| 750   | 750  | 32768             |
| 350   | 350  | 15292             |
| -350  | -350 | -15292            |
| -1000 | -750 | -32768            |

