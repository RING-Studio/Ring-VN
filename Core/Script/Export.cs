namespace RingEngine.Core.Script;

using System;
using System.Threading.Tasks;
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
    public PathSTD StandardizePath(string filePath) => script.StandardizePath(filePath);

    public PathSTD StandardizePath(PathSTD filePath) => script.StandardizePath(filePath);

    public ScriptModule(VNRuntime runtime, PathSTD entryPointPath, PathSTD initCodePath)
    {
        script = new RingScript(entryPointPath);
        interpreter = new PythonInterpreter(
            runtime,
            RingConfig.ProjectRoot,
            UniformLoader.LoadText(initCodePath)
        );
    }

    /// <summary>
    /// 运行脚本至下一个可保存点
    /// </summary>
    /// <param name="index">下一条需要执行的语句</param>
    /// <param name="runtime"></param>
    public async Task Step(VNRuntime runtime)
    {
        var Global = runtime.Storage.Global;
        var @continue = false;
        try
        {
            do
            {
                @continue = script.segments[Global.PC].Continue;
                await script.segments[Global.PC].Execute(runtime);
                Global.PC++;
            } while ((@continue || Global.ForceContinue) && Global.PC < Length);
        }
        catch (Exception)
        {
            Logger.Log($"Error at block {Global.PC} {script.segments[Global.PC]}");
            throw;
        }
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
