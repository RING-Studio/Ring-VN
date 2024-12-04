namespace RingEngine.EAL.Animation;

using Godot;
using RingEngine.EAL.Resource;

public interface ITweenable { }

/// <summary>
/// 对单个节点应用的效果，不能访问其它节点，不能删除节点
/// </summary>
public abstract class IEffect
{
    public delegate ITweenable EvaluateTargetFunc();

    EvaluateTargetFunc EvaluateTarget = null;
    ITweenable _Target = null;
    public ITweenable Target
    {
        get
        {
            _Target ??= EvaluateTarget();
            return _Target;
        }
    }

    /// <summary>
    /// 对节点应用当前效果
    /// </summary>
    public abstract void Apply(Tween tween);

    /// <summary>
    /// 获取效果的持续时间
    /// </summary>
    public abstract double GetDuration();

    public IEffect Bind(ITweenable target)
    {
        _Target = target;
        return this;
    }

    public IEffect Bind(EvaluateTargetFunc evaluateTarget)
    {
        EvaluateTarget = evaluateTarget;
        return this;
    }
}
