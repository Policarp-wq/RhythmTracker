using System;

namespace RhythmTracker.WindowDrawing.Drawing;

public static class Drawings
{
    public static void DrawCircle(CanvasInfo info, CircleInfo circleInfo, Color color)
    {
        var context = info.Context;
        var (cx, cy, radius) = circleInfo;
        context.SetSourceRgb(color.Red, color.Green, color.Blue);
        context.Arc(cx, cy, radius, 0, 2 * Math.PI);
        context.LineTo(cx, cy);
        context.LineTo(cx + radius, cy);
        context.Fill();
    }
}
