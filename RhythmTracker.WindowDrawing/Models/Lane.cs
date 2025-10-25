using System;
using Gtk;
using RhythmTracker.AudioManagement.RhythmHandlers;
using RhythmTracker.WindowDrawing.Drawing;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing.Models;

public class Lane : IRenderable, IRhythmAdjustable, IMovable, IConsumer
{
    public readonly RectangleView View;
    private readonly EndlineView _endline;
    private readonly Scene<RhythmBall> _rhythms;
    private readonly Scene _static;
    public readonly double EpsMs;
    private readonly RhythmBallView _rhythmBallViewParent;

    public bool IsVisible { get; set; } = true;

    public bool IsReadyToConsume => true;
    private const int ENDLINE_WIDTH = 3;
    public event Action OnHit;

    public Lane(RectangleView view, Point xEndLine, Color endlineColor, double epsMs)
    {
        View = view;
        _rhythms = [];
        EpsMs = epsMs;
        double radius = view.Height / 2.05;
        Point initPosition = new(View.Position.X + radius, View.Position.Y + radius);
        _endline = new(xEndLine, ENDLINE_WIDTH, view.Height, endlineColor);
        _static = [View, _endline];
        _rhythmBallViewParent = new(initPosition, radius, new(1, 0, 0));
        OnHit += () =>
        {
            _endline.SwitchColorForPeriod(Color.RedColor, 20);
        };
    }

    private static Point GetEndlinePointByOffset(RectangleView view, int endlineOffsetPercent)
    {
        double x =
            view.Position.X + view.Width * (100 - endlineOffsetPercent) / 100 - ENDLINE_WIDTH;
        double y = view.Position.Y;
        return new(x, y);
    }

    public Lane(RectangleView view, int endlineOffsetPercent, Color endlineColor, double epsMs)
        : this(view, GetEndlinePointByOffset(view, endlineOffsetPercent), endlineColor, epsMs) { }

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

    private double GetDistanceToFinish(double curX)
    {
        return _endline.Position.X - curX;
    }

    private double GetRhythmBallEndpoint(RhythmBall ball)
    {
        return ball.View.Position.X + ball.View.Radius;
    }

    public void Adjust(double currentPosition, int FPS)
    {
        List<int> destroy = [];
        foreach (var (key, rhythm) in _rhythms.Entries)
        {
            double distance = GetDistanceToFinish(GetRhythmBallEndpoint(rhythm));
            if (rhythm.TargetTime - currentPosition < EpsMs / 1000 || distance < 0)
            {
                destroy.Add(key);
                OnHit.Invoke();
                continue;
            }
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
        _static.Render(info);
        _rhythms.Render(info);
    }

    public void Move()
    {
        foreach (var rhythm in _rhythms)
            rhythm.Move();
    }

    public void Consume(double trackPosition)
    {
        SpawnRhythm(trackPosition);
    }
}
