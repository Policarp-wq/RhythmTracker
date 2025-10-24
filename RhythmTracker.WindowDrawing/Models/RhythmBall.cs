using System;
using RhythmTracker.WindowDrawing.Drawing;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing.Models;

public class RhythmBall : BaseRhythmModel
{
    public readonly RhythmBallView View;
    public Axis Axis;
    public double Velocity { get; set; }

    public override bool IsVisible { get; protected set; } = true;

    public readonly double TargetTime;

    public RhythmBall(RhythmBallView view, Axis axis, double targetTime, double velocity)
    {
        View = view;
        Axis = axis;
        TargetTime = targetTime;
        Velocity = velocity;
    }

    public void ChangeVelocity(double velocity)
    {
        Velocity = velocity;
    }

    public override void Move()
    {
        switch (Axis)
        {
            case Axis.X:
                View.Position.X += Velocity;
                break;
            case Axis.Y:
                View.Position.Y += Velocity;
                break;
            case Axis.XY:
                View.Position.X += Velocity;
                View.Position.Y += Velocity;
                break;
        }
    }

    public override void Render(CanvasInfo info)
    {
        View.Render(info);
    }
}
