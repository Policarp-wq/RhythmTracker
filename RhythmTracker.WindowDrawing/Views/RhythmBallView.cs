using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing.Views;

public class RhythmBallView : BaseView
{
    public readonly double Radius;
    public double Velocity { get; set; }

    public override bool IsVisible => true;

    public RhythmBallView(Point position, double radius, Color color)
        : base(position, color)
    {
        Radius = radius;
    }

    public override void Render(CanvasInfo info)
    {
        if (!IsVisible)
            return;
        Drawings.DrawCircle(info, new(Position.X, Position.Y, Radius), Color);
    }

    public override object Clone()
    {
        return new RhythmBallView((Point)Position.Clone(), Radius, Color);
    }
}
