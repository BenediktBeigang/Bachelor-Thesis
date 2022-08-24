using System.Text.Json;

public class Program
{
    public static void Main(string[] args)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Enter Name of File:");
            string? input = Console.ReadLine();
            if (input == "q") break;

            string path = @"Files\";
            string name = input!;
            string filetype = ".json";

            try
            {
                Console.WriteLine("Try to Parse File");

                string jsonString = File.ReadAllText(path + name + filetype);
                List<Sample> record = JsonSerializer.Deserialize<List<Sample>>(jsonString) ?? new List<Sample>();

                Plot_GyroValues(record);
                Plot_Datarate(record);
                Plot_Nodes(record);
                Stats_Datarate(record);
            }
            catch (Exception)
            {

            }
        }
    }

    private static void Stats_Datarate(List<Sample> record)
    {
        List<double> datarates = record.Select(r => r.NodeOne_Datarate).ToList();
        datarates.AddRange(record.Select(r => r.NodeTwo_Datarate));
        double average = Math.Round(datarates.Average(), 2);
        double minimum = datarates.Min();
        double maximum = datarates.Max();
        Console.WriteLine($"Pakets per second\nAverage: {average}\nMinimum: {minimum}\nMaximum: {maximum}");
    }

    private static void Plot_GyroValues(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = record.Select(r => r.NodeTwo_Value).ToArray();
        PlotSignal("GyroValues", x_One, x_Two);
    }

    private static void Plot_Datarate(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Datarate).ToArray();
        double[] x_Two = record.Select(r => r.NodeTwo_Datarate).ToArray();
        PlotSignal("Datarate", x_One, x_Two);
    }

    private static void Plot_Nodes(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = record.Select(r => r.NodeOne_Datarate).ToArray();
        PlotMultAxisSignal("Node One", x_One, x_Two);

        x_One = record.Select(r => r.NodeTwo_Value).ToArray();
        x_Two = record.Select(r => r.NodeTwo_Datarate).ToArray();
        PlotMultAxisSignal("Node Two", x_One, x_Two);
    }

    private static void PlotMultAxisSignal(string title, double[] x_One, double[] x_Two)
    {
        var plt = new ScottPlot.Plot(1600, 900);

        var gyroValues = plt.AddSignal(x_One, sampleRate: 60);
        gyroValues.YAxisIndex = 0;
        var datarate = plt.AddSignal(x_Two, sampleRate: 60);
        datarate.YAxisIndex = 1;

        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: 0, yMax: 100, yAxisIndex: 1);

        plt.Title(title);
        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }

    private static void PlotSignal(string title, double[] x_One, double[] x_Two)
    {
        var plt = new ScottPlot.Plot(1600, 900);
        plt.AddSignal(x_One, sampleRate: 60);
        plt.AddSignal(x_Two, sampleRate: 60);
        plt.Title(title);
        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}