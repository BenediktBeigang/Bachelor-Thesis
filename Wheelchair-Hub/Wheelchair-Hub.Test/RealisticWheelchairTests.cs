// using NUnit.Framework;

// namespace Wheelchair_Hub.Test;

// public class RealisticWheelchairTests
// {
//     private RealisticWheelchair? Transformation;
//     [SetUp]
//     public void Setup()
//     {
//         Transformation = new RealisticWheelchair(30, 55.5, 100, 25);
//     }

//     #region Case 1 - DualWheel_SameDirection
//     [Test]
//     public void ValuesNext_Forward_CorrectForward()
//     {
//         Rotations rot = new Rotations(0, 0, 100, 100);

//         ControllerInput result = Transformation!.Values_Next(rot);

//         Assert.That(result.LeftThumbY, Is.EqualTo(3276));
//         Assert.That(result.RightThumbX, Is.EqualTo(0));
//     }

//     [Test]
//     public void ValuesNext_Backwards_CorrectBackwards()
//     {
//         Rotations rot = new Rotations(, 1000, -30.51, -30.51);

//         ControllerInput result = Transformation!.Values_Next(rot);

//         Assert.That(result.LeftThumbY, Is.EqualTo(-3276));
//         Assert.That(result.RightThumbX, Is.EqualTo(0));
//     }
//     #endregion

//     #region Case 2 - DualWheel_DirectionsAgainstEachOther
//     [Test]
//     public void ValuesNext_LeftTurn_CorrectLeftRotation()
//     {
//         Rotations rot = new Rotations(0, 0, -100, 100);

//         ControllerInput result = Transformation!.Values_Next(rot);

//         Assert.That(result.RightThumbX, Is.EqualTo(0));


//         Assert.That(result.LeftThumbY, Is.EqualTo(0));
//         Assert.That(result.RightThumbX, Is.EqualTo(-3276));
//     }

//     // [Test]
//     // public void NextTransformedValues_RightTurn_CorrectRightRotation()
//     // {
//     //     short rawleft = 0;
//     //     short rawright = 0;
//     //     double left = 100;
//     //     double right = -100;

//     //     (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

//     //     Assert.That(result.Item1, Is.EqualTo(0));
//     //     Assert.That(result.Item2, Is.AtLeast(206));
//     //     Assert.That(result.Item2, Is.AtMost(207));
//     // }
//     #endregion

//     // #region Case 3 - SingleWheel
//     // [Test]
//     // public void NextTransformedValues_SingleWheelLeftForeward_CorrectLeftTurnForewards()
//     // {
//     //     short rawleft = 0;
//     //     short rawright = 0;
//     //     double left = 0;
//     //     double right = 100;

//     //     (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

//     //     Assert.That(result.Item1, Is.AtLeast(49.5));
//     //     Assert.That(result.Item1, Is.AtMost(50.5));
//     //     Assert.That(result.Item2, Is.AtLeast(-104));
//     //     Assert.That(result.Item2, Is.AtMost(-103));
//     // }

//     // [Test]
//     // public void NextTransformedValues_SingleWheelRightForeward_CorrectRightTurnForewards()
//     // {
//     //     short rawleft = 0;
//     //     short rawright = 0;
//     //     double left = 100;
//     //     double right = 0;

//     //     (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

//     //     Assert.That(result.Item1, Is.AtLeast(49.5));
//     //     Assert.That(result.Item1, Is.AtMost(50.5));
//     //     Assert.That(result.Item2, Is.AtLeast(103));
//     //     Assert.That(result.Item2, Is.AtMost(104));
//     // }

//     // [Test]
//     // public void NextTransformedValues_SingleWheelLeftBackward_CorrectLeftTurnBackwards()
//     // {
//     //     short rawleft = 0;
//     //     short rawright = 0;
//     //     double left = 0;
//     //     double right = -100;

//     //     (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

//     //     Assert.That(result.Item1, Is.AtMost(-49.5));
//     //     Assert.That(result.Item1, Is.AtLeast(-50.5));
//     //     Assert.That(result.Item2, Is.AtLeast(103));
//     //     Assert.That(result.Item2, Is.AtMost(104));
//     // }

//     // [Test]
//     // public void NextTransformedValues_SingleWheelRightBackward_CorrectRightTurnBackwards()
//     // {
//     //     short rawleft = 0;
//     //     short rawright = 0;
//     //     double left = -100;
//     //     double right = 0;

//     //     (double, double) result = Transformation!.TransformedValues_Next(rawleft, rawright, left, right);

//     //     Assert.That(result.Item1, Is.AtMost(-49.5));
//     //     Assert.That(result.Item1, Is.AtLeast(-50.5));
//     //     Assert.That(result.Item2, Is.AtLeast(-104));
//     //     Assert.That(result.Item2, Is.AtMost(-103));
//     // }
//     // #endregion
// }