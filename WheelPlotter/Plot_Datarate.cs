using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class Plot_Datarate
{
    public Plot_Datarate()
    {
        string jsonString = File.ReadAllText(@"Files/raw.json");
        List<Sample> record = JsonSerializer.Deserialize<List<Sample>>(jsonString) ?? new List<Sample>();
        jsonString = File.ReadAllText(@"Files/espNow.json");
        List<Sample> record2 = JsonSerializer.Deserialize<List<Sample>>(jsonString) ?? new List<Sample>();

        (List<Sample>, List<Sample>) filled = FillUp(record, record2);
        double[] xs = Generate_Xs(Math.Max(record.Count, record2.Count));
        (double[], double[]) wifi = DataratesToMinMax(filled.Item1);
        (double[], double[]) espNow = DataratesToMinMax(filled.Item2);

        var plt = new ScottPlot.Plot(1600, 900);
        plt.Title("Pakete pro Sekunde unter Verwendung von WiFi und WebSockets");
        plt.XLabel("Zeit [s]");

        plt.AddFill(xs, wifi.Item1, xs, wifi.Item2);
        plt.AddFill(xs, espNow.Item1, xs, espNow.Item2);

        var one = plt.AddScatter(xs, wifi.Item1);
        plt.YAxis.Color(one.Color);

        var two = plt.AddScatter(xs, wifi.Item2);
        var twoAxis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        twoAxis.Color(two.Color);

        var three = plt.AddScatter(xs, espNow.Item1);
        var threeAxis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 4);
        threeAxis.Color(three.Color);

        var four = plt.AddScatter(xs, espNow.Item2);
        var fourAxis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 6);
        fourAxis.Color(two.Color);

        // plt.SetAxisLimitsX(0, 54.185);
        // plt.SetAxisLimits(yMin: 57, yMax: 88, yAxisIndex: 0);
        // plt.SetAxisLimits(yMin: 57, yMax: 88, yAxisIndex: 2);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }

    private (List<Sample>, List<Sample>) FillUp(List<Sample> record1, List<Sample> record2)
    {
        int one = record1.Count;
        int two = record2.Count;
        int sub = Math.Abs(one - two);
        if (sub == 0) return (record1, record2);

        List<Sample> fill = new();
        for (int i = 0; i < sub; i++)
        {
            fill.Add(new Sample());
        }

        if (one > two)
        {
            record2.AddRange(fill);
        }
        else
        {
            record1.AddRange(fill);
        }
        return (record1, record2);
    }

    private double[] Generate_Xs(int count)
    {
        double step = ((double)1 / (double)60);
        double[] xs = new double[count];
        for (int i = 0; i < count; i++)
        {
            xs[i] = step * 1;
        }
        return xs;
    }

    private (double[], double[]) DataratesToMinMax(List<Sample> record)
    {
        double[] max = new double[record.Count];
        double[] min = new double[record.Count];
        for (int i = 0; i < record.Count; i++)
        {
            double one = record[i].NodeOne_Datarate;
            double two = record[i].NodeTwo_Datarate;
            max[i] = Math.Max(one, two);
            min[i] = Math.Min(one, two);
        }
        return (min, max);
    }
}