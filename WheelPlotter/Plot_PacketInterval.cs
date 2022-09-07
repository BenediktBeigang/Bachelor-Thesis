using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class Plot_PacketInterval
{
    public Plot_PacketInterval(List<Sample> record, string file)
    {
        double[] nodeOneYs = record.Select(r => r.NodeOne_PacketInterval).ToArray();
        double[] nodeTwoYs = record.Select(r => r.NodeTwo_PacketInterval).ToArray();

        var plt = new ScottPlot.Plot(5000, 1200);
        var legend = plt.Legend();

        string endString = (file == "Datenrate_espnow") ? "ESP-Now und dem seriellen Port" : "WiFi und WebSockets";
        plt.Title($"Zeit zwischen zwei Paketen im Verlauf der Zeit unter Verwendung von {endString}");
        plt.XLabel("Time [s]");

        var nodeOne = plt.AddSignal(nodeOneYs, sampleRate: 60);
        nodeOne.Color = System.Drawing.Color.RoyalBlue;
        nodeOne.YAxisIndex = 0;
        plt.YAxis.Label("Zeit zwischen zwei Paketen (Node-One) [ms]");
        plt.YAxis.Color(nodeOne.Color);

        // var acceleration = plt.AddSignal(x_Two, sampleRate: 60);
        // acceleration.Color = System.Drawing.Color.DarkGreen;
        // acceleration.YAxisIndex = 1;
        // plt.YAxis2.Ticks(true);
        // plt.YAxis2.Label("Änderungsrate a [°/s^2]");
        // plt.YAxis2.Color(acceleration.Color);

        var nodeTwo = plt.AddSignal(nodeTwoYs, sampleRate: 60);
        nodeTwo.Color = System.Drawing.Color.Orange;
        var nodeTwo_Axis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
        nodeTwo.YAxisIndex = 2;
        nodeTwo_Axis.Label("Zeit zwischen zwei Paketen (Node-Two) [ms]");
        nodeTwo_Axis.Color(nodeTwo.Color);

        double xAxisMax = record.Count / 60;
        plt.SetAxisLimitsX(xMin: 0, xMax: xAxisMax);
        plt.SetAxisLimits(yMin: 0, yMax: 50, yAxisIndex: 0);
        plt.SetAxisLimits(yMin: 0, yMax: 50, yAxisIndex: 2);

        double average = Average_Interval(record);
        double maxInterval = Max_Interval(record);



        plt.AddHorizontalLine(maxInterval, System.Drawing.Color.Black);
        plt.AddHorizontalLine(average, System.Drawing.Color.Black);
        // plt.AddText("792", 55, 950, 20, System.Drawing.Color.Black);
        // plt.AddText("-892", 55, -750, 20, System.Drawing.Color.Black);
        // plt.AddText("Schwellenwert s2", 8.85, -25, 20, System.Drawing.Color.Black);
        // plt.AddText("Sichtachsenbewegung", 8.27, 125, 20, System.Drawing.Color.Black);
        // plt.AddText("Einzelradbewegung", 8.6, 125, 20, System.Drawing.Color.Red);

        ScottPlot.FormsPlotViewer win = new(plt);
        Task.Run(() => win.ShowDialog());
    }

    private (double nodeOne, double nodeTwo) Average_Interval_Seperated(List<Sample> record)
    {
        double one = record.Select(r => r.NodeOne_PacketInterval).Average();
        double two = record.Select(r => r.NodeTwo_PacketInterval).Average();
        return (one, two);
    }

    private double Average_Interval(List<Sample> record)
    {
        var one = record.Select(r => r.NodeOne_PacketInterval);
        var two = record.Select(r => r.NodeTwo_PacketInterval);
        return one.Concat(two).Average();
    }

    private double Max_Interval(List<Sample> record)
    {
        var one = record.Select(r => r.NodeOne_PacketInterval);
        var two = record.Select(r => r.NodeTwo_PacketInterval);
        return one.Concat(two).Max();
    }
}