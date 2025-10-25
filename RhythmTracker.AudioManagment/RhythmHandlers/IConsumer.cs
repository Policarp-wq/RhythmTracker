using System;

namespace RhythmTracker.AudioManagement.RhythmHandlers;

public interface IConsumer
{
    public void Consume(double trackPosition);
    public bool IsReadyToConsume { get; }
}
