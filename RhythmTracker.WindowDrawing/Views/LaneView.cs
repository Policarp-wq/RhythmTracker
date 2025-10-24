using System;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing.Views;

public class LaneView : BaseView
{
    public readonly double Width;
    public readonly double Height;

    public LaneView(Point position, double width, double height, Color color)
        : base(position, color)
    {
        Width = width;
        Height = height;
    }

    public override bool IsVisible => true;

    public override void Render(CanvasInfo info)
    {
        if (!IsVisible)
            return;
        info.Context.SetSourceRgb(Color);
        info.Context.Rectangle(Position.X, Position.Y, Width, Height);
        info.Context.Fill();
    }
}
