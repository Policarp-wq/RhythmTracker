using System;

namespace RhythmTracker.Core.Interpreters;

public interface IInterpreter
{
    public Task Play();
}
