using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing.Views;

public class RhythmBallView : BaseView
{
    public readonly double Radius;

    public RhythmBallView(Point position, double radius, Color color)
        : base(position, color)
    {
        Radius = radius;
    }

    public override void Render(CanvasInfo info)
    {
        Drawings.DrawCircle(info, new(Position.X, Position.Y, Radius), Color);
    }
}
