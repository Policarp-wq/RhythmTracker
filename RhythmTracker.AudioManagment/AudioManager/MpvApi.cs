using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using RhythmTracker.AudioManagement.Logging;

namespace RhythmTracker.AudioManagement.AudioManager;

public class MpvApi : IDisposable, IAudioManager
{
    private const int POLL_INTERVAL = 250;
    private const string MPV_SERVER_SOCKET_PATH = "/tmp/mpvsocket";
    private readonly Process _playProcess;
    private MpvHub? _mpvHub;
    public Task? PlayTask { get; private set; }

    public bool IsPlaying()
    {
        try
        {
            return PlayTask != null && !_playProcess.HasExited;
        }
        catch (Exception) { }
        return false;
    }

    private int _cnt = 0;
    private bool _isDisposed = false;

    public MpvApi(string audioFile)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "mpv",
            Arguments =
                $"{audioFile} --terminal=no --input-terminal=no --input-cursor=no --input-ipc-server={MPV_SERVER_SOCKET_PATH}",
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        _playProcess = new Process() { StartInfo = psi };
    }

    private async Task<string?> ExecuteCommand(Dictionary<string, object> command)
    {
        if (!IsPlaying() || _mpvHub == null)
        {
            return null;
        }
        int requestId = _cnt++;
        await _mpvHub.SendCommand(command, requestId);
        var response = await _mpvHub.GetResponse(requestId);
        return response;
    }

    public async Task<string> GetProperty(string property)
    {
        Dictionary<string, object> command = new()
        {
            {
                "command",
                new List<string> { "get_property", property }
            },
        };
        var response = await ExecuteCommand(command);
        if (response == null)
            throw new Exception("Response to command was null");
        using JsonDocument json = JsonDocument.Parse(response);
        JsonElement root = json.RootElement;
        return root.GetProperty("data").ToString();
    }

    public async Task<double> GetCurrentPosition()
    {
        return double.Parse(await GetProperty("playback-time"), CultureInfo.InvariantCulture);
    }

    public async Task<string> GetFileName()
    {
        return await GetProperty("media-title");
    }

    public async Task StartPlayingAudio(CancellationToken token, int pingInterval)
    {
        _playProcess.Start();
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        bool isConnected = false;
        while (!isConnected)
        {
            try
            {
                await socket.ConnectAsync(new UnixDomainSocketEndPoint(MPV_SERVER_SOCKET_PATH));
                _mpvHub = new(socket);
            }
            catch (Exception)
            {
                continue;
            }
            isConnected = true;
        }
        Log.Info("Connected to mpv player");
        TaskCompletionSource<bool> _started = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        PlayTask = new Task(
            async () =>
            {
                _started.TrySetResult(true);
                try
                {
                    Log.Info("Started playing audio");
                    while (!_playProcess.HasExited)
                    {
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(pingInterval, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Log.Info("Cancelled player");
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    Dispose();
                }
            },
            token
        );
        PlayTask.Start();
        await _started.Task;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        try
        {
            if (!_playProcess.HasExited)
                _playProcess.Kill();
        }
        catch (InvalidOperationException) { }
        _playProcess.Dispose();

        _mpvHub?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<double> GetDuration()
    {
        return double.Parse(await GetProperty("duration"), CultureInfo.InvariantCulture);
    }
}
