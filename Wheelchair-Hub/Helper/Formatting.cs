public class Formatting
{
    private readonly string[] Header;
    private readonly string[] HeaderLine;
    public readonly string FormatString;

    public Formatting(string[] header)
    {
        Header = header;
        FormatString = GenerateFormatString();
        HeaderLine = GenerateHeaderLine();
    }

    private string[] GenerateHeaderLine()
    {
        string[] columns = new string[Header.Length];
        for (int i = 0; i < Header.Length; i++)
        {
            string line = "";
            line.PadLeft(Header[i].Length, '-');
            columns[i] = line;
        }
        return columns;
    }

    private string GenerateFormatString()
    {
        string columns = "";
        for (int i = 0; i < Header.Length; i++)
        {
            string mid = $"{i},{Header[i].Length}";
            columns += "|{" + mid + "}";
        }
        return columns + "|";
    }
}