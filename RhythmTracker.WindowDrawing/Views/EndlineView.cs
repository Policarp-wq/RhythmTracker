using System;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.WindowDrawing.Views;

public class EndlineView : RectangleView
{
    public readonly Color PrimaryColor;

    public EndlineView(Point position, double width, double height, Color color)
        : base(position, width, height, color)
    {
        PrimaryColor = color;
    }

    private int _renderTimesLeftToSwitchBack = 0;

    public void SwitchColorForPeriod(Color color, int renderTimesPersist)
    {
        Color = color;
        _renderTimesLeftToSwitchBack = renderTimesPersist;
    }

    public override void Render(CanvasInfo canvasInfo)
    {
        if (_renderTimesLeftToSwitchBack <= 0)
        {
            Color = PrimaryColor;
        }
        else
            _renderTimesLeftToSwitchBack--;
        base.Render(canvasInfo);
    }
}
