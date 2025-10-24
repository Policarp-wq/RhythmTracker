using System;

namespace RhythmTracker.WindowDrawing.Models;

public interface IRhythmAdjustable
{
    public void Adjust(double currentPosition, int FPS);
}
