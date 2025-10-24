using System;
using Cairo;

namespace RhythmTracker.WindowDrawing.Drawing;

public static class ContextExtensions
{
    public static void SetSourceRgb(this Context context, Color color)
    {
        context.SetSourceRgb(color.Red, color.Green, color.Blue);
    }
}
