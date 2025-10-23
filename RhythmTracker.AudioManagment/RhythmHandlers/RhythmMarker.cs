using System;
using System.Threading.Tasks;

namespace RhythmTracker.AudioManagement.RhythmHandlers;

public class RhythmMarker
{
    private readonly IAudioManager _manager;
    private List<Rhythm> _flags = [];

    public RhythmMarker(IAudioManager manager)
    {
        _manager = manager;
    }

    public async Task Flag(int lane)
    {
        var position = await _manager.GetCurrentPosition();
        _flags.Add(new(position, lane));
    }

    public async Task SaveFlags()
    {
        using StreamWriter writer = new("flags");
        foreach (var flag in _flags)
        {
            await writer.WriteAsync(flag.ToString() + ";");
        }
        await writer.FlushAsync();
    }
}
