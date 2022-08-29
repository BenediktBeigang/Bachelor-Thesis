public class Plot_1Th
{
    public Plot_1Th(List<Sample> record, string movementState, string file)
    {
        double[] x_One = record.Select(r => r.NodeOne_SmoothedValue).ToArray();
        double[] x_Two = record.Select(r => r.NodeOne_Acceleration + r.NodeTwo_Acceleration).ToArray();
        double[] x_Three = record.Select(r => r.NodeTwo_SmoothedValue).ToArray();

        var plt = new ScottPlot.Plot(5000, 1200);
        var legend = plt.Legend();
        // plt.Title($"AngularVelocity with Acceleration ({movementState}) ({file})");
        plt.Title("Bewegungs-Zustände mit unerwünschtem Auslösen einer Interaktionstaste");
        plt.XLabel("Time [s]");

        Program.DrawMovementStates(record, ref plt, movementState);

        var node_One = plt.AddSignal(x_One, sampleRate: 60);
        node_One.Color = System.Drawing.Color.RoyalBlue;
        node_One.YAxisIndex = 0;
        plt.YAxis.Label("Bahngeschwindigkeit vL [°/s]");
        plt.YAxis.Color(node_One.Color);

        // var acceleration = plt.AddSignal(x_Two, sampleRate: 60);
        // acceleration.Color = System.Drawing.Color.DarkGreen;
        // acceleration.YAxisIndex = 1;
        // plt.YAxis2.Ticks(true);
        // plt.YAxis2.Label("Änderungsrate a [°/s^2]");
        // plt.YAxis2.Color(acceleration.Color);

        var node_Two = plt.AddSignal(x_Three, sampleRate: 60);
        node_Two.Color = System.Drawing.Color.Orange;
        var node_Two_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        node_Two.YAxisIndex = 2;
        node_Two_Axis.Label("Bahngeschwindigkeit vR [°/s]");
        node_Two_Axis.Color(node_Two.Color);

        plt.SetAxisLimitsX(xMin: 8.25, xMax: 9);
        plt.SetAxisLimits(yMin: -500, yMax: 150, yAxisIndex: 0);
        // plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        plt.SetAxisLimits(yMin: -500, yMax: 150, yAxisIndex: 2);

        // plt.SetAxisLimits(yMin: -32767, yMax: 32767, yAxisIndex: 0);
        // plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        // plt.SetAxisLimits(yMin: -32767, yMax: 32767, yAxisIndex: 2);

        plt.AddHorizontalLine(-100, System.Drawing.Color.Black);
        // plt.AddHorizontalLine(-25, System.Drawing.Color.Black);
        plt.AddText("Schwellenwert s", 8.85, -100, 20, System.Drawing.Color.Black);
        // plt.AddText("Schwellenwert s1", 8.85, -100, 20, System.Drawing.Color.Black);
        // plt.AddText("Schwellenwert s2", 8.85, -25, 20, System.Drawing.Color.Black);
        plt.AddText("Sichtachsenbewegung", 8.27, 125, 20, System.Drawing.Color.Black);
        plt.AddText("Einzelradbewegung", 8.6, 125, 20, System.Drawing.Color.Red);


        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}