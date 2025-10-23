using System;

namespace RhythmTracker.AudioManagement;

public interface IAudioManager
{
    public Task<double> GetDuration();
    public Task<double> GetCurrentPosition();
    public Task<string> GetFileName();
    public bool IsPlaying();
}
