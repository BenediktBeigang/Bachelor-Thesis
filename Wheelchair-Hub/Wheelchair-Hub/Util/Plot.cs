public class Plot
{
    public Plot(List<(double, double)> record)
    {
        double[] x_One = record.Select(r => r.Item1).ToArray();
        double[] x_Two = record.Select(r => r.Item2).ToArray();

        var plt = new ScottPlot.Plot(1600, 900);
        plt.AddSignal(x_One, sampleRate: 60);
        plt.AddSignal(x_Two, sampleRate: 60);
        plt.Title("RawGyroValues");
        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }

    public Plot(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = record.Select(r => r.NodeTwo_Value).ToArray();
        PlotSignal("GyroValues", x_One, x_Two);

        x_One = record.Select(r => r.NodeOne_Datarate).ToArray();
        x_Two = record.Select(r => r.NodeTwo_Datarate).ToArray();
        PlotSignal("Datarate", x_One, x_Two);
    }

    private void PlotSignal(string title, double[] x_One, double[] x_Two)
    {
        var plt = new ScottPlot.Plot(1600, 900);
        plt.AddSignal(x_One, sampleRate: 60);
        plt.AddSignal(x_Two, sampleRate: 60);
        plt.Title(title);
        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}