using System;

namespace RhythmTracker;

public interface IAudioManager
{
    public Task<double> GetDuration();
    public Task<double> GetCurrentPosition();
    public Task<string> GetFileName();
    public bool IsPlaying();
}
