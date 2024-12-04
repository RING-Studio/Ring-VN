namespace Test.Core.Script;

using System.Collections.Generic;
using System.Linq;
using RingEngine.Core.Script;
using Sprache;

[TestClass]
public class TestParser
{
    const string script =
        @"# 章节标题

角色名:""台词""

```python
#show_character()
```

changeBG <img src=""bg1.png"" style=""zoom: 10%;"" /> with dissolve

show <img src=""assets/chara.png"" style=""zoom:25%;"" /> as 红叶 at farleft with dissolve

";

    [TestMethod]
    public void Parse()
    {
        var ret = Parser._Parse(script);
        Assert.AreEqual(5, ret.Count);
        Assert.IsInstanceOfType(ret[0], typeof(ShowChapterName));
        Assert.AreEqual("章节标题", ((ShowChapterName)ret[0]).ChapterName);
        Assert.IsInstanceOfType(ret[1], typeof(Say));
        Assert.AreEqual("角色名", ((Say)ret[1]).Name);
        Assert.AreEqual("台词", ((Say)ret[1]).Content);
        Assert.IsInstanceOfType(ret[2], typeof(CodeBlock));
        Assert.AreEqual("python", ((CodeBlock)ret[2]).Identifier);
        Assert.AreEqual("#show_character()", ((CodeBlock)ret[2]).Code);
        Assert.IsInstanceOfType(ret[3], typeof(ChangeBG));
        Assert.AreEqual("bg1.png", ((ChangeBG)ret[3]).ImgPath);
        Assert.AreEqual("dissolve", ((ChangeBG)ret[3]).Effect);
        Assert.IsInstanceOfType(ret[4], typeof(Show));
        Assert.AreEqual("assets/chara.png", ((Show)ret[4]).ImgPath);
        Assert.AreEqual("farleft", ((Show)ret[4]).Placement);
        Assert.AreEqual("dissolve", ((Show)ret[4]).Effect);
        Assert.AreEqual("红叶", ((Show)ret[4]).ImgName);
    }

    [TestMethod]
    public void Parse2()
    {
        var script = File.ReadAllText("../../../../main.md");
        var ret = Parser._Parse(script);
    }

    [TestMethod]
    public void ParseLabel()
    {
        var ret = Parser.LabelParser.End().Parse(@"**选择支1**");
        Assert.AreEqual("选择支1", ret.Name);
    }

    [TestMethod]
    public void ParseShowChapterName()
    {
        var ret = Parser.ShowChapterNameParser.End().Parse("# Chapter 1\n");
        Assert.AreEqual("Chapter 1", ret.ChapterName);
    }

    [TestMethod]
    public void ParseShowChapterNameNotEnd()
    {
        var ret = Parser.ShowChapterNameParser.TryParse("# Chapter 1\nother content");
        Assert.IsTrue(ret.WasSuccessful);
        Assert.AreEqual("other content", ret.Remainder.Source[ret.Remainder.Position..]);
        Assert.AreEqual("Chapter 1", ret.Value.ChapterName);
    }

    [TestMethod]
    [DataRow("红叶 : \"台词 abab;:.\n\"", "红叶", "台词 abab;:.\n")]
    [DataRow("红叶 ： \"台词 abab;:.\n\"", "红叶", "台词 abab;:.\n")]
    [DataRow("： \"台词 abab;:.\n\"", "", "台词 abab;:.\n")]
    public void ParseSay(string input, string name, string content)
    {
        var ret = Parser.SayParser.End().Parse(input);
        Assert.AreEqual(name, ret.Name);
        Assert.AreEqual(content, ret.Content);
    }

    [TestMethod]
    public void ParseSayNotEnd()
    {
        var ret = Parser.SayParser.TryParse("红叶 : \"台词\"\nother content");
        Assert.IsTrue(ret.WasSuccessful);
        Assert.AreEqual("\nother content", ret.Remainder.Source[ret.Remainder.Position..]);
        Assert.AreEqual("红叶", ret.Value.Name);
        Assert.AreEqual("台词", ret.Value.Content);
    }

