namespace RingEngine.EAL.Animation;

using Godot;
using RingEngine.EAL.Resource;

public class SetAlpha : IEffect
{
    public static SetAlpha Transparent = new(0);
    public static SetAlpha Opaque = new(1);

    public float Alpha;

    public SetAlpha(double alpha)
    {
        Alpha = (float)alpha;
    }

    public override void Apply(Tween tween)
    {
        tween.TweenCallback(Callable.From(() => (Target as IRenderable).Alpha = Alpha));
    }

    public override double GetDuration() => 0;
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

    public override void Apply(Tween tween)
    {
        tween.TweenProperty(Target as Sprite2D, "modulate:a", EndAlpha, Duration);
    }

    public override double GetDuration() => Duration;
}
