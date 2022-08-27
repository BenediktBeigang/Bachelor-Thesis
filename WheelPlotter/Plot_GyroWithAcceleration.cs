public class Plot_GyroWithAcceleration
{
    public Plot_GyroWithAcceleration(List<Sample> record, string movementState, string file)
    {
        double[] x_One = record.Select(r => r.NodeOne_SmoothedValue).ToArray();
        double[] x_Two = record.Select(r => r.NodeOne_Acceleration + r.NodeTwo_Acceleration).ToArray();
        double[] x_Three = record.Select(r => r.NodeTwo_SmoothedValue).ToArray();

        var plt = new ScottPlot.Plot(1600, 900);
        plt.Title($"AngularVelocity with Acceleration ({movementState}) ({file})");
        plt.XLabel("Time [s]");

        Program.DrawMovementStates(record, ref plt, movementState);

        var node_One = plt.AddSignal(x_One, sampleRate: 60);
        node_One.Color = System.Drawing.Color.RoyalBlue;
        node_One.YAxisIndex = 0;
        plt.YAxis.Label("Node_One [°/s]");
        plt.YAxis.Color(node_One.Color);

        var acceleration = plt.AddSignal(x_Two, sampleRate: 60);
        acceleration.Color = System.Drawing.Color.DarkGreen;
        acceleration.YAxisIndex = 1;
        plt.YAxis2.Ticks(true);
        plt.YAxis2.Label("Acceleration Sum [°/s^2]");
        plt.YAxis2.Color(acceleration.Color);

        var node_Two = plt.AddSignal(x_Three, sampleRate: 60);
        node_Two.Color = System.Drawing.Color.Orange;
        var node_Two_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        node_Two.YAxisIndex = 2;
        node_Two_Axis.Label("Node_Two [°/s]");
        node_Two_Axis.Color(node_Two.Color);

        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        plt.SetAxisLimits(yMin: -1000, yMax: 1000, yAxisIndex: 2);

        plt.AddHorizontalLine(-925, System.Drawing.Color.Red);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}