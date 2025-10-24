using System;
using Gtk;
using RhythmTracker.AudioManagement.AudioManager;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.AudioManagement.RhythmHandlers;
using RhythmTracker.WindowDrawing.Drawing;
using RhythmTracker.WindowDrawing.Models;
using RhythmTracker.WindowDrawing.Views;

namespace RhythmTracker.WindowDrawing;

public class RhythmCanvas : IDisposable
{
    private readonly ApplicationWindow _window;
    private readonly MpvApi _mpvApi;
    private readonly RhythmPulser _pulser;
    private readonly Canvas _canvas;
    public readonly int FPS;
    private Scene _scene;

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
        LaneView laneView = new(new(0, 200), window.DefaultWidth, 60, new(0, 1, 0));
        Lane lane = new(laneView, 50);

        lane.SpawnRhythm(7);
        lane.SpawnRhythm(5);
        lane.SpawnRhythm(3);
        lane.SpawnRhythm(1);
        _scene = [lane];
        _canvas.Scene = _scene;
    }

    public async Task Run(CancellationToken token)
    {
        await _pulser.FillBuffer();
        AudioContext audioContext = new(_mpvApi);

        int refreshMs = 50;
        using CancellationTokenSource refreshEndToken = new();
        audioContext.OnPositionGot += OnPositionGot;

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

    public void OnFrame()
    {
        foreach (var el in _scene)
        {
            if (el is IMovable movable)
            {
                movable.Move();
            }
        }
    }

    public Task OnPositionGot(double curPos)
    {
        foreach (var el in _scene)
        {
            if (el is IRhythmAdjustable adjustable)
            {
                adjustable.Adjust(curPos, FPS);
            }
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _pulser.Dispose();
        _mpvApi.Dispose();
        GC.SuppressFinalize(this);
        Log.Info("Rhythm canvas disposed");
    }
}
