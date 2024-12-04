namespace RingEngine.Core.General;

using RingEngine.EAL.General;

public interface IOutput
{
    void Print(string message);
}

/// <summary>
/// 使用GD.Print记录日志
/// </summary>
public static class Logger
{
    public static IOutput Output = new GDOutput();

    public static void Log(string message)
    {
        Output.Print($"[Log] {message}");
    }

    public static void Warning(string message)
    {
        Output.Print($"[Warning] {message}");
    }

    public static void Error(string message)
    {
        Output.Print($"[Error] {message}");
    }
}
