using System;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing.Views;

public abstract class BaseView : IRenderable
{
    public Point Position { get; set; }
    public Color Color { get; set; }
    public abstract bool IsVisible { get; }

    public BaseView(Point position, Color color)
    {
        Position = position;
        Color = color;
    }

    public abstract void Render(CanvasInfo info);
}
