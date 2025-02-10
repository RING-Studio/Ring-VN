namespace RingEngine.Legacy;

using System.Collections.Generic;
using System.Linq;
using Godot;
using RingEngine.Core;
using RingEngine.Core.General;
using RingEngine.Core.Stage;

/// <summary>
/// 需要全局信息的效果
/// </summary>
//public interface ITransition
//{
//    /// <summary>
//    /// 构建转场效果
//    /// </summary>
//    /// <param name="runtime"></param>
//    /// <param name="newBG"></param>
//    /// <returns>若干效果组，直接提交给EffectBuffer即可</returns>
//    public IEnumerable<EffectGroup> Build(VNRuntime runtime, Texture2D newBG);

//    /// <summary>
//    /// 获取转场的持续时间
//    /// </summary>
//    public double GetDuration();
//}

//public class DissolveTrans : ITransition
//{
//    public Texture2D mask;
//    public double duration;

//    public DissolveTrans(Texture2D mask = null, double duration = 2)
//    {
//        this.mask = UniformLoader.Load<Texture2D>("res://assets/Runtime/black.png");
//        this.duration = duration;
//    }

//    public IEnumerable<EffectGroup> Build(VNRuntime runtime, Texture2D newBG)
//    {
//        var stage = runtime.Stage;
//        var group1 = new EffectGroupBuilder()
//            .Add(
//                new LambdaEffect(() =>
//                {
//                    stage.Mask?.Drop();
//                    stage.Mask = Canvas.AddMask("Mask", mask, Placement.BG);
//                    stage.Mask.Alpha().Set(0);
//                })
//            )
//            .Add(OpacityEffect.Dissolve(duration / 2))
//            .Add(new LambdaEffect(() => stage.Background.Texture = newBG))
//            .Add(OpacityEffect.Fade(duration / 2))
//            .Add(
//                new LambdaEffect(() =>
//                {
//                    stage.Mask.Drop();
//                    stage.Mask = null;
//                })
//            )
//            .Build(stage.Mask);
//        return new[] { group1 }.AsEnumerable();
//    }

//    public double GetDuration() => duration;
//}

//public class ImageTrans : ITransition
//{
//    public string maskPath = "res://assets/Runtime/black.png";
//    public string controlPath;
//    public double duration;
//    public bool reversed;
//    public float smooth;

//    public ImageTrans(double duration = 2, bool reversed = false, double smooth = 0.2)
//    {
//        this.controlPath = "res://assets/Runtime/rule_10.png";
//        this.duration = duration;
//        this.reversed = reversed;
//        this.smooth = (float)smooth;
//    }

//    public ImageTrans(
//        string controlPath,
//        double duration = 2,
//        bool reversed = false,
//        double smooth = 0.2
//    )
//    {
//        this.controlPath = controlPath;
//        this.duration = duration;
//        this.reversed = reversed;
//        this.smooth = (float)smooth;
//    }

//    public IEnumerable<EffectGroup> Build(VNRuntime runtime, Texture2D2D newBG)
//    {
//        if (!maskPath.StartsWith("res://"))
//        {
//            maskPath = runtime.script.ToResourcePath(maskPath);
//        }
//        if (!controlPath.StartsWith("res://"))
//        {
//            controlPath = runtime.script.ToResourcePath(controlPath);
//        }
//        var mask = GD.Load<Texture2D2D>(maskPath);
//        var control = GD.Load<Texture2D2D>(controlPath);
//        var canvas = runtime.canvas;
//        var group1 = new EffectGroupBuilder()
//            .Add(
//                canvas.Mask,
//                new Chain(
//                    new LambdaEffect(() =>
//                    {
//                        canvas.AddMask(mask);
//                        var material = new ShaderMaterial
//                        {
//                            Shader = GD.Load<Shader>(
//                                "res://Runtime/VNRuntime/Effect/mask.gdshader"
//                            ),
//                        };
//                        material.SetShaderParameter("progress", 0.0);
//                        material.SetShaderParameter("smooth_size", smooth);
//                        material.SetShaderParameter("control", control);
//                        material.SetShaderParameter("reversed", reversed);
//                        canvas.Mask.Material = material;
//                    }),
//                    new LambdaEffect(
//                        (node, tween) =>
//                            tween.TweenMethod(
//                                Callable.From(
//                                    (float progress) =>
//                                        ((ShaderMaterial)canvas.Mask.Material).SetShaderParameter(
//                                            "progress",
//                                            progress
//                                        )
//                                ),
//                                0.0,
//                                1.0,
//                                duration / 2
//                            )
//                    )
//                )
//            )
//            .Build();
//        var group2 = new EffectGroupBuilder()
//            .Add(
//                canvas.Mask,
//                new Chain(
//                    new LambdaEffect(() => canvas.BG.Texture2D = canvas.Stretch(newBG)),
//                    new LambdaEffect(
//                        (node, tween) =>
//                            tween.TweenMethod(
//                                Callable.From(
//                                    (float progress) =>
//                                        ((ShaderMaterial)canvas.Mask.Material).SetShaderParameter(
//                                            "progress",
//                                            progress
//                                        )
//                                ),
//                                1.0,
//                                0.0,
//                                duration / 2
//                            )
//                    ),
//                    new LambdaEffect(() => canvas.Mask.Material = null),
//                    new LambdaEffect(canvas.RemoveMask)
//                )
//            )
//            .Build();
//        return new[] { group1, group2 }.AsEnumerable();
//    }

//    public double GetDuration() => duration;
//}
