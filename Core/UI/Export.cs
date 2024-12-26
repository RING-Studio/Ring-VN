namespace RingEngine.Core.UI;

using System.Collections.Generic;
using EAL.Resource;
using Godot;
using RingEngine.Core.General;
using RingEngine.Core.Script;
using RingEngine.EAL.SceneTree;

public class UIModule
{
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

    public static void DisplayBranch(IEnumerable<Branch.BranchOption> options) =>
        UI.DisplayBranch(options);

    public static void BranchCallBack(int index, string text)
    {
        Logger.Log($"BranchCallBack {index} {text}");
    }
}
