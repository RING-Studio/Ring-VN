namespace RingEngine.Core.Script;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Python.Runtime;
using RingEngine.Core.Animation;
using RingEngine.Core.General;
using RingEngine.Core.Stage;
using static RingEngine.Core.General.AssertWrapper;

#nullable enable

public class RingScript
{
    public PathSTD ScriptPath;
    public List<IScriptBlock> segments = [];

    public RingScript(PathSTD filePath)
    {
        ScriptPath = filePath;
        try
        {
            segments = Parser._Parse(UniformLoader.LoadText(filePath));
        }
        catch (Exception ex)
        {
            LogException(ex);
            throw;
        }
        Logger.Log($"Script Loaded: {ScriptPath}, block number: {segments.Count}");
    }

    // 将相对于脚本的路径转换为相对于根目录的路径
    public PathSTD StandardizePath(string filePath) => PathSTD.From(ScriptPath.Directory) + filePath;
    public PathSTD StandardizePath(PathSTD filePath) => PathSTD.From(ScriptPath.Directory) + filePath;
}

public abstract class IScriptBlock
{
    /// <summary>
    /// 执行完当前语句块后是否继续执行
    /// </summary>
    public bool Continue = false;

    /// <summary>
    /// 执行当前代码块。
    /// </summary>
    public abstract Task Execute(VNRuntime runtime);
}

public class ChangeScript : IScriptBlock
{
    public PathSTD NewScriptPath;
    public ChangeScript(PathSTD path)
    {
        Continue = true;
        NewScriptPath = path;
    }
    public override Task Execute(VNRuntime runtime)
    {
        runtime.Script.script = new RingScript(runtime.Script.StandardizePath(NewScriptPath));
        // 重置PC
        runtime.Storage.Global.PC = -1;

        return Task.CompletedTask;
    }
}

public class Label : IScriptBlock
{
    public string Name { get; set; }

    public Label(string name)
    {
        Continue = true;
        Name = name;
    }

    // 没有意义，直接执行下一句
    public override Task Execute(VNRuntime runtime) => Task.CompletedTask;
}

/// <summary>
/// 非条件跳转，条件跳转包含在Branch中了。
/// </summary>
public class JumpToLabel : IScriptBlock
{
    public string LabelName;

    public JumpToLabel(string labelName)
    {
        Continue = true;
        this.LabelName = labelName;
    }

    public override Task Execute(VNRuntime runtime)
    {
        for (var i = 0; i < runtime.Script.script.segments.Count; i++)
        {
            if (runtime.Script.script.segments[i] is Label label && label.Name == LabelName)
            {
                // Label不做操作，可以安全跳过
                runtime.Storage.Global.PC = i;
                return Task.CompletedTask;
            }
        }
        Throw(new KeyNotFoundException($"Label {LabelName} not found."));
        return Task.CompletedTask;
    }
}

public class Branch : IScriptBlock
{
    public enum BranchType
    {
        Vertical,
        Horizontal
    }

    public record struct BranchOption(string Text, string Label);

    public BranchType Type;
    public BranchOption[] Options;

    public Branch(BranchType type, IEnumerable<BranchOption> options)
    {
        Type = type;
        Options = options.ToArray();
    }

    /// <summary>
    /// 打开选择支并设置选项回调。
    /// </summary>
    public override Task Execute(VNRuntime runtime)
    {
        switch (Type)
        {
            case BranchType.Vertical:
                NotImplemented();
                break;
            case BranchType.Horizontal:
                runtime.UI.DisplayBranch(Options);
                break;
            default:
                NotImplemented();
                break;
        }
        runtime.Paused = true;

        return Task.CompletedTask;
    }
}

public class Show : IScriptBlock
{
    public string ImgName;
    public PathSTD ImgPath;
    public string Placement;
    public string? Effect;

