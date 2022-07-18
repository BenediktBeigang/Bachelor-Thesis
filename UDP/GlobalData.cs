public static class GlobalData
{
    public static bool LeftNodeConnected { get; set; }
    public static bool RightNodeConnected { get; set; }

    public static int LeftValue { get; set; }
    public static int RightValue { get; set; }

    public static List<string> LastMessages { get; set; } = new();

    public static double CalculatedLeftValue { get; set; }
    public static double CalculatedRightValue { get; set; }
}