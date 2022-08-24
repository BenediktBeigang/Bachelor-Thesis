// using System.Timers;

// public class Benchmark
// {
//     private static System.Timers.Timer? Timer_EverySecond;
//     private const int TIME_BETWEEN_CALLS = 1000;

//     public Benchmark()
//     {
//         Timer();
//     }

//     private void Timer()
//     {
//         Timer_EverySecond = new System.Timers.Timer(TIME_BETWEEN_CALLS);
//         Timer_EverySecond.Elapsed += Datarate!;
//         Timer_EverySecond.AutoReset = true;
//         Timer_EverySecond.Enabled = true;
//     }

//     private static void Datarate(object sender, ElapsedEventArgs e)
//     {
//         if (Node.Node_One.ConnectionType is not ConnectionType.NOTHING)
//             Node.Node_One.Update_DataRate(TIME_BETWEEN_CALLS);
//         if (Node.Node_Two.ConnectionType is not ConnectionType.NOTHING)
//             Node.Node_Two.Update_DataRate(TIME_BETWEEN_CALLS);
//     }
// }