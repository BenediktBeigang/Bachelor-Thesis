public class ControllerInput
{
    public short LeftThumbX;
    public short LeftThumbY;
    public short RightThumbX;
    public short RightThumbY;

    public bool A;
    public bool B;
    public bool X;
    public bool Y;

    public ControllerInput()
    {
        LeftThumbX = 0;
        LeftThumbY = 0;
        RightThumbX = 0;
        RightThumbY = 0;
        A = false;
        B = false;
        X = false;
        Y = false;
    }

    public ControllerInput(short leftThumbX, short leftThumbY, short rightThumbX, short rightThumbY, bool a, bool b, bool x, bool y)
    {
        LeftThumbX = leftThumbX;
        LeftThumbY = leftThumbY;
        RightThumbX = rightThumbX;
        RightThumbY = rightThumbY;
        A = a;
        B = b;
        X = x;
        Y = y;
    }

    public static ControllerInput operator +(ControllerInput a, ControllerInput b)
    => new ControllerInput(
        (short)(a.LeftThumbX + b.LeftThumbX), (short)(a.LeftThumbY + b.LeftThumbY),
        (short)(a.RightThumbX + b.RightThumbX), (short)(a.RightThumbY + b.RightThumbY),
        a.A || b.A, a.B || b.B,
        a.X || b.X, a.Y || b.Y);

    public override string ToString()
    {
        string output = "";
        output += $"LeftThumbX: {LeftThumbX}\n";
        output += $"LeftThumbY: {LeftThumbY}\n";
        output += $"RightThumbX: {RightThumbX}\n";
        output += $"RightThumbY: {RightThumbY}\n";
        output += $"A: {A}\n";
        output += $"B: {B}\n";
        output += $"X: {X}\n";
        output += $"Y: {Y}";
        return output;
    }
}