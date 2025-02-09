namespace RingEngine.Core.Animation2;

using System.Collections.Generic;
using System.Linq;
using Godot;
using RingEngine.Core.General;
using static General.AssertWrapper;

/// <summary>
/// 集中管理所有节点的Tween以便于快进操作
/// </summary>
public static class TweenManager
{
    static HashSet<Tween> Tweens = [];

    public static bool IsEmpty() => Tweens.Count == 0;

    static void AddTween(Tween tween)
    {
        if (Tweens.Contains(tween))
        {
            Logger.Warn("Trying to add a tween that is already in the manager.");
            return;
        }

        // 防止空Tween报Start with no Tweener
        tween.TweenCallback(Callable.From(() => { }));

        Tweens.Add(tween);
        tween.Finished += () => Tweens.Remove(tween);
    }

    /// <summary>
    /// 在给定节点上创建一个Tween
    /// </summary>
    public static Tween CreateTween(Node node)
    {
        var tween = node.CreateTween();
        AddTween(tween);
        return tween;
    }

    /// <summary>
    /// 在默认节点（RingIO）上创建一个Tween
    /// </summary>
    public static Tween CreateTween()
    {
        var tween = SceneTreeProxy.RingIO.CreateTween();
        AddTween(tween);
        return tween;
    }

    /// <summary>
    /// 立即完成所有正在运行的动画
    /// </summary>
    public static void Flush()
    {
        foreach (var tween in Tweens.ToArray())
        {
            tween.CustomStep(114514);
        }
    }
}
