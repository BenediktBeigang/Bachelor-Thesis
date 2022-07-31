using NUnit.Framework;

namespace Wheelchair_Hub.Test;

public class RealisticWheelchairTests
{
    private RealisticWheelchair? Transformation;
    [SetUp]
    public void Setup()
    {
        Transformation = new RealisticWheelchair(30, 55.5);
    }

    #region Case 1 - DualWheel_SameDirection
    [Test]
    public void NextTransformedValues_Forward_CorrectForward()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = 100;
        double right = 100;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.EqualTo(100));
        Assert.That(result.Item2, Is.EqualTo(0));
    }

    [Test]
    public void NextTransformedValues_Backwards_CorrectBackwards()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = -100;
        double right = -100;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.EqualTo(-100));
        Assert.That(result.Item2, Is.EqualTo(0));
    }
    #endregion

    #region Case 2 - DualWheel_DirectionsAgainstEachOther
    [Test]
    public void NextTransformedValues_LeftTurn_CorrectLeftRotation()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = -100;
        double right = 100;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.EqualTo(0));
        Assert.That(result.Item2, Is.AtLeast(-207));
        Assert.That(result.Item2, Is.AtMost(-206));
    }

    [Test]
    public void NextTransformedValues_RightTurn_CorrectRightRotation()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = 100;
        double right = -100;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.EqualTo(0));
        Assert.That(result.Item2, Is.AtLeast(206));
        Assert.That(result.Item2, Is.AtMost(207));
    }
    #endregion

    #region Case 3 - SingleWheel
    [Test]
    public void NextTransformedValues_SingleWheelLeftForeward_CorrectLeftTurnForewards()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = 0;
        double right = 100;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.AtLeast(49.5));
        Assert.That(result.Item1, Is.AtMost(50.5));
        Assert.That(result.Item2, Is.AtLeast(-104));
        Assert.That(result.Item2, Is.AtMost(-103));
    }

    [Test]
    public void NextTransformedValues_SingleWheelRightForeward_CorrectRightTurnForewards()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = 100;
        double right = 0;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.AtLeast(49.5));
        Assert.That(result.Item1, Is.AtMost(50.5));
        Assert.That(result.Item2, Is.AtLeast(103));
        Assert.That(result.Item2, Is.AtMost(104));
    }

    [Test]
    public void NextTransformedValues_SingleWheelLeftBackward_CorrectLeftTurnBackwards()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = 0;
        double right = -100;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.AtMost(-49.5));
        Assert.That(result.Item1, Is.AtLeast(-50.5));
        Assert.That(result.Item2, Is.AtLeast(103));
        Assert.That(result.Item2, Is.AtMost(104));
    }

    [Test]
    public void NextTransformedValues_SingleWheelRightBackward_CorrectRightTurnBackwards()
    {
        short rawleft = 0;
        short rawright = 0;
        double left = -100;
        double right = 0;

        (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

        Assert.That(result.Item1, Is.AtMost(-49.5));
        Assert.That(result.Item1, Is.AtLeast(-50.5));
        Assert.That(result.Item2, Is.AtLeast(-104));
        Assert.That(result.Item2, Is.AtMost(-103));
    }
    #endregion
}