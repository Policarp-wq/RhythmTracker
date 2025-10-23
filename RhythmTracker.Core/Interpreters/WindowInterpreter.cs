using Gtk;
using RhythmTracker.AudioManagement.AudioManager;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.AudioManagement.RhythmHandlers;
using RhythmTracker.WindowDrawing;
using RhythmTracker.WindowDrawing.Drawing;

namespace RhythmTracker.Core.Interpreters;

public class WindowInterpreter : IInterpreter, IDisposable
{
    public RhythmCanvas? _canvas;

    public async Task Play(CancellationToken token)
    {
        var app = Application.New("com.rhythm", Gio.ApplicationFlags.FlagsNone);
        app.OnActivate += async (_, _) => await CreateWindowWithCanvas(app, token);
        app.RunWithSynchronizationContext(null);
    }

    public async Task CreateWindowWithCanvas(Application app, CancellationToken token)
    {
        var window = ApplicationWindow.New(app);
        window.Title = "LOL";
        window.DefaultWidth = 450;
        window.DefaultHeight = 450;
        _canvas = new(window, "orchid.mp3", "flags", 50);
        window.Present();
        Log.Info("Created window");
        await _canvas.Run(token);
        Log.Info("Canvas is on");
    }

    public void Dispose()
    {
        _canvas?.Dispose();
        GC.SuppressFinalize(this);
        Log.Info(nameof(WindowInterpreter) + " disposed");
    }

    // public async Task PulseSpeedCorrector(
    //     double currentTrackPos,
    //     int refreshMs,
    //     int frames,
    //     RhythmPulser pulser
    // )
    // {
    //     if (!_currentPulse.HasValue)
    //     {
    //         return;
    //     }

    //     if (_currentPulse - currentTrackPos - (1.0 * refreshMs / 1000) < 0)
    //     {
    //         _radius = 0;
    //         _currentPulse = _nextPulse;
    //         _nextPulse = await pulser.GetNextPulse();
    //     }

    //     if (!_currentPulse.HasValue)
    //     {
    //         _delta = -100;
    //         _prevX = 0;
    //         _radius = 64;
    //         return;
    //     }

    //     _delta = (_targetRadius - _radius) / (_currentPulse.Value - currentTrackPos) / frames;
    // }

    // public void PulseDrawing(CanvasInfo info)
    // {
    //     var (context, width, height) = (info.Context, info.Width, info.Height);
    //     var (cx, cy) = (width / 2.0, height / 2.0);
    //     context.SetSourceRgb(1, 0.2, 0);
    //     if (_delta <= 0)
    //     {
    //         if (_prevX + _radius > width)
    //         {
    //             _prevX = 0;
    //         }

    //         if (_prevX == 0)
    //         {
    //             _prevX = _radius - 1;
    //         }

    //         cx = _prevX + 1;
    //         _prevX = cx;
    //         Drawings.DrawCircle(info, new(cx, cy, _radius));
    //         return;
    //     }

    //     Drawings.DrawCircle(info, new(cx, cy, _radius));
    //     _radius += _delta;
    //     if (_radius >= _targetRadius)
    //     {
    //         _radius = 0;
    //     }
    // }
}
