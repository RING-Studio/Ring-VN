namespace RingEngine.Core.General;

using System;
using System.Runtime.CompilerServices;

public class AssertException : Exception
{
    public AssertException(string message)
        : base(message) { }
}

public static class AssertWrapper
{
    /// <summary>
    /// 输出到Logger的Assert。
    /// </summary>
    /// <param name="condition">断言条件。</param>
    /// <param name="message">断言失败时的消息。</param>
    /// <param name="filePath">调用代码的文件路径。</param>
    /// <param name="lineNumber">调用代码的行号。</param>
    /// <param name="memberName">调用代码的成员名。</param>
    public static void Assert(
        bool condition,
        string message = "Assertion failed.",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = ""
    )
    {
        if (!condition)
        {
            // 创建详细的错误消息，包含文件、行号、表达式和用户消息
            var errorMessage =
                $"{message}\n" + $"Location: {filePath} (Line {lineNumber}, Member: {memberName})";
            Logger.Error(errorMessage);
            throw new AssertException(errorMessage);
        }
    }

    /// <summary>
    /// 不可能执行到的代码。
    /// </summary>
    public static void Unreachable(
        string message = "Unreachable code reached.",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = ""
    )
    {
        Assert(false, message, filePath, lineNumber, memberName);
    }
}
