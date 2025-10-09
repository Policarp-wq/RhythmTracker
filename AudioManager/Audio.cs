using System;

namespace RhythmTracker.AudioManager;

public class Audio
{
    private readonly IAudioManager _audioManager;
    public event Func<double, Task>? OnPositionGot;

    public Audio(IAudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    public async Task TrackAudioTime(int pingInterval, Action? onEnd = null)
    {
        try
        {
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
    }
}
