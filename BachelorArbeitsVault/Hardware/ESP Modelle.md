# ESP Modelle

## Specs
### Prozessor 
L106 32-bit [RISC](https://de.wikipedia.org/wiki/Reduced_Instruction_Set_Computer "Reduced Instruction Set Computer")-Mikroprozessorkern, basierend auf dem [Tensilica](https://de.wikipedia.org/wiki/Tensilica "Tensilica") Xtensa Diamond Standard 106Micro mit 80 MHz
### Memory:
- 32 [KiB](https://de.wikipedia.org/wiki/KiB "KiB") Befehlsspeicher
- 32 KiB Befehlsspeichercache
- 80 KiB Benutzerdaten-RAM
- 16 KiB ETS Systemdaten-RAM

### Flashspeicher
- Externer Quad-SPI Flashspeicher: bis zu 16 MiB werden unterstützt (512 KiB bis 4 MiB sind bereits angeschlossen)

### WIFI
-   [IEEE 802.11](https://de.wikipedia.org/wiki/IEEE_802.11 "IEEE 802.11") b/g/n [Wi-Fi](https://de.wikipedia.org/wiki/Wi-Fi "Wi-Fi")
-   802.11n bis 72,2 Mb/s
    -   Integrierte(r) [T/R switch](https://de.wikipedia.org/wiki/Duplexer "Duplexer"), [balun](https://de.wikipedia.org/wiki/Balun "Balun"), [LNA](https://de.wikipedia.org/wiki/Low_Noise_Amplifier "Low Noise Amplifier"), [power amplifier](https://de.wikipedia.org/w/index.php?title=RF_power_amplifier&action=edit&redlink=1 "RF power amplifier (Seite nicht vorhanden)") und [Leistungsanpassung](https://de.wikipedia.org/wiki/Leistungsanpassung "Leistungsanpassung")
    -   [WEP](https://de.wikipedia.org/wiki/Wired_Equivalent_Privacy "Wired Equivalent Privacy") oder [WPA/WPA2](https://de.wikipedia.org/wiki/Wi-Fi_Protected_Access "Wi-Fi Protected Access")-Verschlüsselung, ebenso offene Netzwerke
- 16 [GPIO](https://de.wikipedia.org/wiki/Allzweckeingabe/-ausgabe "Allzweckeingabe/-ausgabe")-PINs
- [SPI](https://de.wikipedia.org/wiki/Serial_Peripheral_Interface "Serial Peripheral Interface")
- [I²C](https://de.wikipedia.org/wiki/I%C2%B2C "I²C") (nur per Software implementiert)
- [I²S](https://de.wikipedia.org/wiki/I%C2%B2S "I²S")-Schnittstellen mit DMA (PINs mit GPIO geteilt)
- [UART](https://de.wikipedia.org/wiki/Universal_Asynchronous_Receiver_Transmitter "Universal Asynchronous Receiver Transmitter") auf einem dedizierten PIN, zusätzlich ein reiner Nur-Sende-UART, aktivierbar auf GPIO2
- 10-bit-[ADC](https://de.wikipedia.org/wiki/Analog-Digital-Umsetzer "Analog-Digital-Umsetzer") mit [sukzessiver Approximation](https://de.wikipedia.org/wiki/Analog-Digital-Umsetzer#Sukzessive_Approximation "Analog-Digital-Umsetzer")
- [RTC](https://de.wikipedia.org/wiki/Echtzeituhr "Echtzeituhr") auf GPIO16, hiermit kann per Brücke zu RST der Chip aus dem Deep Sleep aufgeweckt werden

## Modelle
### ESP32 NodeMCU Module 
- WLAN WiFi Dev Kit C Development Board mit CP2102 (Nachfolgermodell zum ESP8266)
- kompatibel mit Arduino 
- [Amazon-Link](https://www.amazon.de/AZDelivery-NodeMCU-Development-Nachfolgermodell-ESP8266/dp/B071P98VTG/ref=sr_1_2?keywords=Espressif%2BESP32%2BWLAN%2BDev%2BKit%2BBoard%2BDevelopment%2BBluetooth%2BWifi%2Bv1%2BWROOM32%2BNodeMCU&linkCode=osi&qid=1649506800&sr=8-2&th=1)
- Preis: 5 Stück, 34€


### NodeMCU Amica Modul V2
- ESP8266 ESP-12F WiFi WiFi Development Board mit CP2102 
- kompatibel mit Arduino
- [Amazon-Link](https://www.amazon.de/AZDelivery-NodeMCU-ESP8266-ESP-12E-Development/dp/B0754LZ73Z/ref=sr_1_14?__mk_de_DE=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=2MIQS17R4M12&keywords=arduino%2Bmit%2Bwlan&qid=1650647445&sprefix=arduino%2Bmit%2Bwlan%2Caps%2C96&sr=8-14&th=1)
- Preis: 5 Stück, 20€ 

### NodeMCU Lolin V3 Module
- ESP8266 ESP-12F WiFi WiFi Development Board mit CH340 
- kompatibel mit Arduino
- [Amazon-Link](https://www.amazon.de/AZDelivery-NodeMCU-ESP8266-ESP-12E-Development/dp/B074Q2WM1Y/ref=sr_1_13?__mk_de_DE=%C3%85M%C3%85%C5%BD%C3%95%C3%91&crid=2MIQS17R4M12&keywords=arduino%2Bmit%2Bwlan&qid=1650647445&sprefix=arduino%2Bmit%2Bwlan%2Caps%2C96&sr=8-13&th=1)
- Preis: 5 Stück, 20€

# Links
[Sending Strings](https://www.youtube.com/watch?v=zBgsbJMoOGk)