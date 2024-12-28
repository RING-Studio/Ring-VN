namespace RingEngine.Core.Animation;

using Godot;
using RingEngine.Core.General;

/// <summary>
/// 对单个节点应用的效果，不能访问其它节点，不能删除节点
/// </summary>
public abstract class IEffect
{
    public delegate Node EvaluateTargetFunc();

    protected EvaluateTargetFunc EvaluateTarget = null;
    protected Node _Target = null;
    public Node Target
    {
        get
        {
            _Target ??= EvaluateTarget();
            return _Target;
        }
    }

    /// <summary>
    /// <para>对节点应用当前效果</para>
    /// 为防止并行时出错，每个Apply中只能调用一次Tween的方法
    /// </summary>
    public abstract void Apply(Tween tween);

    /// <summary>
    /// 获取效果的持续时间
    /// </summary>
    public abstract double GetDuration();

    /// <summary>
    /// 仅用于绑定到Runtime的静态节点，涉及Removable节点都应该使用Bind(EvaluateTargetFunc)
    /// </summary>
    public IEffect Bind(Node target)
    {
        // Pythonnet的全局变量会被重复调用，导致Target被继承，COW防止共享引用
        var copy = Clone();
        copy._Target = target;
        return copy;
    }

    public IEffect Bind(EvaluateTargetFunc evaluateTarget)
    {
        var copy = Clone();
        copy.EvaluateTarget = evaluateTarget;
        return copy;
    }

    public IEffect Clone()
    {
        var clone = (IEffect)MemberwiseClone();
        AssertWrapper.Assert(clone != this);
        clone._Target = null;
        clone.EvaluateTarget = null;
        return clone;
    }

    public void ClearTarget()
    {
        _Target = null;
        EvaluateTarget = null;
    }
}
