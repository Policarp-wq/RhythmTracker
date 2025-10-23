using System;
using Gtk;
using RhythmTracker.AudioManagement.AudioManager;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.AudioManagement.RhythmHandlers;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing;

public class RhythmCanvas : IDisposable
{
    private readonly ApplicationWindow _window;
    private readonly MpvApi _mpvApi;
    private readonly RhythmPulser _pulser;
    private readonly Canvas _canvas;
    public readonly int FPS;
    private readonly RhythmVisualizer _visualizer;
    private double? _currentPulse;
    private double? _nextPulse;

    public RhythmCanvas(
        ApplicationWindow window,
        string audioFile,
        string flagsFile,
        int refreshRate
    )
    {
        _window = window;
        _canvas = new Canvas(
            (info) =>
            {
                var (cr, width, height) = (info.Context, info.Width, info.Height);
                cr.SetSourceRgb(0, 0, 0.4);
                cr.Rectangle(0, 0, width, height);
                cr.Fill();
            }
        );
        _window.Child = _canvas;
        _mpvApi = new(audioFile);
        _pulser = new(_mpvApi, flagsFile, 200, 10);
        FPS = refreshRate;
        _visualizer = new(
            _window.DefaultHeight / 2.0,
            _window.DefaultWidth / 2.0,
            _window.DefaultWidth / 2.0,
            new(0.8, 0, 0.2)
        );
    }

    public async Task Run(CancellationToken token)
    {
        await _pulser.FillBuffer();
        _currentPulse = await _pulser.GetNextPulse();
        _nextPulse = await _pulser.GetNextPulse();
        AudioContext audioContext = new(_mpvApi);

        int refreshMs = 50;
        _canvas.AddRenderable(_visualizer);
        // _canvas.AddRenderable(new RhythmBallView(new(20, 20), 10, new(0, 0.2, 0.1)));
        // _canvas.AddRenderable(new RhythmBallView(new(40, 20), 10, new(0, 0, 0.1)));
        // _canvas.AddRenderable(new RhythmBallView(new(60, 20), 10, new(1, 0, 0)));
        using CancellationTokenSource refreshEndToken = new();
        audioContext.OnPositionGot += async (pos) => await OnPositionGot(pos, refreshMs);

        var refreshTask = Task.Run(
            () => _canvas.StartRefreshing(FPS, OnFrame, refreshEndToken.Token),
            token
        );
        await _mpvApi.StartPlayingAudio(token, 250);
        await audioContext.TrackAudioTime(
            refreshMs,
            token,
            () => Console.WriteLine("\nPlayback finished. Press any key to exit.")
        );
        refreshEndToken.Cancel();
        await refreshTask;
    }

    public async Task OnPositionGot(double curPos, double epsMs)
    {
        if (!_currentPulse.HasValue)
            return;
        if (_currentPulse.Value - curPos < epsMs / 1000)
        {
            _currentPulse = _nextPulse;
            _nextPulse = await _pulser.GetNextPulse();
            _visualizer.Reset();
            if (!_currentPulse.HasValue)
            {
                Log.Info("Pulsing is over");
                _visualizer.Delta = 0;
                _canvas.UpdateDrawingFunc(_visualizer.Render);
                return;
            }
        }
        _visualizer.AdjustDelta((_currentPulse.Value - curPos) * FPS);
        // _canvas.UpdateDrawingFunc(_visualizer.GetDrawingFunc);
    }

    public void OnFrame()
    {
        _visualizer.IncreaseRadius();
    }

    public void Dispose()
    {
        _pulser.Dispose();
        _mpvApi.Dispose();
        GC.SuppressFinalize(this);
        Log.Info("Rhythm canvas disposed");
    }
}
