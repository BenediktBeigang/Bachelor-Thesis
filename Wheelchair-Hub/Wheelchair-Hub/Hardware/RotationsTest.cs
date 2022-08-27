public class RotationsTest
{
    public readonly short[] RawLeft_Buffer;
    public short RawLeft { get { return RawLeft_Buffer[0]; } }
    public double AngularVelocityLeft { get { return RawLeft_Buffer[0] / StepsPerDegree; } }
    public double[] AngularVelocityLeft_Buffer { get { return Get_AngularVelocityBuffer(RawLeft_Buffer); } }

    public readonly short[] RawRight_Buffer;
    public short RawRight { get { return RawRight_Buffer[0]; } }
    public double AngularVelocityRight { get { return RawRight_Buffer[0] / StepsPerDegree; } }
    public double[] AngularVelocityRight_Buffer { get { return Get_AngularVelocityBuffer(RawRight_Buffer); } }

    private readonly double StepsPerDegree;

    public RotationsTest(short[] leftBuffer, short[] rightBuffer, double stepsPerDegree)
    {
        RawLeft_Buffer = leftBuffer;
        RawRight_Buffer = rightBuffer;
        StepsPerDegree = stepsPerDegree;
    }

    public RotationsTest()
    {
        RawLeft_Buffer = new short[] { 0 };
        RawRight_Buffer = new short[] { 0 };
        StepsPerDegree = 1;
    }

    private double[] Get_AngularVelocityBuffer(short[] rawBuffer)
    {
        double[] aV_Buffer = new double[rawBuffer.Length];
        for (int i = 0; i < rawBuffer.Length; i++)
        {
            aV_Buffer[i] = rawBuffer[i] / StepsPerDegree;
        }
        return aV_Buffer;
    }

    public void MuteLeft()
    {
        Array.Clear(RawLeft_Buffer);
    }

    public void MuteRight()
    {
        Array.Clear(RawRight_Buffer);
    }

}
