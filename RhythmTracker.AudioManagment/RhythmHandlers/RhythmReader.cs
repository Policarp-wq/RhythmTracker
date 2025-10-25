using System;
using System.Globalization;
using System.Text;

namespace RhythmTracker.AudioManagement.RhythmHandlers;

public class RhythmReader : IDisposable
{
    public readonly string FlagFile;
    private readonly StreamReader _reader;
    public bool IsFullyRead => _reader.EndOfStream;
    private IAudioManager _audioManager;
    private Queue<Rhythm> _rhythms = [];
    private readonly int _bufferCnt;
    public readonly double EpsilonS;

    public RhythmReader(IAudioManager audioManager, string flagFile, int epsilonMs, int bufferCnt)
    {
        _audioManager = audioManager;
        FlagFile = flagFile;
        _reader = new StreamReader(flagFile);
        EpsilonS = 1.0 * epsilonMs / 1000;
        _bufferCnt = bufferCnt;
    }

    private void Pulse(Rhythm rhythm)
    {
        System.Console.WriteLine($"Pulsing!: {rhythm}");
    }

    public async Task<double?> GetNextPulse()
    {
        if (_rhythms.Count == 0)
            return null;
        var pos = _rhythms.Dequeue();
        await FillBuffer();
        return pos.TrackPosition;
    }

    public async Task RefreshActiveRhythms(double currentDuration)
    {
        while (_rhythms.Count > 0)
        {
            var curRhythm = _rhythms.Peek();
            // TODO: Weird shi
            if (currentDuration + EpsilonS > curRhythm.TrackPosition)
            {
                Pulse(_rhythms.Dequeue());
                continue;
            }
            else if (curRhythm.TrackPosition - EpsilonS < currentDuration)
            {
                _rhythms.Dequeue();
            }
            break;
        }
        await FillBuffer();
    }

    public async Task FillBuffer()
    {
        if (!IsFullyRead)
        {
            for (int i = 0; i < _bufferCnt - _rhythms.Count; ++i)
            {
                var rhythm = await ReadRhythm();
                if (rhythm == null)
                    break;
                _rhythms.Enqueue(rhythm);
            }
        }
    }

    public async Task<Rhythm?> ReadRhythm()
    {
        StringBuilder rhythmBuilder = new();
        await Task.Run(() =>
        {
            while (!_reader.EndOfStream)
            {
                char ch = (char)_reader.Read();
                if (ch == ';')
                    break;
                rhythmBuilder.Append(ch);
            }
        });
        if (_reader.EndOfStream)
            return null;
        string s = rhythmBuilder.ToString();
        var splitted = s.Split('|');
        if (splitted.Length != 2)
            throw new FormatException($"Wrong rhythm: {s}");
        double position = double.Parse(splitted[0], CultureInfo.InvariantCulture);
        int lane = int.Parse(splitted[1]);
        return new(position, lane);
    }

    public void Dispose()
    {
        _reader.Dispose();
    }
}
