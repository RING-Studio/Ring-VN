namespace RingEngine.EAL;

using Godot;

/// <summary>
/// 显示窗口，目前固定为全局窗口
/// </summary>
public class Window
{
    public static int Width =>
        ProjectSettings.GetSetting("display/window/size/viewport_width").AsInt32();
    public static int Height =>
        ProjectSettings.GetSetting("display/window/size/viewport_height").AsInt32();
}
