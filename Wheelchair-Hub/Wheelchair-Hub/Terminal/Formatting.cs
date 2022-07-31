public class Formatting
{
    private readonly string[] Header;
    public readonly string FormatString;

    public Formatting(string[] header)
    {
        Header = header;
        FormatString = GenerateFormatString();
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