namespace RingEngine.Core.Animation;

using Godot;

/// <summary>
/// 对单个节点应用的效果，不能访问其它节点，不能删除节点
/// </summary>
public abstract class IEffect
{
    public delegate Node EvaluateTargetFunc();

    EvaluateTargetFunc EvaluateTarget = null;
    Node _Target = null;
    public Node Target
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

    /// <summary>
    /// 仅用于绑定到Runtime的静态节点，涉及Removable节点都应该使用Bind(EvaluateTargetFunc)
    /// </summary>
    public IEffect Bind(Node target)
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
