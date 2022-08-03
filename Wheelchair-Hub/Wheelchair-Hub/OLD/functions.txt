// private static string CenterValue(int value, int maxLength)
// {
//     string valueAsString = value.ToString();
//     int valueLength = valueAsString.Length;

//     int emptyCount = maxLength - valueLength;
//     int padding = (int)emptyCount / 2;

//     valueAsString = Formatting.GenerateSpacesLeft(valueAsString, padding);
//     valueAsString = Formatting.GenerateSpacesRight(valueAsString, padding);

//     while (valueAsString.Length < maxLength)
//     {
//         valueAsString = Formatting.GenerateSpacesRight(valueAsString, 1);
//     }
//     return valueAsString;
// }


#region FastSend

// client.MessageReceived.Subscribe(msg => WebSocket_OnData(msg.Binary, client));

// consoleString += String.Format("|{0,10}|{1,10}|{2,10}|", "Hi-Lo", hi, lo);
// consoleString += String.Format("|{0,10}|{1,10}|{2,10}|", "ByteLength", GlobalData.ByteLength, "");


// string hi = GlobalData.Hi.ToString();
// string lo = GlobalData.Lo.ToString();

// public static int Hi { get; set; }
// public static int Lo { get; set; }

// public static int ByteLength { get; set; }

// private void WebSocket_OnData(byte[] data, WebsocketClient client)
// {
//     GlobalData.ByteLength = data.Length;
//     GlobalData.Hi = (int)data[0];
//     GlobalData.Lo = (int)data[1];
//     switch (client.Name)
//     {
//         case "left":
//             GlobalData.LeftValue = data[0] | data[1] << 8;
//             break;
//         case "right":
//             GlobalData.RightValue = data[0] | data[1] << 8;
//             break;
//     }
// }

#endregion