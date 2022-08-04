# Bachelor-Thesis
In my Bachelor-Thesis I want to use the rotating wheels of a wheelchair to do controller-inputs. For my embedded-system I use an ESP32 (Board: DevKit C V4) and the MPU6050 Gyroscope. They are held together by an 3D-Printed Box that also contains a power bank for power-supply.
On the other end is a .Net 6.0 Console Application receiving the data and using it to map the gyro-data to different controller-inputs. For example I simulated differently realistic wheelchairs or used the data to control one full controller-joy-stick and four controller-buttons. The goal is to use the wheelchair as a new method to move in Virtual-Reality without limitations and cause less motion-sickness.

## Languages
- C/C++
- C#

## Software/Frameworks
- Visual Studio Code
- PlatformIO
- Blender
- Arduino-Framework
- .Net 6.0
- Obsidian

## Used Libraries

**C++**

| Library                                                                       | Description                                                         |
| ----------------------------------------------------------------------------- | ------------------------------------------------------------------- |
| [WiFi-Manager](https://github.com/tzapu/WiFiManager)                          | Used to connect ESP with WiFI-Network without hardcoded credentials |
| [links2004/WebSockets@^2.3.7](https://github.com/Links2004/arduinoWebSockets) | Used to host a WebSocket-Server                                     |
  
**C#**
| Library                                                           | Description                                                              |
| ----------------------------------------------------------------- | ------------------------------------------------------------------------ |
| [websocket-client](https://github.com/Marfusios/websocket-client) | Used to subscribe to the WebSocket-Server on the ESP                     |
| [ViGEm.NET](https://github.com/ViGEm/ViGEm.NET)                   | Used to emulate and programmatically control a XBox360-Controller        |
| [ScottPlot](https://github.com/ScottPlot/ScottPlot)               | Used to render plots of the rotation-data and data rate of the connection |
| [nunit](https://github.com/nunit/nunit)                           | Used for Unit-Testing                                                    |

## Gallery