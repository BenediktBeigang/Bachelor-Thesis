public class Plot_GyroSignalOld
{
    public Plot_GyroSignalOld(List<Sample> record)
    {
        double[] x_One = record.Select(r => r.NodeOne_Value).ToArray();
        double[] a1 = new double[] { 1, 1, 1 };
        double[] b1 = new double[] { 0.2, 0.3, 0.5 };
        double[] a2 = new double[] { 1, 1, 1 };
        double[] b2 = new double[] { 0.063, 0.125, 0.063 };
        double[] inputSignal = record.Select(r => r.NodeOne_Value).ToArray();
        double[] x_Two = Filter2(inputSignal);//Filter(a1, b1, inputSignal);
        // double[] x_Two = Filter3(inputSignal, 60, 100);
        //x_Two = Filter(a2, b2, inputSignal);

        string output = "";
        foreach (double d in x_Two)
        {
            output += d.ToString() + '\n';
        }
        File.WriteAllText("values.txt", output);

        for (int i = 0; i < x_Two.Length; i++)
        {
            if (double.IsNormal(x_Two[i]) is false) x_Two[i] = 0;
        }

        var plt = new ScottPlot.Plot(1600, 900);
        plt.XLabel("Time [s]");
        plt.Title($"AngularVelocity raw/smoothed");

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

    private static double[] Filter2(double[] signal)
    {
        double[] scale = new double[] { 0.2, 0.2, 0.15, 0.15, 0.1, 0.1, 0.1 };

        double[] result = new double[signal.Length];
        for (int i = 0; i < signal.Length; i++)
        {
            double filteredValue = 0;
            for (int j = 0; j < scale.Length; j++)
            {
                filteredValue += (i - j < 0) ? 0 : signal[i - j] * scale[j];
            }
            result[i] = filteredValue;
            // result[i] = signal[i] * 0.5 + signal[i - 1] * 0.3 + signal[i - 2] * 0.2;
        }
        return result;
    }

    // Butterworth
    public static double[] Filter3(double[] indata, double deltaTimeinsec, double CutOff)
    {
        if (indata == null) return null;
        if (CutOff == 0) return indata;

        double Samplingrate = 1 / deltaTimeinsec;
        long dF2 = indata.Length - 1;        // The data range is set with dF2
        double[] Dat2 = new double[dF2 + 4]; // Array with 4 extra points front and back
        double[] data = indata; // Ptr., changes passed data

        // Copy indata to Dat2
        for (long r = 0; r < dF2; r++)
        {
            Dat2[2 + r] = indata[r];
        }
        Dat2[1] = Dat2[0] = indata[0];
        Dat2[dF2 + 3] = Dat2[dF2 + 2] = indata[dF2];

        const double pi = 3.14159265358979;
        double wc = Math.Tan(CutOff * pi / Samplingrate);
        double k1 = 1.414213562 * wc; // Sqrt(2) * wc
        double k2 = wc * wc;
        double a = k2 / (1 + k1 + k2);
        double b = 2 * a;
        double c = a;
        double k3 = b / k2;
        double d = -2 * a + k3;
        double e = 1 - (2 * a) - k3;

        // RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
        double[] DatYt = new double[dF2 + 4];
        DatYt[1] = DatYt[0] = indata[0];
        for (long s = 2; s < dF2 + 2; s++)
        {
            DatYt[s] = a * Dat2[s] + b * Dat2[s - 1] + c * Dat2[s - 2]
                       + d * DatYt[s - 1] + e * DatYt[s - 2];
        }
        DatYt[dF2 + 3] = DatYt[dF2 + 2] = DatYt[dF2 + 1];

        // FORWARD filter
        double[] DatZt = new double[dF2 + 2];
        DatZt[dF2] = DatYt[dF2 + 2];
        DatZt[dF2 + 1] = DatYt[dF2 + 3];
        for (long t = -dF2 + 1; t <= 0; t++)
        {
            DatZt[-t] = a * DatYt[-t + 2] + b * DatYt[-t + 3] + c * DatYt[-t + 4]
                        + d * DatZt[-t + 1] + e * DatZt[-t + 2];
        }

        // Calculated points copied for return
        for (long p = 0; p < dF2; p++)
        {
            data[p] = DatZt[p];
        }

        return data;
    }

    private static double[] Filter(double[] a, double[] b, double[] signal)
    {
        double[] result = new double[signal.Length];
        for (int i = 0; i < signal.Length; ++i)
        {
            double tmp = 0.0;
            for (int j = 0; j < b.Length; ++j)
            {
                if (i - j < 0) continue;
                tmp += b[j] * signal[i - j];
            }
            for (int j = 1; j < a.Length; ++j)
            {
                if (i - j < 0) continue;
                tmp -= a[j] * result[i - j];
            }
            tmp /= a[0];
            result[i] = tmp;
        }
        return result;
    }


}