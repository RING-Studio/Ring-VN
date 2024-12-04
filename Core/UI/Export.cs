namespace RingEngine.Core.UI.Export;

using System.Collections.Generic;
using EAL.Resource;
using RingEngine.Core.General;
using RingEngine.Core.Script;
using RingEngine.EAL.SceneTree;

public class UIModule
{
    public int TextSpeed { get; set; }

    public Widget Root => new(SceneTreeProxy.UIRoot);
    public TextBox TextBox => SceneTreeProxy.TextBox;
    public TextBox CharacterNameBox => SceneTreeProxy.CharacterNameBox;
    public TextBox ChapterNameBox => SceneTreeProxy.ChapterNameBox;
    public Widget ChapterNameBack => SceneTreeProxy.ChapterNameBack;
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
        get => ChapterNameBack.Alpha;
        set => ChapterNameBack.Alpha = value;
    }

    public double DurationForText(string text) => text.Length / TextSpeed;

    public void CharacterSay(string name, string content)
    {
        CharacterName = name;
        TextBox.Text = content;
    }

    public void TextBoxVisibleRatio(float visibleRatio)
    {
        TextBox.VisibleRatio = visibleRatio;
    }

    public static void DisplayBranch(IEnumerable<Branch.BranchOption> options) =>
        UI.DisplayBranch(options);

    public static void BranchCallBack(int index, string text)
    {
        Logger.Log($"BranchCallBack {index} {text}");
    }
}
