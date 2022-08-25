public class Plot_Gyro
{
    public Plot_Gyro(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = record.Select(r => r.NodeTwo_Value).ToArray();

        var plt = new ScottPlot.Plot(1600, 900);
        plt.XLabel("Time [s]");
        plt.Title("AngularVelocity");

        Program.DrawMovementStates(record, ref plt, "button");

        var node_One = plt.AddSignal(x_One, sampleRate: 60);
        node_One.Color = System.Drawing.Color.RoyalBlue;
        node_One.YAxisIndex = 0;
        plt.YAxis.Label("Node_One [°/s]");
        plt.YAxis.Color(node_One.Color);

        var node_Two = plt.AddSignal(x_Two, sampleRate: 60);
        node_Two.Color = System.Drawing.Color.Orange;
        var Node_Two_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        node_Two.YAxisIndex = 2;
        Node_Two_Axis.Label("Node_Two [°/s]");
        Node_Two_Axis.Color(node_Two.Color);

        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 2);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }


}