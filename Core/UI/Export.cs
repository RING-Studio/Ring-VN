namespace RingEngine.Core.UI;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using RingEngine.Core.General;
using RingEngine.Core.Script;
using static RingEngine.Core.General.AssertWrapper;

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

public abstract class UITheme
{
    public abstract PathSTD ScenePath { get; }
    public Control Root => SceneTreeProxy.ThemeRoot;
    public abstract void CharacterSay(string name, string content);
}

public class DefaultTheme : UITheme
{
    public override PathSTD ScenePath => "res://scenes/DefaultTheme.tscn";
    public RichTextLabel TextBox => Root.GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox");
    public RichTextLabel CharacterNameBox => Root.GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer2/TextBox");
    public RichTextLabel ChapterNameBox => Root.GetNode<RichTextLabel>("./ChapterNameBack/ChapterName");
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

    public void TextBoxVisibleRatio(float visibleRatio)
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
