namespace RingEngine.Core.Script;

using RingEngine.Core.General;
using RingEngine.Core.Storage;

/// <summary>
/// 脚本模块，脚本对象存储、解析、执行
/// </summary>
public class ScriptModule
{
    // 脚本内嵌代码解释器
    public PythonInterpreter interpreter;

    // 脚本源代码
    public RingScript script;

    public int Length => script.segments.Count;

    // 将相对于脚本的路径转换为相对于根目录的路径
    public PathSTD ToPathSTD(string filePath) => script.ToPathSTD(filePath);

    public ScriptModule(VNRuntime runtime, PathSTD entryPointPath, PathSTD initCodePath)
    {
        script = new RingScript(entryPointPath);
        interpreter = new PythonInterpreter(
            runtime,
            GlobalConfig.ProjectRoot,
            UniformLoader.LoadText(initCodePath)
        );
    }

    /// <summary>
    /// 运行脚本至下一个可保存点
    /// </summary>
    /// <param name="index">下一条需要执行的语句</param>
    /// <param name="runtime"></param>
    /// <returns>下次执行时第一条需要被执行的语句</returns>
    public int Step(int index, VNRuntime runtime)
    {
        var @continue = false;
        do
        {
            @continue = script.segments[index].Continue;
            script.segments[index].Execute(runtime);
            index++;
        } while (@continue && index < Length);
        return index;
    }

    public string Serialize()
    {
        return script.ScriptPath.RelativePath;
    }

    public void Deserialize(string path)
    {
        script = new RingScript(path);
    }
}
