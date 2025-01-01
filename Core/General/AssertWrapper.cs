namespace RingEngine.Core.General;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public class AssertException : Exception
{
    public AssertException(string message)
        : base(message) { }
}

public static class AssertWrapper
{
    /// <summary>
    /// 将异常打印到Logger。
    /// </summary>
    /// <param name="e">要打印的异常</param>
    /// <param name="filePath">调用代码的文件路径。</param>
    /// <param name="lineNumber">调用代码的行号。</param>
    /// <param name="memberName">调用代码的成员名。</param>
    /// <param name="expression">调用代码的表达式。</param>
    public static void LogException(
        Exception e,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        string expression = null
    )
    {
        // 创建详细的错误消息，包含文件、行号、成员变量和用户消息
        var errorMessage =
            $"{e.Message}\nLocation: {filePath} (Line {lineNumber}, Member: {memberName}";
        if (expression != null)
        {
            errorMessage += $", Expression: {expression}";
        }
        errorMessage += ")";
        Logger.Error(errorMessage);
    }

    /// <summary>
    /// 将异常打印到Logger再抛出。
    /// </summary>
    /// <param name="e">要抛出的异常</param>
    [DoesNotReturn]
    public static void Throw(
        Exception e,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        string expression = null
    )
    {
        LogException(e, filePath, lineNumber, memberName, expression);
        throw e;
    }

    /// <summary>
    /// Throw的泛型包裹，用于通过类型检查。
    /// </summary>
    /// <typeparam name="T">需要coerce to的类型</typeparam>
    public static T Throw<T>(
        Exception e,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        string expression = null
    )
    {
        Throw(e, filePath, lineNumber, memberName, expression);
        return default;
    }

    /// <summary>
    /// 输出到Logger的Assert。
    /// </summary>
    /// <param name="condition">断言条件。</param>
    /// <param name="message">断言失败时的消息。</param>
    public static void Assert(
        bool condition,
        string message = "Assertion failed.",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        [CallerArgumentExpression(nameof(condition))] string expression = null
    )
    {
        if (!condition)
        {
            Throw(new AssertException(message), filePath, lineNumber, memberName, expression);
        }
    }

    /// <summary>
    /// 不可能执行到的代码。
    /// </summary>
    [DoesNotReturn]
    public static void Unreachable(
        string message = "Unreachable code reached.",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = ""
    )
    {
        Throw(new AssertException(message), filePath, lineNumber, memberName);
    }

    /// <summary>
    /// 未实现的代码。
    /// </summary>
    [DoesNotReturn]
    public static void NotImplemented(
        string message = "Not implemented.",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = ""
    )
    {
        Throw(new NotImplementedException(message), filePath, lineNumber, memberName);
    }
}
