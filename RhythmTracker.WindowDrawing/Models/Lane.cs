using System;
using Gtk;
using RhythmTracker.WindowDrawing.Drawing;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing.Models;

public class Lane : IRenderable, IRhythmAdjustable, IMovable
{
    public readonly LaneView View;
    private readonly Scene<RhythmBall> _rhythms;
    public readonly double EpsMs;
    private readonly RhythmBallView _rhythmBallViewParent;

    public bool IsVisible { get; set; } = true;

    public Lane(LaneView view, double epsMs)
    {
        View = view;
        _rhythms = [];
        EpsMs = epsMs;
        double radius = view.Height / 2.05;
        Point initPosition = new(View.Position.X + radius, View.Position.Y + radius);
        _rhythmBallViewParent = new(initPosition, radius, new(1, 0, 0));
    }

    public void SpawnRhythm(double targetPosition)
    {
        RhythmBall rhythm = new(
            (RhythmBallView)_rhythmBallViewParent.Clone(),
            Axis.X,
            targetPosition,
            0
        );
        _rhythms.Add(rhythm);
    }

    public void Adjust(double currentPosition, int FPS)
    {
        List<int> destroy = [];
        foreach (var (key, rhythm) in _rhythms.Entries)
        {
            if (!rhythm.IsVisible)
                continue;
            if (
                rhythm.TargetTime - currentPosition < EpsMs / 1000
                || rhythm.View.Position.X + rhythm.View.Radius > View.Position.X + View.Width
            )
            {
                destroy.Add(key);
                continue;
            }
            double distance =
                View.Position.X + View.Width - rhythm.View.Position.X - rhythm.View.Radius;
            double velocity = distance / (rhythm.TargetTime - currentPosition) / FPS;
            rhythm.ChangeVelocity(velocity);
        }
        foreach (var k in destroy)
            _rhythms.Remove(k);
    }

    public void Render(CanvasInfo info)
    {
        if (!IsVisible)
            return;
        View.Render(info);
        _rhythms.Render(info);
    }

    public void Move()
    {
        foreach (var rhythm in _rhythms)
            rhythm.Move();
    }
}
