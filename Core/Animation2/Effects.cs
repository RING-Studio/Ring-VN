namespace RingEngine.Core.Animation2;
using Godot;
using System;
using RingEngine.Core.General;
using System.Threading.Tasks;
using System.Linq;

/// <summary>
/// 对单个节点应用的效果，不能访问其它节点，不能删除节点
/// </summary>
public interface IEffect
{
    /// <summary>
    /// <para>对节点应用当前效果</para>
    /// </summary>
    public Task Apply(Node target);

    /// <summary>
    /// 获取效果的持续时间
    /// </summary>
    public double GetDuration();
}

public class ChainEffect : IEffect
{
    public IEffect[] Effects;
    public ChainEffect(params IEffect[] effects)
    {
        Effects = effects;
    }
    public async Task Apply(Node target)
    {
        foreach (var effect in Effects)
        {
            await effect.Apply(target);
        }
    }
    public double GetDuration() => Effects.Select(effect => effect.GetDuration()).Sum();
}

public class LambdaEffect : IEffect
{
    // 用于参数类型
    public delegate void EffectFunc(Tween tween);
    public delegate void CallBack();

    EffectFunc Func;
    double Duration;

    public LambdaEffect(EffectFunc func, double duration = 0)
    {
        Func = func;
        Duration = duration;
    }

    /// <summary>
    /// 仅包含单个TweenCallBack的LambdaEffect
    /// </summary>
    /// <param name="callBack">要调用的CallBack</param>
    /// <param name="duration">效果持续时间</param>
    public LambdaEffect(CallBack callBack, double duration = 0)
    {
        Func = (tween) => tween.TweenCallback(Callable.From(() => callBack()));
        Duration = duration;
    }

    public static LambdaEffect From(CallBack callBack, double duration = 0) =>
        new(callBack, duration);

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween();
        Func(tween);
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }

    public double GetDuration() => Duration;
}

public class MethodInterpolation<T> : IEffect
{
    public delegate void Method(T arg);
    Method Func;
    T From;
    T To;
    double Duration;

    public MethodInterpolation(Method func, T from, T to, double duration)
    {
        Func = func;
        From = from;
        To = to;
        Duration = duration;
    }

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween();
        tween.TweenMethod(
            Callable.From(new Action<T>(Func)),
            Variant.From(From),
            Variant.From(To),
            Duration
        );
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }

    public double GetDuration() => Duration;
}

public class Delay : IEffect
{
    public double Duration;

    public Delay(double duration)
    {
        Duration = duration;
    }

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween();
        tween.TweenInterval(Duration);
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }

    public double GetDuration() => Duration;
}

public class SetAlpha : IEffect
{
    public static SetAlpha Transparent = new(0);
    public static SetAlpha Opaque = new(1);

    public float Alpha;

    public SetAlpha(double alpha)
    {
        Alpha = (float)alpha;
    }

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween(target);
        tween.TweenCallback(Callable.From(() => (target as CanvasItem).Alpha().Set(Alpha)));
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }

    public double GetDuration() => 0;
}

public class OpacityEffect : IEffect
{
    public static OpacityEffect Dissolve(double duration = 1.0) => new(1.0, duration);

    public static OpacityEffect Fade(double duration = 1.0) => new(0.0, duration);

    public float EndAlpha;
    public double Duration;

    public OpacityEffect(double endAlpha, double duration)
    {
        EndAlpha = (float)endAlpha;
        Duration = duration;
    }

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween(target);
        var endModulate = (target as CanvasItem).Modulate;
        endModulate.A = EndAlpha;
        tween.TweenProperty(target, "modulate", endModulate, Duration);
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }

    public double GetDuration() => Duration;

    public override string ToString() =>
        $"{(EndAlpha == 1.0 ? "DissolveEffect" : EndAlpha == 0.0 ? "FadeEffect" : "OpacityEffect")}";
}
