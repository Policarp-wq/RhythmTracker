using System;

namespace RhythmTracker.WindowDrawing.Drawing;

public class Point : ICloneable
{
    public double X { get; set; }
    public double Y { get; set; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }

    public object Clone() => new Point(X, Y);
}
