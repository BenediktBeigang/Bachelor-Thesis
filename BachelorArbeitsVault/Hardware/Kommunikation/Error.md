# Error

Connected to Network!
Try to connect to Host
...Received packet of size 15 
From 192.168.2.100, port 11000
START of Content:

Searching Nodes

END of Content:

IP set
PORT set
Convert Reply
Sending Reply
Reply begin
Guru Meditation Error: Core  1 panic'ed (LoadProhibited). Exception was unhandled.

Core  1 register dump:
PC      : 0x4014f45e  PS      : 0x00060830  A0      : 0x800d28ef  A1      : 0x3ffb2710  
A2      : 0x3ffc2f50  A3      : 0x00000000  A4      : 0x00000012  A5      : 0x00000012
A6      : 0x00ff0000  A7      : 0xff000000  A8      : 0x3f4004e0  A9      : 0x3ffb26e0  
A10     : 0x00000001  A11     : 0x00000001  A12     : 0x00000000  A13     : 0x0000ff00
A14     : 0x00ff0000  A15     : 0xff000000  SAR     : 0x00000011  EXCCAUSE: 0x0000001c  
EXCVADDR: 0x00000000  LBEG    : 0x40089795  LEND    : 0x400897a5  LCOUNT  : 0xffffffff

Backtrace:0x4014f45b:0x3ffb27100x400d28ec:0x3ffb2730 0x400d2965:0x3ffb2760 0x400d2ad5:0x3ffb27a0 0x400d2b80:0x3ffb27e0 0x400de9d5:0x3ffb2820 

ELF file SHA256: 0000000000000000

Rebooting...
ets Jun  8 2016 00:22:57

rst:0xc (SW_CPU_RESET),boot:0x13 (SPI_FAST_FLASH_BOOT)
configsip: 0, SPIWP:0xee
clk_drv:0x00,q_drv:0x00,d_drv:0x00,cs0_drv:0x00,hd_drv:0x00,wp_drv:0x00
mode:DIO, clock div:2
load:0x3fff0030,len:1184
load:0x40078000,len:12776
load:0x40080400,len:3032
entry 0x400805e4
*wm:[1] AutoConnect