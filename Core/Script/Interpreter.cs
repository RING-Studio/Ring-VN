namespace RingEngine.Core.Script;

using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Python.Runtime;
using RingEngine.Core;
using RingEngine.Core.General;

public class PythonInterpreter
{
    PyModule scope;

    /// <summary>
    /// 创建一个Python解释器实例
    /// </summary>
    /// <param name="runtime">Ring Runtime</param>
    /// <param name="init">
    /// 初始化脚本
    /// 等效于：
    /// <code>
    /// PythonInterpreter interpreter = new PythonInterpreter();
    /// interpreter.Exec(init);
    /// </code>
    /// </param>
    public PythonInterpreter(VNRuntime runtime, string projectRoot, string init = "")
    {
        if (!PythonEngine.IsInitialized)
        {
            var PythonDir = new DirectoryInfo(Path.Combine(projectRoot, "python"));
            var Dll = PythonDir
                .GetFiles()
                .Where(file => file.Extension == ".dll")
                .Where(file => file.Name.StartsWith("python3"))
                .Where(file => file.Name != "python3.dll")
                .ToArray();
            if (Dll.Length < 1)
            {
                // 找不到Python DLL，尝试从官网下载
                Logger.Log("Cannot find Python DLL, trying to download from python.org...");
                var ZipPath = Path.Combine(projectRoot, "python", "python.zip");
                using (var web = new WebClient())
                {
                    web.DownloadFile(
                        "https://www.python.org/ftp/python/3.11.9/python-3.11.9-embed-amd64.zip",
                        ZipPath
                    );
                }
                ZipFile.ExtractToDirectory(ZipPath, PythonDir.FullName);
                Dll = PythonDir
                    .GetFiles()
                    .Where(file => file.Extension == ".dll")
                    .Where(file => file.Name.StartsWith("python3"))
                    .Where(file => file.Name != "python3.dll")
                    .ToArray();
                if (Dll.Length < 1)
                {
                    Logger.Log(
                        "Failed to download Python, please download it manually and put it in the python folder."
                    );
                    Logger.Log(
                        "https://www.python.org/ftp/python/3.11.9/python-3.11.9-embed-amd64.zip"
                    );
                    Logger.Error("Cannot find Python DLL");
                    return;
                }
                Logger.Log("Download Succeed.");
            }
            Runtime.PythonDLL = Dll[0].FullName;
            PythonEngine.Initialize();
        }
        using (Py.GIL())
        {
            scope = Py.CreateScope();
            scope.Set("runtime", runtime);
            scope.Exec(init);
        }
    }

    /// <summary>
    /// 获取Python全局变量
    /// </summary>
    /// <param name="name">变量名</param>
    /// <returns></returns>
    public dynamic this[string name] => scope.Get(name);

    /// <summary>
    /// 对传入的Python表达式求值并返回。由于PyObject自动类型转换的限制，返回值需要赋值给显式注明类型的变量。
    /// </summary>
    public dynamic Eval(string expr)
    {
        using (Py.GIL())
        {
            return scope.Eval(expr);
        }
    }

    /// <summary>
    /// 对传入的Python表达式求值并返回。由于提供了泛型参数，返回值可以直接使用。
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    public T Eval<T>(string expr) => Eval(expr);

    /// <summary>
    /// 执行一个Python代码段，不返回值。
    /// </summary>
    public void Exec(string code)
    {
        using (Py.GIL())
        {
            scope.Exec(code);
        }
    }
}
