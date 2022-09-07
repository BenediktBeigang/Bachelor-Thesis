using NUnit.Framework;

namespace Wheelchair_Hub.Test;

public class GyroTests
{
    private Gyro? _Gyro;
    [SetUp]
    public void Setup()
    {
        _Gyro = new Gyro(GyroMode.GYRO_1000, DeviceNumber.ONE);
    }

    [Test]
    public void SmoothValue_ValidValues_CorrectReturnValue()
    {
        double[] unfilteredValues = new double[] { 7, 6, 5, 4, 3, 2, 1 };

        double result = _Gyro!.SmoothValue(unfilteredValues, 0);

        Assert.That(result, Is.AtLeast(4.54));
        Assert.That(result, Is.AtMost(4.56));
        // Assert.That(result.Item1, Is.EqualTo(100));
        // Assert.That(result.Item2, Is.EqualTo(0));
        Assert.That(true);
    }

    [Test]
    public void SmoothedDegreePerSecondLast_Test()
    {
        double[] unfilteredValues = new double[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        double[] result = _Gyro!.SmoothedDegreePerSecond_Last(2, unfilteredValues);

        Assert.That(result[0], Is.AtLeast(3.44));
        Assert.That(result[0], Is.AtMost(3.46));
        Assert.That(result[1], Is.AtLeast(4.44));
        Assert.That(result[1], Is.AtMost(4.46));
    }
}