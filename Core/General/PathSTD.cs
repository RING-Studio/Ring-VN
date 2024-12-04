namespace RingEngine.Core.General;

using System.IO;
using Godot;

public class PathSTD
{
    /// <summary>
    /// 以根目录为基准的路径，没有前缀，路径分隔符为"/"，文件夹没有"/"结尾
    /// </summary>
    public string RelativePath { get; }

    public string Directory => Path.GetDirectoryName(RelativePath);

    /// <summary>
    /// 没有办法识别文件和文件夹，用的时候自己确认路径是什么
    /// </summary>
    public string FileName => Path.GetFileName(RelativePath);
    public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(RelativePath);
    public string Extension => Path.GetExtension(RelativePath);
    public string GodotPath => $"res://{RelativePath}";
    public string GodotDirectory => $"res://{Directory}";

    PathSTD(string path)
    {
        RelativePath = path.TrimSuffix("\\").TrimSuffix("/").Replace('\\', '/');
    }

    public static PathSTD From(string path)
    {
        if (path.StartsWith("res://"))
        {
            return new PathSTD(path.TrimPrefix("res://"));
        }
        else if (path.StartsWith("./"))
        {
            return new PathSTD(path.TrimPrefix("./"));
        }
        else
        {
            return new PathSTD(path);
        }
    }

    public static PathSTD operator +(PathSTD a, string b) =>
        From(a.RelativePath + "/" + From(b).RelativePath);
}
