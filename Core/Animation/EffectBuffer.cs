namespace RingEngine.Core.Animation;

using System.Collections.Generic;
using System.Linq;
using Godot;
using RingEngine.Core.General;
using static RingEngine.Core.General.AssertWrapper;

public class EffectGroup
{
    public List<IEffect> Effects;

    /// <summary>
    /// Tween绑定对象，如果为null则绑定在RingIO上
    /// </summary>
    public Node Driver = null;

    public EffectGroup(List<IEffect> effects, Node target)
    {
        Effects = effects;
        Driver = target;
    }
}

/// <summary>
/// 构建一组顺序作用在同一个对象上的效果
/// </summary>
public class EffectGroupBuilder
{
    public List<IEffect> Effects = [];

    public EffectGroupBuilder Add(LambdaEffect.CallBack effect)
    {
        Effects.Add(new LambdaEffect(effect));
        return this;
    }

    public EffectGroupBuilder Add(IEffect effect)
    {
        Effects.Add(effect);
        return this;
    }

    /// <summary>
    /// 构建效果组并绑定作用对象，如果不指定对象则在全局Tween上执行
    /// </summary>
    public EffectGroup Build(Node target = null) => new(Effects, target);
}

public class EffectBuffer
{
    // 当前正在运行的效果
    Tween RunningEffect = null;
    EffectGroup RunningGroup = null;

    // Tween是否活动
    public bool IsRunning => RunningEffect != null && RunningEffect.IsRunning();

    // 等待队列
    List<EffectGroup> buffer = [];

    public bool HasPendingAnimation =>
        IsRunning || RunningGroup.Effects.Count > 0 || buffer.Count > 0;

    public void Append(EffectGroup group) => buffer.Add(group);

    public void Append(IEnumerable<EffectGroup> groups) => buffer.AddRange(groups);

    // TODO: Interrupt的时候是打断所有排队的动画还是只打断当前正在运行的动画？
    public void Interrupt()
    {
        if (IsRunning)
        {
            RunningEffect.Pause();
            RunningEffect.CustomStep(114514);
            RunningEffect.Kill();
            RunningEffect = null;
        }
    }

    /// <summary>
    /// 执行下一个效果
    /// </summary>
    /// <returns>执行是否成功，失败表示RunningGroup已经执行完毕或为null</returns>
    public bool Step()
    {
        if (RunningGroup == null || RunningGroup.Effects.Count == 0)
        {
            RunningGroup = null;
            return false;
        }

        var effect = RunningGroup.Effects.First();
        RunningGroup.Effects.RemoveAt(0);
        // 在这个位置上节点的动画应当已经执行完成（或被打断），多个buffer争用节点由caller解决
        var tween =
            RunningGroup.Driver == null
                ? SceneTreeProxy.RingIO.CreateTween()
                : RunningGroup.Driver.CreateTween();
        effect.Apply(tween);
        RunningEffect = tween;
        return true;
    }

    /// <summary>
    /// 外部轮询触发
    /// </summary>
    public void Execute()
    {
        if (!IsRunning && !Step())
        {
            if (buffer.Count == 0)
            {
                return;
            }
            RunningGroup = buffer.First();
            buffer.RemoveAt(0);
            Assert(Step());
        }
    }
}
