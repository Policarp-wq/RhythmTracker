using System.Diagnostics;
using System.Globalization;
using RhythmTracker;
using RhythmTracker.AudioManager;

static TimeSpan GetTimeFromRawSeconds(double seconds) => TimeSpan.FromSeconds((int)seconds);

string audioFile = "orchid.mp3";
InputManager manager = new();
manager.Subscribe(ConsoleKey.Spacebar, () => System.Console.WriteLine("Kek"));
CancellationTokenSource tokenSource = new();
using MpvApi mpv = new(audioFile);
await mpv.StartPlayingAudio(tokenSource.Token);
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    tokenSource.Cancel();
};

var timerTask = Task.Run(async () =>
{
    try
    {
        Console.CursorVisible = false;
        TimeSpan prev = TimeSpan.Zero;
        TimeSpan duration = GetTimeFromRawSeconds(await mpv.GetDuration());
        System.Console.WriteLine($"Now playing: {await mpv.GetFileName()}");
        System.Console.Write(prev + "/" + duration + "\r");
        while (mpv.IsPlaying)
        {
            var res = GetTimeFromRawSeconds(await mpv.GetCurrentPosition());
            if (res != prev)
            {
                prev = res;
                Console.Write($"\r{res}/{duration}   ");
            }
            // System.Console.WriteLine(res);
            await Task.Delay(250);
        }
    }
    catch (InvalidOperationException) { }

    if (mpv.PlayTask != null)
        await mpv.PlayTask;
    Console.CursorVisible = true;
    Console.WriteLine("\nPlayback finished. Press any key to exit.");
});

manager.ListenConsoleInput(() => timerTask.IsCompleted);
