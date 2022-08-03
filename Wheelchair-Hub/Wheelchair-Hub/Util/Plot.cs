public class Plot
{
    public Plot(List<(short, short)> record)
    {
        double[] x_One = ToDoubleArray(record.Select(r => r.Item1).ToArray());
        double[] x_Two = ToDoubleArray(record.Select(r => r.Item2).ToArray());

        var plt = new ScottPlot.Plot(1600, 900);
        plt.AddSignal(x_One, sampleRate: 60);
        plt.AddSignal(x_Two, sampleRate: 60);
        plt.Title("RawGyroValues");
        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());

        // string path = @"Files\";
        // string time = DateTime.Now.ToLocalTime().ToLongTimeString();
        // time = $"{time[0] + time[1]}_{time[3] + time[4]}_{time[6] + time[7]}";
        // string filetype = ".png";
        // plt.SaveFig(path + time + filetype);
    }

    private double[] ToDoubleArray(short[] values)
    {
        double[] result = new double[values.Length];
        for (int i = 0; i < values.Length; i++)
        {
            result[i] = (short)values[i];
        }
        return result;
    }
}