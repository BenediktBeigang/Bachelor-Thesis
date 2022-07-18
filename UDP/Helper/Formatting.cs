public static class Formatting
{
    public static string GenerateSpacesLeft(string input, int count)
    {
        string spaces = "";
        for (int i = 0; i < count; i++)
        {
            spaces += " ";
        }
        return spaces + input;
    }

    public static string GenerateSpacesRight(string input, int count)
    {
        string spaces = "";
        for (int i = 0; i < count; i++)
        {
            spaces += " ";
        }
        return input + spaces;
    }
}