    [TestMethod]
    public void ParseCodeBlock()
    {
        var ret = Parser
            .CodeBlockParser.End()
            .Parse(
                @"``` Python
# 控制逻辑
# 代码块可以操作运行时提供的舞台对象(StageObject)
# e.g.
#show_character()
```"
            );
        Assert.AreEqual("Python", ret.Identifier);
        Assert.AreEqual(
            @"# 控制逻辑
# 代码块可以操作运行时提供的舞台对象(StageObject)
# e.g.
#show_character()",
            ret.Code
        );
    }

    [TestMethod]
    public void ParseBranch()
    {
        var ret = Parser
            .BranchParser.End()
            .Parse(
                @"| 竖排  |        |
| ----- | ------ |
| 选项1 | label1 |
| 选项2 | label2 |
| 选项3 | label3 |"
            );

        Assert.AreEqual(Branch.BranchType.Vertical, ret.Type);
        var references = new List<Branch.BranchOption>
        {
            new("选项1", "label1"),
            new("选项2", "label2"),
            new("选项3", "label3")
        };
        foreach (var (reference, actual) in references.Zip(ret.Options))
        {
            Assert.AreEqual(reference, actual);
        }
    }
}

[TestClass]
public class TestBuiltInParser
{
    [TestMethod]
    [DataRow(@"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left")]
    [DataRow(@"show<img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />as 红叶 at left")]
    [DataRow(
        @"show   <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />   as 红叶 at left"
    )]
    public void ParseShow(string input)
    {
        var ret = BuiltInFunctionParser.ShowParser.End().Parse(input);
        Assert.AreEqual("红叶", ret.ImgName);
        Assert.AreEqual("assets/bg2.jpg", ret.ImgPath);
        Assert.AreEqual("left", ret.Placement);
        Assert.IsNull(ret.Effect);
    }

    [TestMethod]
    public void ParseShowWithEffect()
    {
        var ret = BuiltInFunctionParser
            .ShowParser.End()
            .Parse(
                @"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with dissolve"
            );
        Assert.AreEqual("红叶", ret.ImgName);
        Assert.AreEqual("assets/bg2.jpg", ret.ImgPath);
        Assert.AreEqual("left", ret.Placement);
        Assert.AreEqual("dissolve", ret.Effect);
    }

    [TestMethod]
    public void ParseShowWithInlineCode()
    {
        var ret = BuiltInFunctionParser
            .ShowParser.End()
            .Parse(
                @"show <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> as 红叶 at left with `Dissolve(2.0, 0.5)`"
            );
        Assert.AreEqual("红叶", ret.ImgName);
        Assert.AreEqual("assets/bg2.jpg", ret.ImgPath);
        Assert.AreEqual("left", ret.Placement);
        Assert.AreEqual("Dissolve(2.0, 0.5)", ret.Effect);
    }

    [TestMethod]
    public void ParseChangeBG()
    {
        var ret = BuiltInFunctionParser
            .ChangeBGParser.End()
            .Parse(@"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />");
        Assert.AreEqual("assets/bg2.jpg", ret.ImgPath);
        Assert.IsNull(ret.Effect);
    }

    [TestMethod]
    [DataRow(
        @"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with dissolve"
    )]
    [DataRow(
        @"changeBG<img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />with dissolve"
    )]
    [DataRow(
        @"changeBG   <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" />   with dissolve"
    )]
    public void ParseChangeBGWithEffect(string input)
    {
        var ret = BuiltInFunctionParser.ChangeBGParser.End().Parse(input);
        Assert.AreEqual("assets/bg2.jpg", ret.ImgPath);
        Assert.AreEqual("dissolve", ret.Effect);
    }

    [TestMethod]
    public void ParseChangeBGWithInlineCode()
    {
        var ret = BuiltInFunctionParser
            .ChangeBGParser.End()
            .Parse(
                @"changeBG <img src=""assets/bg2.jpg"" alt=""bg2"" style=""zoom:25%;"" /> with `Dissolve(2.0, 0.5)`"
            );
        Assert.AreEqual("assets/bg2.jpg", ret.ImgPath);
        Assert.AreEqual("Dissolve(2.0, 0.5)", ret.Effect);
    }

    [TestMethod]
    public void ParseHide()
    {
        var ret = BuiltInFunctionParser.HideParser.End().Parse(@"hide 红叶");
        Assert.AreEqual("红叶", ret.Name);
        Assert.IsNull(ret.Effect);
    }

    [TestMethod]
    public void ParseHideWithEffect()
    {
        var ret = BuiltInFunctionParser.HideParser.End().Parse(@"hide 红叶 with dissolve");
        Assert.AreEqual("红叶", ret.Name);
        Assert.AreEqual("dissolve", ret.Effect);
    }

    [TestMethod]
    [DataRow(@"goto label1", "label1")]
    [DataRow(@"goto `label expression`", "label expression")]
    public void ParseJumpToLabel(string input, string labelName)
    {
        var ret = BuiltInFunctionParser.JumpToLabelParser.End().Parse(input);
        Assert.AreEqual(labelName, ret.LabelName);
    }

    [TestMethod]
    [DataRow(@"UIAnim dissolve", "dissolve")]
    [DataRow(@"UIAnim `Dissolve(2.0, 0.5)`", "Dissolve(2.0, 0.5)")]
    public void ParseUIAnim(string input, string effect)
    {
        var ret = BuiltInFunctionParser.UIAnimParser.End().Parse(input);
        Assert.AreEqual(effect, ret.Effect);
    }
}
