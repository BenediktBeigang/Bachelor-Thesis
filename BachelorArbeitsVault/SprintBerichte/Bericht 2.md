# Bericht 2 (Woche 3/4)
---

### Allgemeine Situation
Da das Problem mit den Gyroskopen an einem fehlenden Anschluss eines Gyro-Pins lag, ist es notwendig gewesen, die Platinen neu zu löten, bzw. anzupassen. Leider ist mein Kommilitone, bei dem ich dies bisher gemacht habe, an Corona erkrankt, sodass ich keinen Zugang zu seiner Werkstatt hatte. Ich habe deshalb ein wenig umdisponieren müssen.

### WiFi und Verbindungen
Begonnen habe ich mit dem Fixen der Gyros und der WiFi-Kommunikation. Diese funktionieren nun zusammen. Anschließend habe ich das System getestet und weiterentwickelt. 
- Es können jetzt zwei Geräte gleichzeitig angeschlossen werden.
- Die Datenraten können gemessen werden.
- Beide Seiten können mit Verbindungsabbrüchen umgehen und versuchen sich wieder zu verbinden.

### ESP-Now Kommunikation
Ich habe mit der Implementation einer weiteren Verbindung angefangen. Diese besteht aus einem Verbindungsprotokoll des Chipherstellers, sowie der Übertragung der Daten über einen seriellen Port. Diese Variante konnte fertig implementiert werden und kann jetzt mit der WiFi-Alternative verglichen werden.

### Rohdatenumrechnung
Ich habe den Code, der im Zuge des vorangegangenen Projekts entstanden ist, zur Berechnung einer realistischen Rollstuhlbewegung in meine neue Software überführt. Im nächsten Schritt können auch andere Umrechnungen implementiert werden. So habe ich zum Beispiel geplant eine vereinfachte Rollstuhlsimulation zu implementieren, da ich vermute, dass diese auf den Benutzer angenehmer wirken könnte. Aber auch das Steuern einer Maus ist denkbar.

### Controller-Emulation
Mithilfe einer Bibliothek kann die Software jetzt einen XBox360-Controller emulieren und die Eingaben des Rollstuhls auf diesen mappen.

### Vollständiger System-Test
Seit letzten Freitag ist mein Kommilitone wieder negativ, sodass ich bei ihm schnell vorbeigehuscht bin und das Nötigste abgeholt habe, um die Leiterplatten umlöten zu können. Dabei habe ich auch endlich den eigentlichen Rollstuhl abgeholt, sowie die fehlende gedruckte Box. Am Wochenende habe ich es dann noch geschafft, das System einmal erfolgreich ganz zu testen. Das heißt: 
1. Beide Boxen sind mit der Hardware an den Rädern befestigt.
2. Die Daten werden drahtlos an den Rechner geschickt.
3. Mithilfe der Umwandlung der Rohdaten wird die Bewegung des Rollstuhls simuliert.
4. Die errechneten Daten werden als Controller-Ausgabe zurückgegeben.

![[Box_AnRollstuhl.jpg|300]]

Im nächsten Schritt möchte ich das System in echter Software testen. Sollte das System noch an manchen Stellen Verbesserungspotenzial haben, möchte ich mögliche Lösungen evaluieren und wenn noch Zeit ist diese auch implementieren und testen.

### Recherche, Theorie und Schreiben
In den Tagen in denen ich keine Software mehr schreiben konnte, wegen des Krankheitsfalles, habe ich die Zeit genutzt, mit der Recherche zu beginnen und erste Unterkapitel zu schreiben. Dabei habe ich mich vor allem mit den Hardware-Themen beschäftigt, an denen ich die vergangenen Wochen gearbeitet habe. Zusätzlich habe ich die Rohdatenumrechnung in eine Rollstuhlsimulation erläutert.