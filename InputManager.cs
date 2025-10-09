using System;

namespace RhythmTracker;

public class InputManager
{
    private readonly Dictionary<ConsoleKey, Action> _onKeyPressed = [];

    public InputManager() { }

    public void Subscribe(ConsoleKey key, Action onKeyPressed)
    {
        _onKeyPressed.Add(key, onKeyPressed);
    }

    public void ListenConsoleInput(Func<bool> stopSignal)
    {
        while (!stopSignal.Invoke())
        {
            ConsoleKey key = Console.ReadKey(true).Key;
            if (stopSignal.Invoke())
                break;
            if (_onKeyPressed.TryGetValue(key, out var act))
            {
                act();
            }
        }
    }
}
