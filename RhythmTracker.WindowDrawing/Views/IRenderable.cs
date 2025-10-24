using System;

namespace RhythmTracker.WindowDrawing.Views;

public interface IRenderable
{
    public void Render(CanvasInfo info);
    public bool IsVisible { get; }
}
