public class Mouse : ValueTransformation
{
    public Mouse() : base(0, 0)
    {

    }

    public override (double, double) TransformedValues_Next(short rawValue_One, short rawValue_Two, double value_One, double value_Two)
    {
        throw new NotImplementedException();
    }
}