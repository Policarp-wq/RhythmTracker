using RhythmTracker.AudioManagement;
using RhythmTracker.AudioManagement.AudioManager;
using RhythmTracker.AudioManagement.RhythmHandlers;

namespace RhythmTracker.Core.Interpreters;

public class ConsoleInterpreter : IInterpreter
{
    public async Task Play()
    {
        static TimeSpan GetTimeFromRawSeconds(double seconds) => TimeSpan.FromSeconds((int)seconds);
        string audioFile = "orchid.mp3";
        using MpvApi mpv = new(audioFile);
        bool isMarking = true;
        System.Console.WriteLine("Marking = 1, Testing = 0");
        if (int.Parse(Console.ReadLine()!) == 0)
        {
            isMarking = false;
        }

        RhythmMarker rhythmMarker = new(mpv);

        InputManager inputManager = new();
        inputManager.Subscribe(
            ConsoleKey.DownArrow,
            async () =>
            {
                await rhythmMarker.Flag(0);
            }
        );

        inputManager.Subscribe(
            ConsoleKey.LeftArrow,
            async () =>
            {
                await rhythmMarker.Flag(-1);
            }
        );
        inputManager.Subscribe(
            ConsoleKey.RightArrow,
            async () =>
            {
                await rhythmMarker.Flag(1);
            }
        );

        CancellationTokenSource tokenSource = new();
        Console.CursorVisible = false;
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            tokenSource.Cancel();
        };

        await mpv.StartPlayingAudio(tokenSource.Token, 250);
        System.Console.WriteLine($"Now playing: {await mpv.GetFileName()}");

        TimeSpan duration = GetTimeFromRawSeconds(await mpv.GetDuration());
        TimeSpan prev = TimeSpan.Zero;
        Console.Write($"\r{prev}/{duration}   ");
        Task PrintAudioTime(double currentPosition)
        {
            var res = GetTimeFromRawSeconds(currentPosition);
            if (res != prev)
            {
                prev = res;
                Console.Write($"\r{res}/{duration}   ");
            }

            return Task.CompletedTask;
        }

        AudioContext audio = new(mpv);
        if (isMarking)
        {
            audio.OnPositionGot += PrintAudioTime;
        }

        var track = audio.TrackAudioTime(
            50,
            () => Console.WriteLine("\nPlayback finished. Press any key to exit.")
        );

        if (isMarking)
        {
            inputManager.ListenConsoleInput(() => track.IsCompleted);
            await rhythmMarker.SaveFlags();
        }
        else
        {
            RhythmPulser pulser = new(mpv, "flags", 200, 10);
            audio.OnPositionGot += pulser.RefreshActiveRhythms;
            await track;
            audio.OnPositionGot -= pulser.RefreshActiveRhythms;
        }

        audio.OnPositionGot -= PrintAudioTime;
        Console.CursorVisible = true;
    }
}
