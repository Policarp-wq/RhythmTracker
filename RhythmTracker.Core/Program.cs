using Gtk;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.Core.Interpreters;

Log.IsLogging = true;

ConsoleInterpreter interpreter = new();
using WindowInterpreter _window = new();

await _window.Play().ConfigureAwait(true);
