using System;
using Gtk;
using RhythmTracker.AudioManagement.AudioManager;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.AudioManagement.RhythmHandlers;

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
                cr.SetSourceRgb(0, 0, 0.2);
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
            _window.DefaultWidth / 2.0
        );
    }

    public async Task Run()
    {
        await _pulser.FillBuffer();
        _currentPulse = await _pulser.GetNextPulse();
        _nextPulse = await _pulser.GetNextPulse();
        AudioContext audioContext = new(_mpvApi);

        int refreshMs = 50;
        _canvas.UpdateDrawingFunc(_visualizer.GetDrawingFunc);
        audioContext.OnPositionGot += async (pos) => await OnPositionGot(pos, refreshMs);

        _canvas.StartRefreshing(FPS, OnFrame);

        CancellationTokenSource tokenSource = new();
        await _mpvApi.StartPlayingAudio(tokenSource.Token, 250);
        await audioContext.TrackAudioTime(
            refreshMs,
            () => Console.WriteLine("\nPlayback finished. Press any key to exit.")
        );
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
                _canvas.UpdateDrawingFunc(_visualizer.GetDrawingFunc);
                return;
            }
        }
        _visualizer.AdjustDelta((_currentPulse.Value - curPos) * FPS);
        // _canvas.UpdateDrawingFunc(_visualizer.GetDrawingFunc);
    }

    public void OnFrame()
    {
        _visualizer.IncreaseRadius();
        _canvas.Render();
    }

    public void Dispose()
    {
        _pulser.Dispose();
        _mpvApi.Dispose();
        GC.SuppressFinalize(this);
        Log.Info("Rhythm canvas disposed");
    }
}
