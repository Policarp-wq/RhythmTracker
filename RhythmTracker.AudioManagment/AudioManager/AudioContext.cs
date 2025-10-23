using System;
using RhythmTracker.AudioManagement.Logging;

namespace RhythmTracker.AudioManagement.AudioManager;

public class AudioContext
{
    private readonly IAudioManager _audioManager;
    public event Func<double, Task>? OnPositionGot;

    public AudioContext(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public async Task TrackAudioTime(int pingInterval, Action? onEnd = null)
    {
        try
        {
            Log.Info("Started tracking audio track time");
            while (_audioManager.IsPlaying())
            {
                var curPos = await _audioManager.GetCurrentPosition();
                if (OnPositionGot != null)
                    await OnPositionGot(curPos);
                await Task.Delay(pingInterval);
            }
        }
        catch (InvalidOperationException) { }
        onEnd?.Invoke();
        Log.Info("Ended tracking time");
    }
}
