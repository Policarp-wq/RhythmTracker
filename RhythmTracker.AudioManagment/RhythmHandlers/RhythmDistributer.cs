using System;

namespace RhythmTracker.AudioManagement.RhythmHandlers;

public class RhythmDistributer
{
    private readonly Dictionary<int, IConsumer> _consumers;
    private readonly RhythmReader _reader;
    private Queue<Rhythm> _buffer;
    public readonly int BufferSize;
    public double? Next => _buffer.Count == 0 ? null : _buffer.Peek().TrackPosition;

    public RhythmDistributer(RhythmReader reader, int bufferSize)
    {
        _reader = reader;
        _consumers = [];
        BufferSize = bufferSize;
        _buffer = [];
    }

    public void AddConsumer(int laneId, IConsumer consumer)
    {
        _consumers.Add(laneId, consumer);
    }

    public async Task FullBuffer()
    {
        while (_buffer.Count < BufferSize && !_reader.IsFullyRead)
        {
            var rhythm = await _reader.ReadRhythm();
            if (rhythm == null)
                return;
            if (!_consumers.ContainsKey(rhythm.LaneNumber))
                continue;
            _buffer.Enqueue(rhythm);
        }
    }

    public async Task Distribute()
    {
        if (_buffer.Count == 0)
            return;
        if (!_consumers[_buffer.Peek().LaneNumber].IsReadyToConsume)
            return;
        var rhythm = _buffer.Dequeue();
        _consumers[rhythm.LaneNumber].Consume(rhythm.TrackPosition);
        await FullBuffer();
    }
}
