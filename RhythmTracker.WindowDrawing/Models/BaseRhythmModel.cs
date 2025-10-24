using System;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing.Models;

public abstract class BaseRhythmModel : IMovable, IRenderable
{
    public abstract void Move();
    public abstract void Render(CanvasInfo info);
    public virtual bool IsVisible { get; protected set; }
}
