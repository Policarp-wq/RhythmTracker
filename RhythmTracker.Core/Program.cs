using Gtk;
using RhythmTracker.AudioManagement.Logging;
using RhythmTracker.Core.Interpreters;

Log.IsLogging = true;

ConsoleInterpreter interpreter = new();
using WindowInterpreter _window = new();
using CancellationTokenSource tokenSource = new();
Console.CancelKeyPress += (sender, args) =>
{
    args.Cancel = true;
    tokenSource.Cancel();
    ;
};
await _window.Play(tokenSource.Token).ConfigureAwait(true);
