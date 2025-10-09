using System;

namespace RhythmTracker.Rhythm;

public record LaneFlag(double TrackPosition, int LaneNumber)
{
    public override string ToString()
    {
        return $"{TrackPosition:.3f}|{LaneNumber}";
    }
}
