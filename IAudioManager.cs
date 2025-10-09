using System;

namespace RhythmTracker;

public interface IAudioManager
{
    public Task<double> GetDuration();
    public Task<double> GetCurrentPosition();
}
