namespace RingEngine.Core;

using System;
using System.Linq;
using System.Threading.Tasks;
using Audio;
using RingEngine.Core.Animation;
using RingEngine.Core.General;
using Script;
using Stage;
using Storage;
using UI;
using static General.AssertWrapper;

public class VNRuntime
{
    public bool CanSnapshot = true;
    public bool Paused
    {
        get => Storage.Global.Paused;
        set => Storage.Global.Paused = value;
    }
    public ScriptModule Script;
    public UIModule UI;
    public StageModule Stage;
    public StorageModule Storage;
    public AudioModule Audio;
    public Task CurrentTask = null;

    public VNRuntime()
    {
        Script = new ScriptModule(this, "res://main.md", "res://init.py");
        UI = new UIModule()
        {
            runtime = this,
            Theme = new DefaultTheme()
        };
        Stage = new StageModule();
        Storage = new StorageModule();
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

    public bool GetNextScriptBlock(out Task task)
    {
        throw new NotImplementedException();

    }

    /// <summary>
    /// 运行脚本至下一个中断点
    /// </summary>
    public void Step()
    {
        if (Storage.Global.Paused)
        {
            Logger.Log("Runtime is paused, no stepping.");
            return;
        }
        // 如果当前没有任务或者任务已经完成，那么就执行到下一个保存点
        if (CurrentTask == null || CurrentTask.IsCompleted)
        {
            if (Storage.Global.PC < Script.Length)
            {
                // 脚本执行前是稳态，所有动画都已结束，在这里进行Snapshot
                Storage.Global.History.Add(new Snapshot(this));
                CurrentTask = Script.Step(this);
            }
            else
            {
                Logger.Error("Script Out of Bound!");
            }
        }
        // 动画进行中调用Step会刷新当前正在运行的步骤
        else if (CurrentTask != null && !CurrentTask.IsCompleted)
        {
            TweenManager.Flush();
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
