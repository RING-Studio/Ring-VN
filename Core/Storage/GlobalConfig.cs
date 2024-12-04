namespace RingEngine.Core.Storage;

#nullable enable

using System.Collections.Generic;
using Godot;

public class GlobalConfig
{
    public static string ProjectRoot =>
        OS.HasFeature("editor")
            ? ProjectSettings.GlobalizePath("res://")
            : OS.GetExecutablePath().GetBaseDir();

    public string DefaultRuntime = "AVG";

    /// <summary>
    /// 画布大小，等于Godot中设置的窗口分辨率
    /// </summary>
    public Vector2I CanvasSize = new(1920, 1080);

    /// <summary>
    /// 立绘基准分辨率，导入的素材应当符合该分辨率
    /// </summary>
    public Vector2 CharacterSize = new(950, 2184);

    /// <summary>
    /// 文本显示速度（字符数/秒）
    /// </summary>
    public double TextSpeed = 20;

    /// <summary>
    /// 角色名与立绘y基线对应关系
    /// </summary>
    public Dictionary<string, double> YBaseTable = [];
}
