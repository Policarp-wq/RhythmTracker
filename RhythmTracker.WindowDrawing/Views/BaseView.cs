using System;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing.Views;

public abstract class BaseView : IRenderable
{
    public Point Position;
    public Color Color;

    public BaseView(Point position, Color color)
    {
        Position = position;
        Color = color;
    }

    public abstract void Render(CanvasInfo info);
}
