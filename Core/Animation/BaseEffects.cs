namespace RingEngine.Core.Animation;

using System;
using System.Linq;
using Godot;

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

    public override void Apply(Tween tween)
    {
        Func(tween);
    }

    public override double GetDuration() => Duration;
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

    public override void Apply(Tween tween)
    {
        tween.TweenMethod(
            Callable.From(new Action<T>(Func)),
            Variant.From(From),
            Variant.From(To),
            Duration
        );
    }

    public override double GetDuration() => Duration;
}

public class Delay : IEffect
{
    public double Duration;

    public Delay(double duration)
    {
        Duration = duration;
    }

    public override void Apply(Tween tween)
    {
        tween.TweenInterval(Duration);
    }

    public override double GetDuration() => Duration;
}

public class ParallelEffect : IEffect
{
    public IEffect[] Effects;
    public ParallelEffect(params IEffect[] effects)
    {
        Effects = effects;
    }

    public static ParallelEffect From(params IEffect[] effects) => new(effects);

    public override void Apply(Tween tween)
    {
        tween.SetParallel();
        var first_effect = Effects[0];
        first_effect.Apply(tween);
        // 假设每个Apply中只有一个Tween方法
        tween.SetParallel();
        foreach (var effect in Effects[1..])
        {
            effect.Apply(tween);
        }
        tween.SetParallel(false);
    }
    public override double GetDuration() => Effects.Select(effect => effect.GetDuration()).Max();
}
