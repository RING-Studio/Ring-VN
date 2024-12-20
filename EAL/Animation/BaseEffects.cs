namespace RingEngine.EAL.Animation;

using System;
using Godot;

// 用于参数类型
public delegate void EffectFunc(Tween tween);
public delegate void CallBack();

public class LambdaEffect : IEffect
{
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
