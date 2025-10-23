using System;

namespace RhythmTracker.WindowDrawing.Drawing;

public static class Drawings
{
    public static void DrawCircle(CanvasInfo info, CircleInfo circleInfo)
    {
        var context = info.Context;
        var (cx, cy, radius) = circleInfo;
        context.SetSourceRgb(0.8, 0, 0);
        context.Arc(cx, cy, radius, 0, 2 * Math.PI);
        context.LineTo(cx, cy);
        context.LineTo(cx + radius, cy);
        context.Fill();
    }
}