    public Show(PathSTD path, string placement, string? effect, string name)
    {
        Continue = true;
        ImgName = name;
        ImgPath = path;
        Placement = placement;
        Effect = effect;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        var texture = UniformLoader.Load<Texture2D>(runtime.Script.script.StandardizePath(ImgPath));
        var charas = runtime.Stage.Characters;
        var interpreter = runtime.Script.interpreter;

        var has_old = false;
        // 同名图片改名为_old
        if (charas.Has(ImgName))
        {
            has_old = true;
            charas.MarkAsOld(ImgName);
        }
        // 添加新的图片
        var img = Canvas.AddCharacter(ImgName, texture, interpreter.Eval<Placement>(Placement));
        charas.Add(ImgName, img);
        if (Effect != null)
        {
            img.Alpha().Set(0);
        }
        // 图片位置不变加Fade会导致图片闪烁
        // 只有有老图片且新图片位置不同且有动画时才添加Fade效果
        var fadeTask = Task.CompletedTask;
        if (has_old && Effect != null && img.Placement() != charas.Old[ImgName].Placement())
        {
            fadeTask = charas.Old[ImgName].Apply(OpacityEffect.Fade());
        }

        if (Effect != null)
        {
            // 应用自定义效果
            IEffect instance = interpreter.Eval(Effect);
            await charas[ImgName].Apply(instance);
        }

        // 释放同名图片
        if (has_old)
        {
            await fadeTask;
            charas.Old.Remove(ImgName).Drop();
        }
    }

    public override string ToString() =>
        $"show: name: {ImgName}, path: {ImgPath}, placement: {Placement}, effect: {Effect}";
}

public class Hide : IScriptBlock
{
    public string Name;
    public string? Effect;

    public Hide(string name, string? effect)
    {
        Name = name;
        Effect = effect;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        if (Effect != null)
        {
            await runtime.Stage.Characters[Name].Apply(runtime.Script.interpreter.Eval<IEffect>(Effect));
        }
        runtime.Stage.Characters.Remove(Name).Drop();
    }

    public override string ToString() => $"hide: name: {Name}, effect: {Effect}";
}

public class ChangeBG : IScriptBlock
{
    public PathSTD ImgPath;
    public string? Effect;

    public ChangeBG(PathSTD path, string? effect)
    {
        Continue = true;
        ImgPath = path;
        Effect = effect;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        var stage = runtime.Stage;
        var texture = UniformLoader.Load<Texture2D>(runtime.Script.StandardizePath(ImgPath));

        var newBG = Canvas.AddBG(texture, Placement.BG);
        if (Effect != null)
        {
            newBG.Alpha().Set(0);
        }
        var oldBG = stage.Background;
        stage.Background = newBG;
        stage.OldBackground = oldBG;

        if (Effect != null)
        {
            IEffect instance = runtime.Script.interpreter.Eval(Effect);
            await newBG.Apply(instance);
        }

        stage.OldBackground.Drop();
    }

    public override string ToString() => $"changeBG: path: {ImgPath}, effect: {Effect}";
}

public class ChangeScene : IScriptBlock
{
    public PathSTD BGPath;
    public string? Effect;

    public ChangeScene(PathSTD path, string? effect)
    {
        //@continue = true;
        BGPath = path;
        Effect = effect;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        //var canvas = runtime.Stage;
        //var texture = UniformLoader.Load<Texture2D>(runtime.Script.StandardizePath(BGPath));
        //if (Effect != null)
        //{
        //    ITransition instance = runtime.Script.interpreter.Eval(Effect);
        //    runtime.Animation.mainBuffer.Append(instance.Build(runtime, texture));
        //}
        //else
        //{
        //    // Legacy code，找时间删掉。
        //    Logger.Error("changeScene没有动画，为什么不用changeBG？");
        //    Unreachable();
        //}
    }
}

public class UIAnim : IScriptBlock
{
    public string Effect;

    public UIAnim(string effect)
    {
        Continue = true;
        Effect = effect;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        var ui = runtime.UI.Theme.Root;
        await ui.Apply(runtime.Script.interpreter.Eval<IEffect>(Effect));
    }
}

public class ShowChapterName : IScriptBlock
{
    public string ChapterName;

