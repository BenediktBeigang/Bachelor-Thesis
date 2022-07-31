# Programm Ablauf
___

## Connection
1. Connection wird angelegt (Setup)
2. [[Node]]s werden überschrieben mit neuer Connection Variante
3. Verbindungsaufbau (Connect)
4. [[Gyro-Calibration]] wird gestartet
5. Es werden Pakete empfangen und in den Value-Buffer gespeichert
6. Ein [[Heartbeat]] misst die Datenrate (Wie viele ankommenden Pakete pro Sekunde)

___

## [[ConsolePrint]]
- Printet das Terminal und bezieht Daten aus GlobalData

___

## [[Check_Calibration]]
- Überwacht, ob ein [[Gyroskop]] seinen CalibrationState auf REQUESTET setzt.

___

## [[Refresh_Controller]]

Rechnet mit ValueTransform die Werte aus und schickt sie mit Controller an den Controller. Die Daten werden in [°/s] berechnet da beim rollstuhl die frage aufkommen würde, wie groß der wendekeis ist wenn die values raw-values sind.

___

## [[Heartbeat]]
Jede Sekunde wird überprüft, ob die Anzahl an Datenpaketen pro Sekunde auf null geht/ist.
Wenn das der Fall ist, wird  die DisconnectionTime erhöht.
Wenn die MAXIMUM_DISCONNECTION_DURATION erreicht wird, wird die Verbindung zu diesem Node aus Programmsicht geschlossen/beendet.

___

## Programm

### Befehle
| Input | Event                                  |
| ----- | -------------------------------------- |
| q     | Stops Program                          |
| c     | Starts Calibration                     |
| f     | Flip node on wheelchair                |
| 1     | Flips Rotation-Direction of Device ONE |
| 2     | Flips Rotation-Direction of Device Two |


