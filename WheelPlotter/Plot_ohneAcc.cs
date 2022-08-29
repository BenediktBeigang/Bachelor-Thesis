public class Plot_ohneAcc
{
    public Plot_ohneAcc(List<Sample> record, string movementState, string file)
    {
        double[] x_One = record.Select(r => r.NodeOne_SmoothedValue).ToArray();
        double[] x_Two = record.Select(r => r.NodeOne_Acceleration + r.NodeTwo_Acceleration).ToArray();
        double[] x_Three = record.Select(r => r.NodeTwo_SmoothedValue).ToArray();

        var plt = new ScottPlot.Plot(5000, 1200);
        var legend = plt.Legend();
        // plt.Title($"AngularVelocity with Acceleration ({movementState}) ({file})");
        plt.Title("Bewegungs-Zustände mit unerwünschtem Neigen");
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

        plt.SetAxisLimitsX(xMin: 2.1, xMax: 2.6);
        plt.SetAxisLimits(yMin: -100, yMax: 400, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        plt.SetAxisLimits(yMin: -100, yMax: 400, yAxisIndex: 2);

        // plt.SetAxisLimits(yMin: -32767, yMax: 32767, yAxisIndex: 0);
        // plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        // plt.SetAxisLimits(yMin: -32767, yMax: 32767, yAxisIndex: 2);

        // plt.AddHorizontalLine(-81.25, System.Drawing.Color.Black);
        // plt.AddText("Schwellenwert s3", 2.3, -50, 20, System.Drawing.Color.Black);
        plt.AddText("Neigen", 2.155, 400, 20, System.Drawing.Color.Black);
        //plt.AddText("Ruhezustand", 2.255, 400, 20, System.Drawing.Color.Black);
        plt.AddText("Sichtachsenbewegung", 2.34, 400, 20, System.Drawing.Color.Black);


        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}