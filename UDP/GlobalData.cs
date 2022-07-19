using System.Collections.Concurrent;

public static class GlobalData
{
    public static bool NodeConnected_One { get; set; }
    public static bool NodeConnected_Two { get; set; }

    public static int RawValue_One { get; set; }
    public static int RawValue_Two { get; set; }

    public static List<string> LastMessages { get; set; } = new();

    public static double CalculatedValue_One { get; set; }
    public static double CalculatedValue_Two { get; set; }
}