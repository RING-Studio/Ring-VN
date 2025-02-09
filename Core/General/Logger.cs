namespace RingEngine.Core.General;

using Godot;

/// <summary>
/// 使用GD.Print记录日志
/// </summary>
public static class Logger
{
    public static void Log(string message)
    {
        GD.Print($"[Log] {message}");
    }

    public static void Warn(string message)
    {
        GD.Print($"[Warning] {message}");
    }

    public static void Error(string message)
    {
        GD.Print($"[Error] {message}");
    }
}
