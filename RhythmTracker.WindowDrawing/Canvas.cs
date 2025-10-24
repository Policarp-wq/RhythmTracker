using System;
using System.ComponentModel;
using System.Diagnostics;
using GObject;
using Gtk;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.WindowDrawing.Drawing;
using RhythmTracker.WindowDrawing.Views;
using DrawingFuncType = System.Action<RhythmTracker.WindowDrawing.CanvasInfo>;

namespace RhythmTracker.WindowDrawing;

public class Canvas : DrawingArea
{
    private DrawingFuncType? _currentDrawingFunc;
    public Scene Scene;
    private DrawingFuncType? _background;

    public Canvas(DrawingFuncType? backgroundFunc = null)
        : base([])
    {
        Scene = [];
        _background = backgroundFunc;
        SetDrawFunc(MainDrawFunc);
    }

    public void AddRenderable(IRenderable renderable)
    {
        Scene.Add(renderable);
    }

    public async Task StartRefreshing(int fps, Action onFrame, CancellationToken token)
    {
        Log.Info("Canvas started refreshing");
        const int second = 1000;
        int delay = second / fps;
        if (delay <= 0)
            delay = 0;
        Stopwatch stopwatch = new();
        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                stopwatch.Restart();
                Render();
                onFrame();
                stopwatch.Stop();
                int frameTime = (int)stopwatch.ElapsedMilliseconds;
                int remaining = delay - frameTime;
                if (remaining > 0)
                    await Task.Delay(remaining, token);
            }
        }
        catch (OperationCanceledException) { }
        Log.Info("Canvas stopped refreshing");
    }

    private void MainDrawFunc(DrawingArea drawingArea, Cairo.Context cr, int width, int height)
    {
        CanvasInfo info = new(drawingArea, cr, width, height);
        _background?.Invoke(info);
        Scene.Render(info);
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
