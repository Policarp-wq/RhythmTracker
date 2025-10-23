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
    private List<IRenderable> _renderables = [];
    private DrawingFuncType? _background;

    public Canvas(DrawingFuncType? backgroundFunc = null)
        : base([])
    {
        _background = backgroundFunc;
        SetDrawFunc(MainDrawFunc);
    }

    public void AddRenderable(IRenderable renderable)
    {
        _renderables.Add(renderable);
    }

    public async Task StartRefreshing(int fps, Action onFrame, CancellationToken token)
    {
        Log.Info("Canvas started refreshing");
        const int second = 1000;
        int delay = second / fps;
        if (delay <= 0)
            delay = 0;
        try
        {
            while (true)
            {
                token.ThrowIfCancellationRequested();
                Render();
                onFrame();
                await Task.Delay(delay, token);
            }
        }
        catch (OperationCanceledException) { }
        Log.Info("Canvas stopped refreshing");
    }

    private void MainDrawFunc(DrawingArea drawingArea, Cairo.Context cr, int width, int height)
    {
        CanvasInfo info = new(drawingArea, cr, width, height);
        _background?.Invoke(info);
        _currentDrawingFunc?.Invoke(info);
        foreach (var el in _renderables)
        {
            el.Render(info);
        }
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
