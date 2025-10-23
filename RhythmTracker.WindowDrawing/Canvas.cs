using System;
using System.ComponentModel;
using GObject;
using Gtk;
using RhythmTracker.AudioManagement.Logging;
using DrawingFuncType = System.Action<RhythmTracker.WindowDrawing.CanvasInfo>;

namespace RhythmTracker.WindowDrawing;

public class Canvas : DrawingArea
{
    private DrawingFuncType? _currentDrawingFunc;
    private DrawingFuncType? _background;

    public Canvas(DrawingFuncType? backgroundFunc = null)
        : base([])
    {
        _background = backgroundFunc;
        SetDrawFunc(MainDrawFunc);
    }

    public async Task StartRefreshing(int fps, Action onFrame)
    {
        Log.Info("Canvas started refreshing");
        const int second = 1000;
        int delay = second / fps;
        while (true)
        {
            onFrame();
            await Task.Delay(delay);
        }
    }

    private void MainDrawFunc(DrawingArea drawingArea, Cairo.Context cr, int width, int height)
    {
        CanvasInfo info = new(drawingArea, cr, width, height);
        _background?.Invoke(info);
        _currentDrawingFunc?.Invoke(info);
    }

    public void UpdateDrawingFunc(DrawingFuncType? drawing)
    {
        _currentDrawingFunc = drawing;
    }

    public void Render()
    {
        QueueDraw();
    }
}
