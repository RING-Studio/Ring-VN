namespace RingEngine.Core.Storage;

#nullable enable

using System.Collections.Generic;
using Godot;

public class RingConfig
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

    public Dictionary<string, BGConfig> BGConfigs = [];
    public Dictionary<string, CharacterConfig> CharacterConfigs = [];

    public Callable TextureLoadHook = Callable.From<string, Texture2D, Texture2D>(
        (_, texture) => texture
    );
}

public class BGConfig
{
    public Vector2I Size;

    /// <summary>
    /// 背景图的锚点，显示时会将该点对齐到画布左上角
    /// </summary>
    public Vector2I Anchor;

    public override string ToString() => $"BGConfig(Size: {Size}, Anchor: {Anchor})";
}

public class CharacterConfig
{
    public Vector2I Size;

    /// <summary>
    /// Y基线到图片顶端的距离相对于图片高度的比例
    /// </summary>
    public double YBase;
    public Vector2I AvatarSize;

    /// <summary>
    /// Avatar左上角偏移量
    /// </summary>
    public Vector2I AvatarOffset;
}
