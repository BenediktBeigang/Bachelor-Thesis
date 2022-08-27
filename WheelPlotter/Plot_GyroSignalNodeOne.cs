public class Plot_GyroSignalNodeOne
{
    public Plot_GyroSignalNodeOne(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = record.Select(r => r.NodeOne_SmoothedValue).ToArray();

        var plt = new ScottPlot.Plot(1600, 900);
        plt.XLabel("Time [s]");
        plt.Title($"AngularVelocity raw/smoothed Node One");

        var signal = plt.AddSignal(x_One, sampleRate: 60);
        signal.Color = System.Drawing.Color.RoyalBlue;
        signal.YAxisIndex = 0;
        plt.YAxis.Label("signal [°/s]");
        plt.YAxis.Color(signal.Color);

        var filteredSignal = plt.AddSignal(x_Two, sampleRate: 60);
        filteredSignal.Color = System.Drawing.Color.Orange;
        var filteredSignal_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        filteredSignal.YAxisIndex = 2;
        filteredSignal_Axis.Label("filteredSignal [°/s]");
        filteredSignal_Axis.Color(filteredSignal.Color);

        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 2);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }

}