using System;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing;

public class RhythmVisualizer : IRenderable
{
    public readonly double MxRadius;
    private readonly double _cx;
    private readonly double _cy;
    public readonly Color Color;

    public RhythmVisualizer(double mxRadius, double cx, double cy, Color color)
    {
        MxRadius = mxRadius;
        _cx = cx;
        _cy = cy;
        Color = color;
    }

    public double CurrentRadius { get; private set; }
    public double Delta { get; set; }

    public void AdjustDelta(double targetTime)
    {
        Delta = (MxRadius - CurrentRadius) / targetTime;
    }

    public void IncreaseRadius()
    {
        CurrentRadius += Delta;
        if (CurrentRadius > MxRadius)
            CurrentRadius = MxRadius;
    }

    public void Reset()
    {
        CurrentRadius = 0;
    }

    public void Render(CanvasInfo info)
    {
        Drawings.DrawCircle(info, new(_cx, _cy, CurrentRadius), Color);
    }
}
