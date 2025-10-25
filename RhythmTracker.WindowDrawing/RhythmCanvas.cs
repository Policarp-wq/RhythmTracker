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
    private readonly RhythmReader _reader;
    private readonly RhythmDistributer _distributer;
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
                cr.SetSourceRgb(Color.BlueColor);
                cr.Rectangle(0, 0, width, height);
                cr.Fill();
            }
        );
        _window.Child = _canvas;
        _mpvApi = new(audioFile);
        _reader = new(_mpvApi, flagsFile, 200, 10);
        _distributer = new(_reader, 6);
        FPS = refreshRate;
        RectangleView laneView = new(new(0, 100), window.DefaultWidth, 60, Color.GreenColor);
        RectangleView laneView2 = (RectangleView)laneView.Clone();
        laneView2.Position.Y = laneView.Position.Y + laneView.Height * 1.15;
        RectangleView laneView3 = (RectangleView)laneView.Clone();
        laneView3.Position.Y = laneView2.Position.Y + laneView2.Height * 1.15;
        Lane lane = new(laneView, 5, Color.BlueColor, 25);
        Lane lane2 = new(laneView2, 5, Color.BlueColor, 25);
        Lane lane3 = new(laneView3, 5, Color.BlueColor, 25);

        _distributer.AddConsumer(1, lane);
        _distributer.AddConsumer(0, lane2);
        _distributer.AddConsumer(-1, lane3);

        _scene = [lane, lane2, lane3];
        _canvas.Scene = _scene;
    }

    public async Task Run(CancellationToken token)
    {
        AudioContext audioContext = new(_mpvApi);
        await _distributer.FullBuffer();
        int refreshMs = 50;
        using CancellationTokenSource refreshEndToken = new();
        audioContext.OnPositionGot += OnPositionGot;

        var refreshTask = Task.Run(
            () => _canvas.StartRefreshing(FPS, OnFrame, refreshEndToken.Token),
            token
        );
        await _distributer.Distribute();
        await _distributer.Distribute();
        await _distributer.Distribute();

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

    public async Task OnPositionGot(double curPos)
    {
        foreach (var el in _scene)
        {
            if (el is IRhythmAdjustable adjustable)
            {
                adjustable.Adjust(curPos, FPS);
            }
        }
        if (_distributer.Next.HasValue && _distributer.Next.Value - curPos < 5)
            await _distributer.Distribute();
    }

    public void Dispose()
    {
        _reader.Dispose();
        _mpvApi.Dispose();
        GC.SuppressFinalize(this);
        Log.Info("Rhythm canvas disposed");
    }
}
