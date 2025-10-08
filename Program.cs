using System.Diagnostics;
using System.Globalization;
using RhythmTracker;

string audioFile = "orchid.mp3";
CancellationTokenSource tokenSource = new();

using MpvApi mpv = new(audioFile);
await mpv.StartPlayingAudio(tokenSource.Token);
Console.CancelKeyPress += (sender, e) => tokenSource.Cancel();
Console.CursorVisible = false;
TimeSpan prev = TimeSpan.Zero;
var duration = await mpv.GetDuration();
System.Console.WriteLine($"Now playing: {await mpv.GetFileName()}");
System.Console.Write(prev + "/" + duration + "\r");
while (mpv.IsPlaying)
{
    var res = await mpv.GetAudioTimeSpan();
    if (res != prev)
    {
        prev = res;
        System.Console.Write(res + "/" + duration + "\r");
    }
    // System.Console.WriteLine(res);
    await Task.Delay(250);
}

if (mpv.PlayTask != null)
    await mpv.PlayTask;
