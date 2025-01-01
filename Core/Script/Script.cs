namespace RingEngine.Core.Script;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    /// 执行过程中不应该产生任何立即效果，因为可能有前续动画没有完成，把所有用户可见的操作全都放到动画队列里。
    /// </summary>
    public abstract void Execute(VNRuntime runtime);
}

public class ChangeScript : IScriptBlock
{
    public PathSTD NewScriptPath;
    public ChangeScript(PathSTD path)
    {
        Continue = true;
        NewScriptPath = path;
    }
    public override void Execute(VNRuntime runtime)
    {
        runtime.Script.script = new RingScript(runtime.Script.StandardizePath(NewScriptPath));
        // 重置PC
        runtime.Storage.Global.PC = -1;
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
    public override void Execute(VNRuntime runtime) { }
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

    public override void Execute(VNRuntime runtime)
    {
        for (var i = 0; i < runtime.Script.script.segments.Count; i++)
        {
            if (runtime.Script.script.segments[i] is Label label && label.Name == LabelName)
            {
                // Label不做操作，可以安全跳过
                runtime.Storage.Global.PC = i;
                return;
            }
        }
        Throw(new KeyNotFoundException($"Label {LabelName} not found."));
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
    public override void Execute(VNRuntime runtime)
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

    public override void Execute(VNRuntime runtime)
    {
        var texture = UniformLoader.Load<Texture2D>(runtime.Script.script.StandardizePath(ImgPath));
        var charas = runtime.Stage.Characters;
        var interpreter = runtime.Script.interpreter;
        var effects = new EffectGroupBuilder();

        effects.Add(() =>
        {
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
            if (has_old && Effect != null && img.Placement() != charas.Old[ImgName].Placement())
            {
                // TODO:找时间解决这个workaround
                runtime.Animation.AddTempEffect(
                    OpacityEffect.Fade().Bind(() => charas.Old[ImgName])
                );
            }
        });

        if (Effect != null)
        {
            // 应用自定义效果
            IEffect instance = interpreter.Eval(Effect);
            effects.Add(instance.Bind(() => charas[ImgName]));
        }

        effects.Add(() =>
        {
            // 释放同名图片
            if (charas.Old.Has(ImgName))
            {
                charas.Old.Remove(ImgName).Drop();
            }
        });
        runtime.Animation.mainBuffer.Append(effects.Build());
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

    public override void Execute(VNRuntime runtime)
    {
        var effects = new EffectGroupBuilder();
        if (Effect != null)
        {
            effects.Add(
                runtime
                    .Script.interpreter.Eval<IEffect>(Effect)
                    .Bind(() => runtime.Stage.Characters[Name])
            );
        }
        effects.Add(() => runtime.Stage.Characters.Remove(Name).Drop());
        runtime.Animation.mainBuffer.Append(effects.Build());
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

    public override void Execute(VNRuntime runtime)
    {
        var stage = runtime.Stage;
        var texture = UniformLoader.Load<Texture2D>(runtime.Script.StandardizePath(ImgPath));
        var effects = new EffectGroupBuilder();
        effects.Add(() =>
        {
            var newBG = Canvas.AddBG(texture, Placement.BG);
            if (Effect != null)
            {
                newBG.Alpha().Set(0);
            }
            var oldBG = stage.Background;
            stage.Background = newBG;
            stage.OldBackground = oldBG;
        });

        if (Effect != null)
        {
            IEffect instance = runtime.Script.interpreter.Eval(Effect);
            effects.Add(instance.Bind(() => stage.Background));
        }
        effects.Add(() => stage.OldBackground.Drop());
        runtime.Animation.mainBuffer.Append(effects.Build());
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

    public override void Execute(VNRuntime runtime)
    {
        var canvas = runtime.Stage;
        var texture = UniformLoader.Load<Texture2D>(runtime.Script.StandardizePath(BGPath));
        var effects = new EffectGroupBuilder();
        if (Effect != null)
        {
            ITransition instance = runtime.Script.interpreter.Eval(Effect);
            runtime.Animation.mainBuffer.Append(instance.Build(runtime, texture));
        }
        else
        {
            // Legacy code，找时间删掉。
            Logger.Error("changeScene没有动画，为什么不用changeBG？");
            Unreachable();
        }
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

    public override void Execute(VNRuntime runtime)
    {
        runtime.Animation.mainBuffer.Append(
            new EffectGroupBuilder()
                .Add(runtime.Script.interpreter.Eval<IEffect>(Effect).Bind(runtime.UI.Root))
                .Build()
        );
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

    public override void Execute(VNRuntime runtime)
    {
        var effects = new EffectGroupBuilder();
        effects.Add(() =>
        {
            runtime.UI.ChapterName = ChapterName;
            runtime.UI.ChapterNameAlpha = 0;
        });
        effects.Add(OpacityEffect.Dissolve().Bind(runtime.UI.ChapterNameBack));
        effects.Add(new Delay(2.0));
        effects.Add(OpacityEffect.Fade().Bind(runtime.UI.ChapterNameBack));
        runtime.Animation.nonBlockingBuffer.Append(effects.Build());
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

    public override void Execute(VNRuntime runtime)
    {
        var UI = runtime.UI;
        var effects = new EffectGroupBuilder();
        effects.Add(() =>
        {
            UI.TextBox.VisibleRatio = 0;
            UI.CharacterSay(Name, Content);
        });
        effects.Add(
            new MethodInterpolation<float>(
                UI.TextBoxVisibleRatio,
                0,
                1,
                Content.Length / runtime.Storage.Config.TextSpeed
            )
        );
        runtime.Animation.mainBuffer.Append(effects.Build());
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

    public override void Execute(VNRuntime runtime)
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

    public override void Execute(VNRuntime runtime)
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
    public override void Execute(VNRuntime runtime)
    {
        runtime.Script.interpreter.Exec(Code);
    }

    public override string ToString() => $"CodeBlock: identifier: {Identifier}, code: {Code}";
}
