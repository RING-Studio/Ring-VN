namespace RingEngine.Core;

using System;
using System.Linq;
using Animation;
using Audio;
using RingEngine.Core.General;
using Script;
using Stage;
using Storage;
using UI;
using static General.AssertWrapper;

public class VNRuntime
{
    public bool CanSnapshot = true;
    public ScriptModule Script;
    public UIModule UI;
    public StageModule Stage;
    public StorageModule Storage;
    public AnimationModule Animation;
    public AudioModule Audio;

    public VNRuntime()
    {
        Script = new ScriptModule(this, "res://main.md", "res://init.py");
        UI = new UIModule();
        Stage = new StageModule();
        Storage = new StorageModule();
        Animation = new AnimationModule();
        Audio = new AudioModule();
    }

    /// <summary>
    /// 创建当前状态快照，保存前确保处于一个稳定状态
    /// </summary>
    /// <returns></returns>
    public Snapshot Save()
    {
        Assert(CanSnapshot, "Can't snapshot in this state!");
        return new Snapshot(this);
    }

    public void LoadSnapshot(Snapshot snapshot)
    {
        SceneTreeProxy.Deserialize(snapshot.GodotSceneTree);
        Script.Deserialize(snapshot.ScriptPath);
        Stage.Deserialize();
        Storage.Deserialize(snapshot.Global);
        Storage.Global.History = snapshot.History.ToList();
    }

    /// <summary>
    /// 运行脚本至下一个中断点
    /// </summary>
    public void Step()
    {
        if (Animation.nonBlockingBuffer.IsRunning)
        {
            Animation.nonBlockingBuffer.Interrupt();
        }
        if (Animation.mainBuffer.IsRunning)
        {
            Animation.mainBuffer.Interrupt();
            return;
        }
        if (Storage.Global.PC < Script.Length)
        {
            // 脚本执行前是稳态，所有动画都已结束，在这里进行Snapshot
            Storage.Global.History.Add(new Snapshot(this));
            Script.Step(ref Storage.Global.PC, this);
        }
        else
        {
            throw new IndexOutOfRangeException("Script Out of Bound!");
        }
    }

    //public void Backlog()
    //{
    //    GetParent<Runtime>().SwitchRuntime(this, "Backlog", Global.History);
    //}

    //public void Setting() => SwitchRuntime("Setting", "AVG");

    //public void Goto(string label)
    //{
    //    // Execute结束后会PC++，所以这里要减1
    //    Global.PC = script.labels[label] - 1;
    //}
}
