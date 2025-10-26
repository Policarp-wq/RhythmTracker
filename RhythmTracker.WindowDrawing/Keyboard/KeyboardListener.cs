using System;
using Gtk;

namespace RhythmTracker.WindowDrawing.Keyboard;

public class KeyboardListener
{
    public readonly EventControllerKey ControllerKey;

    // https://docs.gtk.org/gtk4/signal.EventControllerKey.key-pressed.html
    private static Dictionary<uint, ConsoleKey> _translator = new Dictionary<uint, ConsoleKey>
    {
        { 65361, ConsoleKey.LeftArrow },
        { 65364, ConsoleKey.DownArrow },
        { 65363, ConsoleKey.RightArrow },
        { 65362, ConsoleKey.UpArrow },
        { 32, ConsoleKey.Spacebar },
    };
    public event Action<ConsoleKey>? OnKeyPressed;

    public KeyboardListener()
    {
        ControllerKey = EventControllerKey.New();
        ControllerKey.OnKeyPressed += (controller, args) =>
        {
            if (_translator.TryGetValue(args.Keyval, out var ck))
                OnKeyPressed?.Invoke(ck);
            return true;
        };
    }
}
