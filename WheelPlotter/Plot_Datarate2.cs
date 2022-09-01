using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Plot_Datarate2
{
    public Plot_Datarate2(List<Sample> record, string movementState, string file)
    {
        double[] y_One = record.Select(r => r.NodeOne_Datarate).ToArray();
        double[] y_Two = record.Select(r => r.NodeTwo_Datarate).ToArray();

        var plt = new ScottPlot.Plot(5000, 1200);
        var legend = plt.Legend();
        plt.Title("Datenrate...");
        plt.XLabel("Time [s]");

        var wifi_NodeOne = plt.AddSignal(y_One, sampleRate: 60);
        wifi_NodeOne.Color = System.Drawing.Color.RoyalBlue;
        wifi_NodeOne.YAxisIndex = 0;
        plt.YAxis.Label("Datenrate Node-One [Pakete/s]");
        plt.YAxis.Color(System.Drawing.Color.Blue);

        var wifi_NodeTwo = plt.AddSignal(y_Two, sampleRate: 60);
        wifi_NodeTwo.Color = System.Drawing.Color.Orange;
        var wifi_NodeTwo_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        wifi_NodeTwo.YAxisIndex = 2;
        wifi_NodeTwo_Axis.Label("Datenrate Node-Two [Pakete/s]");
        wifi_NodeTwo_Axis.Color(System.Drawing.Color.Blue);

        plt.SetAxisLimitsX(xMin: 0, xMax: 60);
        plt.SetAxisLimits(yMin: 50, yMax: 100, yAxisIndex: 0);
        // plt.SetAxisLimits(yMin: 0, yMax: 400, yAxisIndex: 1);
        plt.SetAxisLimits(yMin: 50, yMax: 100, yAxisIndex: 2);

        // plt.AddHorizontalLine(791.93725, System.Drawing.Color.Black);
        // plt.AddHorizontalLine(-892.1, System.Drawing.Color.Black);
        // plt.AddText("792", 55, 950, 20, System.Drawing.Color.Black);
        // plt.AddText("-892", 55, -750, 20, System.Drawing.Color.Black);
        // plt.AddText("Schwellenwert s2", 8.85, -25, 20, System.Drawing.Color.Black);
        // plt.AddText("Sichtachsenbewegung", 8.27, 125, 20, System.Drawing.Color.Black);
        // plt.AddText("Einzelradbewegung", 8.6, 125, 20, System.Drawing.Color.Red);


        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }
}