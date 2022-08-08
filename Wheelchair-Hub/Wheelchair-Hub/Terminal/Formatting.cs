public class Formatting
{
    private readonly string[] Header;
    public readonly string FormatString;

    public Formatting(string[] header)
    {
        Header = header;
        FormatString = Generate_TableFormatString();
    }

    private string Generate_TableFormatString()
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