using System.Collections.Concurrent;

public static class GlobalData
{
    public static List<string> LastMessages { get; set; } = new();

    public static Node? Node_One { get; set; }
    public static Node? Node_Two { get; set; }
}