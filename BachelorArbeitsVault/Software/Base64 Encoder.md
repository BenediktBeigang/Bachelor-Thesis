# Base64 Encoder
[Online-Converter](https://www.goodconverters.com/binary-to-base64)

### Step 1
Long to binary

142 => 1000 1110
-142 => -1000 1110

Eigentlich wären die negativen zahlen wesentlich länger, so spart man sich aber sowas wie: ```ffff ffff ffff ffff```.


### Step 2
Binärzahl wird in Pakete gepackt: 

0000 0000 0100 1110 0110 1111

| Byte   | Binary  |                      |
| ------ | ------- | -------------------- |
| Byte 1 | 00 0000 | Wird rausgeschmissen |
| Byte 2 | 01 1110 |           e          |
| Byte 3 | 00 0000 |           A          |
| Byte 4 | 10 1111 |           v          |

### Step 3
Falls negativ wird noch '-' angehängt

