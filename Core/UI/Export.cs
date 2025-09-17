namespace RingEngine.Core.UI;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using RingEngine.Core.General;
using RingEngine.Core.Script;
using static RingEngine.Core.General.AssertWrapper;
using Logger = General.Logger;

public class UIModule
{
    public VNRuntime runtime;
    public UITheme Theme;
    public DefaultTheme DefaultTheme
    {
        get
        {
            if (Theme is DefaultTheme defaultTheme)
            {
                return defaultTheme;
            }
            Unreachable("Theme is not DefaultTheme.");
            return null;
        }
    }

    public void DisplayBranch(IEnumerable<Branch.BranchOption> options)
    {
        var branchRoot = SceneTreeProxy.BranchScene.Instantiate<Control>();
        branchRoot.Name = "Branch";
        branchRoot.Set("Callback", Callable.From<int, string, string>(BranchCallBack));
        branchRoot.Set("Texts", new Array<string>(options.Select((option) => option.Text)));
        branchRoot.Set("Labels", new Array<string>(options.Select((option) => option.Label)));
        SceneTreeProxy.UIRoot.AddChild(branchRoot);
    }

    public void BranchCallBack(int index, string text, string label)
    {
        runtime.Paused = false;
        Logger.Log($"BranchCallBack {index} {text} {label}");
        // 这里index不会自动加1，但是Label是空操作所以下次调用Step会自动过，没有影响
        new JumpToLabel(label).Execute(runtime);
        // 选择后应当是continue的
        runtime.Step();
    }
}

/// <summary>
/// 规定了主场景的UI样式，包括对话框、角色名、章节名等。
/// 功能按键是否要包括进来？感觉得要。
/// </summary>
public abstract class UITheme
{
    /// <summary>
    /// 从这个路径加载UI场景。
    /// </summary>
    public abstract PathSTD ScenePath { get; }
    public Control Root => SceneTreeProxy.ThemeRoot;

    /// <summary>
    /// 所有UI样式都支持角色说话。
    /// </summary>
    /// <param name="name">显示角色名</param>
    /// <param name="content">说话内容</param>
    public abstract void CharacterSay(string name, string content);

    /// <summary>
    /// 调节文本框的显示比例，用于UI动画。
    /// </summary>
    /// <param name="visibleRatio">0-1之间</param>
    public abstract void TextBoxVisibleRatio(float visibleRatio);
}

public class DefaultTheme : UITheme
{
    public override PathSTD ScenePath => "res://scenes/DefaultTheme.tscn";
    public RichTextLabel TextBox =>
        Root.GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox");
    public RichTextLabel CharacterNameBox =>
        Root.GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer2/TextBox");
    public RichTextLabel ChapterNameBox =>
        Root.GetNode<RichTextLabel>("./ChapterNameBack/ChapterName");
    public TextureRect ChapterNameBack => Root.GetNode<TextureRect>("./ChapterNameBack");
    public string CharacterName
    {
        get => CharacterNameBox.Text.Trim(['【', '】']);
        set
        {
            if (string.IsNullOrEmpty(value.TrimStart()))
            {
                CharacterNameBox.Text = "";
            }
            else
            {
                CharacterNameBox.Text = "【" + value + "】";
            }
        }
    }

    public override void CharacterSay(string name, string content)
    {
        CharacterName = name;
        TextBox.Text = content;
    }

    public override void TextBoxVisibleRatio(float visibleRatio)
    {
        TextBox.VisibleRatio = visibleRatio;
    }

    public string ChapterName
    {
        get => ChapterNameBox.Text;
        set => ChapterNameBox.Text = value;
    }
    public float ChapterNameAlpha
    {
        get => ChapterNameBack.Alpha();
        set => ChapterNameBack.Alpha().Set(value);
    }
}
