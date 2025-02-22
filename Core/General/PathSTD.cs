namespace RingEngine.Core.General;

using System;
using System.Collections.Generic;
using System.IO;
using Godot;

public class PathSTD : IEquatable<PathSTD>
{
    /// <summary>
    /// 相对路径，没有前缀，路径分隔符为"/"，文件夹没有"/"结尾
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
        // 脚本解析出来的路径可能有空格
        path = path.Trim();
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

    public static PathSTD operator +(PathSTD a, PathSTD b) =>
        From(a.RelativePath + "/" + b.RelativePath);

    public static implicit operator PathSTD(string path) => From(path);

    // Generated
    public override bool Equals(object obj) => this.Equals(obj as PathSTD);

    public bool Equals(PathSTD other) =>
        other is not null && this.RelativePath == other.RelativePath;

    public override int GetHashCode() => HashCode.Combine(this.RelativePath);

    public override string ToString() => RelativePath;

    public static bool operator ==(PathSTD left, PathSTD right) =>
        EqualityComparer<PathSTD>.Default.Equals(left, right);

    public static bool operator !=(PathSTD left, PathSTD right) => !(left == right);
}
