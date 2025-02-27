namespace RingEngine.Core.Animation;

using System;
using System.Threading.Tasks;
using Godot;
using RingEngine.Core.General;

/// <summary>
/// 对单个节点应用的效果，不能访问其它节点，不能删除节点
/// </summary>
public interface IEffect
{
    /// <summary>
    /// <para>对节点应用当前效果</para>
    /// </summary>
    public Task Apply(Node target);
}

public class ChainEffect : IEffect
{
    public IEffect[] Effects;

    private ChainEffect(params IEffect[] effects)
    {
        Effects = effects;
    }

    public static ChainEffect New(params IEffect[] effects) => new(effects);

    public async Task Apply(Node target)
    {
        foreach (var effect in Effects)
        {
            await effect.Apply(target);
        }
    }
}

public static class MethodInterpolation
{
    public static MethodInterpolation<T> New<T>(
        MethodInterpolation<T>.Method func,
        T from,
        T to,
        double duration
    ) => new(func, from, to, duration);
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

    public async Task Apply(Node target = null)
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
}

public class Delay : IEffect
{
    public double Duration;

    private Delay(double duration)
    {
        Duration = duration;
    }

    public static Delay New(double duration) => new(duration);

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween();
        tween.TweenInterval(Duration);
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }
}

public class SetAlpha : IEffect
{
    public static SetAlpha Transparent = new(0);
    public static SetAlpha Opaque = new(1);

    public float Alpha;

    private SetAlpha(double alpha)
    {
        Alpha = (float)alpha;
    }

    public static SetAlpha New(double alpha) => new(alpha);

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween(target);
        tween.TweenCallback(Callable.From(() => (target as CanvasItem).Alpha().Set(Alpha)));
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }
}

public class OpacityEffect : IEffect
{
    public static OpacityEffect Dissolve(double duration = 1.0) => new(1.0, duration);

    public static OpacityEffect Fade(double duration = 1.0) => new(0.0, duration);

    public float EndAlpha;
    public double Duration;

    private OpacityEffect(double endAlpha, double duration)
    {
        EndAlpha = (float)endAlpha;
        Duration = duration;
    }

    public static OpacityEffect New(double endAlpha, double duration) => new(endAlpha, duration);

    public async Task Apply(Node target)
    {
        var tween = TweenManager.CreateTween(target);
        var endModulate = (target as CanvasItem).Modulate;
        endModulate.A = EndAlpha;
        tween.TweenProperty(target, "modulate", endModulate, Duration);
        await tween.ToSignal(tween, Tween.SignalName.Finished);
    }

    public override string ToString() =>
        $"{(EndAlpha == 1.0 ? "DissolveEffect" : EndAlpha == 0.0 ? "FadeEffect" : "OpacityEffect")}";
}
