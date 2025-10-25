namespace RhythmTracker.WindowDrawing.Drawing;

public struct Color
{
    public double Red { get; }
    public double Green { get; }
    public double Blue { get; }

    public Color(double red, double green, double blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    public static readonly Color RedColor = new(1, 0, 0);
    public static readonly Color GreenColor = new(0, 1, 0);
    public static readonly Color BlueColor = new(0, 0, 1);
}
