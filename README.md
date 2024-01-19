# Development and investigation of the usability of a wheelchair as an input device for navigation in virtual space
In this Projekt I want to use the rotation of wheels of a wheelchair to control an emulated XBox-Controller. For my embedded-system I use an ESP32 (Board: DevKit C V4) and the MPU6050 Gyroscope. The components are held together by a 3D-printed box that also contains a power bank for power-supply. On the other end is a .Net 6.0 console application receiving the data and using it to map the gyro-data to different controller-inputs. As long as other software has XBox-Controller support, my Software can be used to control it.

## Folder

| Folder | Description |
|---|---|
| BachelorArbeitsVault | Obsidian-Vault containing notes, links to websites, images, etc. |
| Blender | Blender/STL-Files for printing the Box |
| ESP-Hub | Code for ESP receiving Node-Data and sending it to the Console-App |
| ESP-Node | Code for ESP measuring the Rotation and sending it to Console-App or ESP-Hub  |
| Latex | LaTeX Project to write [Thesis](https://github.com/BenediktBeigang/Bachelor-Thesis/blob/main/Latex/src/main.pdf) |
| Wheelchair-Hub | Console-App to emulating XBox-Controller with Gyro-Data |
| WheelPlotter | App for Plotting RotationData from Wheels with ScottPlot |

## Drivers

- [ViGEmBus](https://ds4-windows.com/download/vigembus-driver/)
- [Driver](https://www.silabs.com/developers/usb-to-uart-bridge-vcp-drivers?tab=downloads) needed to connect ESP32 to USB-Port (install driver in devicemanager).

## Software/Frameworks
- Visual Studio Code
- PlatformIO
- Blender
- Arduino-Framework (C++)
- .Net 6.0 (C#)
- Obsidian

## Used Libraries

**Arduino-Framework/C++**
| Library                                                                       | Description                                                         |
| ----------------------------------------------------------------------------- | ------------------------------------------------------------------- |
| [WiFi-Manager](https://github.com/tzapu/WiFiManager)                          | Used to connect ESP with WiFI-Network without hardcoded credentials |
| [links2004/WebSockets@^2.3.7](https://github.com/Links2004/arduinoWebSockets) | Used to host a WebSocket-Server                                     |
  
**nuGet**
| Library                                                           | Description                                                              |
| ----------------------------------------------------------------- | ------------------------------------------------------------------------ |
| [websocket-client](https://github.com/Marfusios/websocket-client) | Used to subscribe to the WebSocket-Server on the ESP                     |
| [ViGEm.NET](https://github.com/ViGEm/ViGEm.NET)                   | Used to emulate and programmatically control a XBox360-Controller        |
| [ScottPlot](https://github.com/ScottPlot/ScottPlot)               | Used to render plots of the rotation-data and data rate of the connection |
| [nunit](https://github.com/nunit/nunit)                           | Used for Unit-Testing                                                    |

## Gallery
<img src="BachelorArbeitsVault\Quellen\Bilder\Hardware\Platine.jpg" width=50% height=50%/><img src="BachelorArbeitsVault\Quellen\Bilder\Box\Box_Closed.jpg" width=50% height=50%/>
<img src="BachelorArbeitsVault\Quellen\Bilder\Box\Box_Open.jpg" width=50% height=50%/><img src="BachelorArbeitsVault\Quellen\Bilder\Hardware\Wheel.jpg" width=50% height=50%/>
