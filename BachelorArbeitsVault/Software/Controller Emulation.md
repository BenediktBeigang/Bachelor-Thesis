# Controller Emulation
___

## Idee
Ein (XBOX)-[[Controller]] besitzt zwei [[Joysticks]].
Der linke ist meist für die Bewegung belegt, der rechte zum Umschauen.

y-Achse des linken Sticks für Position
x-Achse des rechten Sticks für Rotation um die eigene Achse

___

## [ViGEm](https://vigem.org/)
C# Bibliothek zum emulieren eines XBOX-Controllers.
[NuGet](https://www.nuget.org/packages/Nefarius.ViGEm.Client)
[Tutorial](https://forums.vigem.org/topic/273/vigem-net-feeder-example-step-by-step)

___

## [xInput Controller Tester](https://thatsmytrunks.itch.io/xinput-controller-tester)
Diese kleine Software ermöglicht es Controller zu testen.

___

## Implementierung

Da der [[Controller]] nur einen endlichen Wertebereich für alle seine Eingaben ermöglicht, 
muss eine maximale Geschwindigkeit ermittelt/gesetzt werden, bei dem der [[Controller]] seinen maximalen Wert übermittelt. 

Da der Wertebereich einer [[Joystick-Achse]] und die Ausgabe des [[Gyroskop]]s beides 16-Bit sind, wäre es möglich, 
die Rohdaten direkt durch den [[ValueTransform]] zu schicken und anschließend als Rohdaten an den [[Controller]] zu schicken. 
Dabei könnte man sich die Umrechnung in GradProSekunde sparen.

>sThumbLX (XInput Axis):
>Left thumbstick x-axis value. Each of the thumbstick axis members is a signed value between -32768 and 32767 describing the position of the thumbstick. 
>A value of 0 is centered. Negative values signify down or to the left. 
>Positive values signify up or to the right. 
>The constants XINPUT_GAMEPAD_LEFT_THUMB_DEADZONE or XINPUT_GAMEPAD_RIGHT_THUMB_DEADZONE can be used as a positive and negative value to filter a thumbstick input.
>[XInput Dokumentation](https://docs.microsoft.com/de-de/windows/win32/api/xinput/ns-xinput-xinput_gamepad)

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

