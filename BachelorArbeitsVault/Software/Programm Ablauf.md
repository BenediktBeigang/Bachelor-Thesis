# Programm Ablauf
___

## Connection
1. Connection wird angelegt (Setup)
2. [[Nodes]] werden überschrieben mit neuer Connection Variante
3. Verbindungsaufbau (Connect)
4. [[Gyro-Calibration]] wird gestartet
5. Es werden Pakete empfangen und in den Value-Buffer gespeichert
6. Ein [[Heartbeat]] misst die Datenrate (Wie viele ankommenden Pakete pro Sekunde)

___

## ConsolePrint
- Printet das Terminal und bezieht Daten aus GlobalData

___

## ProgramStep
- Überwacht, ob ein [[Gyroskop]] seinen CalibrationState auf REQUESTET setzt.

___

