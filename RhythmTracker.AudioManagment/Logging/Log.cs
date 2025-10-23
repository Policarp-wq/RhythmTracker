using System;

namespace RhythmTracker.AudioManagement.Logging;

public class Log
{
    public static bool IsLogging = true;

    public static void Info(string message)
    {
        if (!IsLogging)
            return;
        System.Console.WriteLine($"[{DateTime.Now.TimeOfDay}]: {message}");
    }
}
