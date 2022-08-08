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
}