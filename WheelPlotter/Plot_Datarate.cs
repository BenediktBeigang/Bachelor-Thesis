public class Plot_Datarate
{
    public Plot_Datarate(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Datarate).ToArray();
        double[] x_Two = record.Select(r => r.NodeTwo_Datarate).ToArray();

        var plt = new ScottPlot.Plot(1600, 900);
        plt.Title("Messages per Second");
        plt.XLabel("Time [s]");

        var node_One = plt.AddSignal(x_One, sampleRate: 60);
        node_One.YAxisIndex = 0;
        plt.YAxis.Label("Node_One [msg/s]");
        plt.YAxis.Color(node_One.Color);

        var node_Two = plt.AddSignal(x_Two, sampleRate: 60);
        var node_Two_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        node_Two.YAxisIndex = 2;
        node_Two_Axis.Label("Node_Two [msg/s]");
        node_Two_Axis.Color(node_Two.Color);

        plt.SetAxisLimits(yMin: 0, yMax: 100, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: 0, yMax: 100, yAxisIndex: 2);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}