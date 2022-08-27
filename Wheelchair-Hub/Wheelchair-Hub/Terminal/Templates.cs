using System.Collections.Concurrent;

public static class Templates
{
    public static readonly int[] TableLineValuePositions = new int[] { 1, 7, 18, 29, 46, 65, 83 };
    public static readonly int[] InfoLineValuePositions = new int[] { 2, 15, 41, 49, 60, 72, 90, 18 };
    private const string FORMAT_STRING_TABLELINE = "|{0,4}|{1,0}|{2,10}|{3,10}|{4,16}|{5,18}|{6,17}|{7,17}|";
    private const string FORMAT_STRING_INFOHEAD = "|{0,-11}||{1,-24}||{2,-29}||{3,-32}||{4,-18}|";
    private const string FORMAT_STRING_INFO = "|{0,-11}||{1,-24}||{2,-7}|{3,-10}|{4,-10}||{5,-15}|{6,-16}||{7,-18}|";
    private const int VISIBLE_MESSAGES = 5;

    public static string Template()
    {
        string template = "";
        template += Generate_Table() + '\n';
        template += "" + '\n' + '\n';
        template += "Last Messages: " + '\n';
        template += '\n';
        template += "------------------------------------------" + '\n';
        template += "Last Commands: " + '\n';
        template += '\n';
        return template;
    }

    private static string Generate_Table()
    {
        string table = "";
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        table += String.Format($"{FORMAT_STRING_TABLELINE}", "Node", "", "Connection", "Raw Values", "DegreesPerSecond", "Calibration Status", "MessagesPerSecond", "DisconnectionTime") + '\n';
        table += String.Format($"{FORMAT_STRING_TABLELINE}", "----", "", "----------", "----------", "----------------", "------------------", "-----------------", "-----------------") + '\n';

        table += Generate_TableLine(Node.Node_One) + '\n';
        table += Generate_TableLine(Node.Node_Two) + '\n';
        table += "-----------------------------------------------------------------------------------------------------" + "\n";
        table += "----------------------------------------------------------------------------------------------------------------------------" + "\n";
        table += Generate_InfoLine() + "\n";
        table += "----------------------------------------------------------------------------------------------------------------------------" + "\n";
        table += "--------------------------------------------------------------------------------------------------------------------------------------------------------------------------" + "\n";
        table += LegendToStringFlat();
        table += "--------------------------------------------------------------------------------------------------------------------------------------------------------------------------";
        return table;
    }

    private static string Generate_InfoLine()
    {
        string output = "";
        output += String.Format(FORMAT_STRING_INFOHEAD, "", "", "         Flipped", "          Threshold", "") + '\n';
        output += String.Format(FORMAT_STRING_INFO, " GYRO-MODE", " MAPPING-MODE", " Nodes", " Node-One", " Node-Two", " WheelMovement", " ButtonPressing", " WheelMovementMax") + '\n';
        output += String.Format(FORMAT_STRING_INFOHEAD, "", "", "", "", "", "", "", "");
        return output;
    }

    private static string Generate_TableLine(Node node)
    {
        return String.Format($"{FORMAT_STRING_TABLELINE}", "", "", "", "", "", "", "", "");
    }

    private static string LegendToStringFlat()
    {
        string output = "";
        output += "|| q - quit || c - calibrate gyros || f - flip                                  || m - mapping                                                                          ||\n";
        output += "||          ||                     || n - nodes | 1 - wheel one | 2 - wheel two || 1 - realisticWheelchair | 2 - simpleWheelchair | 3 - wheelchairWithButtons | 4 - gui ||\n";
        return output;
    }
}