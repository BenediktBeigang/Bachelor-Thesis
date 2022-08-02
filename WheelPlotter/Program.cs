bool running = true;
while (running)
{
    Console.WriteLine("Enter Name of File:");
    string? input = Console.ReadLine();
    if (input == "q") break;

    string path = @"Files\";
    string name = input!;
    string filetype = ".csv";

    try
    {
        Console.WriteLine("Try to Parse File");
        string[] lines = File.ReadAllLines(path + name + filetype);
        double[] one = new double[lines.Length];
        double[] two = new double[lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            string[] lineSplit = lines[i].Split(';');
            one[i] = double.Parse(lineSplit[0]);
            two[i] = double.Parse(lineSplit[1]);
        }
        var plt = new ScottPlot.Plot(1600, 900);
        plt.AddSignal(one, sampleRate: 60);
        plt.AddSignal(two, sampleRate: 60);
        plt.Title("RawGyroValues");
        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
    catch (Exception)
    {

    }
}