    public ShowChapterName(string chapterName)
    {
        Continue = true;
        ChapterName = chapterName;
    }

    public override Task Execute(VNRuntime runtime)
    {
        runtime.UI.DefaultTheme.ChapterName = ChapterName;
        runtime.UI.DefaultTheme.ChapterNameAlpha = 0;
        _ = runtime.UI.DefaultTheme.ChapterNameBack.Apply(
            new ChainEffect(
            OpacityEffect.Dissolve(),
            new Delay(2.0),
            OpacityEffect.Fade()));
        return Task.CompletedTask;
    }

    public override string ToString() => $"showChapterName: {ChapterName}";
}

public class Say : IScriptBlock
{
    public string Name;
    public string Content;

    public Say(string name, string content)
    {
        Name = name;
        Content = content;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        var UI = runtime.UI;
        UI.DefaultTheme.TextBox.VisibleRatio = 0;
        UI.DefaultTheme.CharacterSay(Name, Content);
        await new MethodInterpolation<float>(
                UI.DefaultTheme.TextBoxVisibleRatio,
                0,
                1,
                Content.Length / runtime.Storage.Config.TextSpeed
            ).Apply(null);
    }

    public override string ToString() => $"Say: name: {Name}, content: {Content}";
}

public class PlayAudio : IScriptBlock
{
    // path to the audio file("" excluded)
    PathSTD Path;

    // 淡入效果持续时间（秒）
    float FadeInTime;

    public PlayAudio(PathSTD path, double fadeInTime = 0.5)
    {
        Continue = true;
        Path = path;
        FadeInTime = (float)fadeInTime;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        //var audio = UniformLoader.Load<AudioStream>(runtime.Script.script.ToPathSTD(Path));
        //runtime.Animation.nonBlockingBuffer.Append(
        //    new EffectGroupBuilder()
        //        .Add(
        //            runtime.audio,
        //            new LambdaEffect(
        //                (_, tween) =>
        //                {
        //                    tween.TweenCallback(
        //                        Callable.From(() =>
        //                        {
        //                            runtime.audio.VolumeDb = -80.0f;
        //                            runtime.audio.Play(audio);
        //                        })
        //                    );
        //                    tween
        //                        .TweenProperty(runtime.audio, "volume_db", 0.0, FadeInTime)
        //                        .SetTrans(Tween.TransitionType.Expo)
        //                        .SetEase(Tween.EaseType.Out);
        //                }
        //            )
        //        )
        //        .Build()
        //);
    }
}

public class StopAudio : IScriptBlock
{
    float FadeOutTime;

    public StopAudio(double fadeOutTime = 1.0)
    {
        Continue = true;
        FadeOutTime = (float)fadeOutTime;
    }

    public override async Task Execute(VNRuntime runtime)
    {
        //runtime.nonBlockingBuffer.Append(
        //    new EffectGroupBuilder()
        //        .Add(
        //            runtime.audio,
        //            new LambdaEffect(
        //                (_, tween) =>
        //                    tween
        //                        .TweenProperty(runtime.audio, "volume_db", -80.0, FadeOutTime)
        //                        .SetTrans(Tween.TransitionType.Expo)
        //                        .SetEase(Tween.EaseType.In)
        //            )
        //        )
        //        .Build()
        //);
    }
}

public class CodeBlock : IScriptBlock
{
    // language specified in markdown codeblock(unused)
    public string Identifier;
    public string Code;

    public CodeBlock(string identifier, string code)
    {
        Continue = true;
        Code = code;
        Identifier = identifier;
    }

    /// <summary>
    /// 执行Python代码块,异常处理交给外层
    /// </summary>
    /// <exception cref="PythonException">Python解释器抛出的异常</exception>
    public override Task Execute(VNRuntime runtime)
    {
        runtime.Script.interpreter.Exec(Code);
        return Task.CompletedTask;
    }

    public override string ToString() => $"CodeBlock: identifier: {Identifier}, code: {Code}";
}
