using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace RhythmTracker.AudioManager;

public class MpvHub : IDisposable
{
    private bool _disposed = false;
    private readonly Socket _socket;
    private readonly NetworkStream _network;
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private ConcurrentDictionary<int, string> _responses = [];
    private readonly int _maxRequestTries;

    public MpvHub(Socket socket, int maxRequestTries = 10)
    {
        _socket = socket;
        _network = new(_socket);
        _reader = new(_network, Encoding.UTF8, leaveOpen: true);
        _writer = new(_network, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };
        _maxRequestTries = maxRequestTries;
    }

    public async Task<string?> GetResponse(int requestId)
    {
        if (_responses.TryRemove(requestId, out var val))
        {
            return val;
        }
        string? response = await _reader.ReadLineAsync();
        int tries = 0;
        while (tries < _maxRequestTries)
        {
            if (response != null && response.Contains("request_id"))
            {
                int idIndex = response.IndexOf("request_id") + 12;
                StringBuilder stringId = new();
                while (idIndex < response.Length && char.IsDigit(response[idIndex]))
                {
                    stringId.Append(response[idIndex]);
                    ++idIndex;
                }
                int id = int.Parse(stringId.ToString());
                if (id == requestId)
                    return response;
                _responses.TryAdd(id, response);
            }
            response = await _reader.ReadLineAsync();
            await Task.Delay(10);
            ++tries;
        }
        return null;
    }

    private static string GetSerializedMpvCommand(Dictionary<string, object> command)
    {
        return JsonSerializer.Serialize(command) + "\n";
    }

    public async Task SendCommand(Dictionary<string, object> command, int requestId)
    {
        command.Add("request_id", requestId);
        var serializedMessage = GetSerializedMpvCommand(command);
        await _writer.WriteAsync(serializedMessage);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;
        if (disposing)
        {
            _writer.Dispose();
            _reader.Dispose();
            _network.Dispose();
            _socket.Dispose();
            _disposed = true;
            System.Console.WriteLine("Hub disposed");
        }
    }
}
