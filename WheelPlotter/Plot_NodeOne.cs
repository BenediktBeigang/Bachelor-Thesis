public class Plot_NodeOne
{
    public Plot_NodeOne(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = record.Select(r => r.NodeOne_Acceleration).ToArray();
        double[] x_Three = record.Select(r => r.NodeOne_Datarate).ToArray();

        var plt = new ScottPlot.Plot(1600, 900);
        plt.Title("Node One");
        plt.XLabel("Time [s]");

        var node_One = plt.AddSignal(x_One, sampleRate: 60);
        node_One.Color = System.Drawing.Color.RoyalBlue;
        node_One.YAxisIndex = 0;
        plt.YAxis.Label("AngularVelocity [°/s]");
        plt.YAxis.Color(node_One.Color);

        var acceleration = plt.AddSignal(x_Two, sampleRate: 60);
        acceleration.Color = System.Drawing.Color.DarkGreen;
        acceleration.YAxisIndex = 1;
        plt.YAxis2.Ticks(true);
        plt.YAxis2.Label("Acceleration Sum [°/s^2]");
        plt.YAxis2.Color(acceleration.Color);

        var messagesPerSecond = plt.AddSignal(x_Three, sampleRate: 60);
        messagesPerSecond.Color = System.Drawing.Color.Orange;
        var messagesPerSecond_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        messagesPerSecond.YAxisIndex = 2;
        messagesPerSecond_Axis.Label("MessagesPerSecond");
        messagesPerSecond_Axis.Color(messagesPerSecond.Color);


        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        plt.SetAxisLimits(yMin: 0, yMax: 100, yAxisIndex: 2);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}