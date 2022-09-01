using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProjectStats
{
    public static string LinesOfCode()
    {
        List<string> filepaths = new();// = Directory.GetFiles(project, "*.cs");

        foreach (string file in Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.cs", SearchOption.AllDirectories))
        {
            filepaths.Add(file);
        }

        int counter = 0;
        foreach (string path in filepaths)
        {
            string[] fileAsString = File.ReadAllLines(path);
            counter += CountLines(fileAsString);
        }

        return $"Lines of Code: {counter}";
    }

    private static int CountLines(string[] file)
    {
        int counter = 0;
        foreach (string s in file)
        {
            if (Comment(s) || Empty(s) || Brackets(s)) continue;
            counter++;
        }
        return counter;
    }

    private static bool Comment(string line)
    {
        line = line.Replace(" ", string.Empty);
        if (line.Length > 0) return line[0] == '/';
        return false;
    }

    private static bool Empty(string line)
    {
        line = line.Replace(" ", string.Empty);
        return line.Length == 0;
    }

    private static bool Brackets(string line)
    {
        line = line.Replace(" ", string.Empty);
        if (line.Length > 0) return line[0] == '{' || line[0] == '}';
        return false;
    }
}
