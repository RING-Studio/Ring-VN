namespace RingEngine.Core.UI;

using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using RingEngine.Core.General;
using RingEngine.Core.Script;

public class UIModule
{
    public VNRuntime runtime;
    public Control Root => SceneTreeProxy.UIRoot;
    public RichTextLabel TextBox => SceneTreeProxy.TextBox;
    public RichTextLabel CharacterNameBox => SceneTreeProxy.CharacterNameBox;
    public RichTextLabel ChapterNameBox => SceneTreeProxy.ChapterNameBox;
    public TextureRect ChapterNameBack => SceneTreeProxy.ChapterNameBack;
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

    public void CharacterSay(string name, string content)
    {
        CharacterName = name;
        TextBox.Text = content;
    }

    public void TextBoxVisibleRatio(float visibleRatio)
    {
        TextBox.VisibleRatio = visibleRatio;
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
