using System;
using System.Collections.Generic;
using Godot;
using RingEngine.Core;
using RingEngine.Core.Animation;
using RingEngine.Core.General;
using RingEngine.Core.Storage;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1050:在命名空间中声明类型",
    Justification = "Godot"
)]
public partial class RingIO : Node
{
    public VNRuntime Runtime;
    public Queue<Callable> Callbacks = [];

    [Export]
    public int PC
    {
        get => Runtime.Storage.Global.PC;
        set => Runtime.Storage.Global.PC = value;
    }

    /// <summary>
    /// 快速完成所有pending的动画，可能需要多帧时间
    /// </summary>
    /// <param name="then">清理动画后需要做的事</param>
    public void FlushAllAnimation(Callable then)
    {
        if (!TweenManager.IsEmpty())
        {
            TweenManager.Flush();
            Callbacks.Enqueue(Callable.From(() => FlushAllAnimation(then)));
        }
        else
        {
            then.Call();
        }
    }

    public override void _Ready()
    {
        Name = "RingIO";
        // 初始化场景树
        var _ = new SceneTreeProxy();
        Runtime = new VNRuntime();
    }

    public override void _Process(double delta)
    {
        var cnt = Callbacks.Count;
        for (var i = 0; i < cnt; i++)
        {
            var callback = Callbacks.Dequeue();
            callback.Call();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey)
        {
            if (Input.IsActionPressed("ui_accept"))
            {
                Runtime.Step();
            }
            else if (Input.IsActionPressed("ui_cancel"))
            {
                //Setting();
            }
            else if (Input.IsActionPressed("Save"))
            {
                FlushAllAnimation(
                    Callable.From(() =>
                    {
                        var snap = Runtime.Save();
                        snap.Save("res://snapshot");
                    })
                );
            }
            else if (Input.IsActionPressed("ui_text_backspace"))
            {
                FlushAllAnimation(
                    Callable.From(() =>
                    {
                        try
                        {
                            Runtime.LoadSnapshot(Runtime.Storage.Global.LoadHistory(1));
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            GD.Print("History is empty.");
                        }
                    })
                );
            }
            else if (Input.IsActionPressed("Load"))
            {
                FlushAllAnimation(
                    Callable.From(() =>
                    {
                        var snap = new Snapshot("res://snapshot");
                        Runtime.LoadSnapshot(snap);
                    })
                );
            }
        }
    }
}
