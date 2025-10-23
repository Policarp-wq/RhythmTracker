using System;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing;

public class RhythmVisualizer
{
    public readonly double MxRadius;
    private readonly double _cx;
    private readonly double _cy;

    public RhythmVisualizer(double mxRadius, double cx, double cy)
    {
        MxRadius = mxRadius;
        _cx = cx;
        _cy = cy;
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

    public void GetDrawingFunc(CanvasInfo info)
    {
        Drawings.DrawCircle(info, new(_cx, _cy, CurrentRadius));
    }
}
