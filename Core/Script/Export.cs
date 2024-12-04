namespace RingEngine.Core.Script.Export;

/// <summary>
/// 脚本模块，脚本对象存储、解析、执行
/// </summary>
public class ScriptModule
{
    // 脚本内嵌代码解释器
    public PythonInterpreter interpreter;

    // 脚本源代码
    public RingScript script;
}
