using System;
using System.Globalization;

namespace RhythmTracker.Rhythm;

public record Rhythm(double TrackPosition, int LaneNumber)
{
    public override string ToString()
    {
        return $"{TrackPosition.ToString("F3", CultureInfo.InvariantCulture)}|{LaneNumber}";
    }
}
