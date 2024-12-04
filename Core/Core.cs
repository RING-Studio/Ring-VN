namespace RingEngine.Core;

using Animation.Export;
using Audio.Export;
using Script.Export;
using Stage.Export;
using Storage.Export;
using UI.Export;

public class VNRuntime
{
    public ScriptModule Script;
    public UIModule UI;
    public StageModule Stage;
    public StorageModule Storage;
    public AnimationModule Animation;
    public AudioModule Audio;

    //public AVGRuntime()
    //{
    //    script = new RingScript("res://main.md");
    //    UI = GD.Load<PackedScene>("res://Runtime/AVGRuntime/UI/UI.tscn").Instantiate<UI>();
    //    UI.Name = "UI";
    //    // 强制显示在canvas之上
    //    UI.ZIndex = 1;
    //    AddChild(UI);
    //    canvas = new Canvas { Name = "Canvas", };
    //    AddChild(canvas);
    //    audio = new Audio() { Name = "Audio" };
    //    AddChild(audio);
    //    mainBuffer = new EffectBuffer();
    //    nonBlockingBuffer = new EffectBuffer();
    //    interpreter = new PythonInterpreter(
    //        this,
    //        GlobalConfig.ProjectRoot,
    //        FileAccess.GetFileAsString("res://init.py")
    //    );
    //    Global = new DataBase();
    //}

    //public ISnapshot Save()
    //{
    //    if (Global.IsExecuting)
    //    {
    //        Global.PC++;
    //        Global.IsExecuting = false;
    //        var ret = new Snapshot(this);
    //        Global.PC--;
    //        Global.IsExecuting = true;
    //        return ret;
    //    }
    //    return new Snapshot(this);
    //}

    //public void LoadSnapshot(ISnapshot snapshot)
    //{
    //    var snapshotConcrete = snapshot as Snapshot;
    //    RemoveChild(UI);
    //    UI.QueueFree();
    //    RemoveChild(canvas);
    //    canvas.QueueFree();
    //    UI = snapshotConcrete.UI.Instantiate<UI>();
    //    AddChild(UI);
    //    canvas = snapshotConcrete.Canvas.Instantiate<Canvas>();
    //    AddChild(canvas);
    //    Global = DataBase.Deserialize(snapshotConcrete.Global);
    //    Global.History = snapshotConcrete.History.ToList();
    //}

    //public void GetMessage(string runtimeName, object message)
    //{
    //    GD.Print($"{runtimeName} send message {message}");
    //    if (runtimeName == "VerticalBranch")
    //    {
    //        var id = (int)message;
    //        Global.LastChosenOptionId = id;
    //        Step();
    //    }
    //    else if (runtimeName == "Breakout")
    //    {
    //        Global["BreakoutData"] = message as string;
    //    }
    //    else if (runtimeName == "Backlog")
    //    {
    //        var step = (int)message;
    //        if (step > 0)
    //        {
    //            LoadSnapshot(Global.LoadHistory(step));
    //        }
    //    }
    //}

    //public void LoadSnapshot(string snapshotPath)
    //{
    //    var snap = new Snapshot(snapshotPath);
    //    LoadSnapshot(snap);
    //}

    //public void DebugSnapshot()
    //{
    //    var snap = new Snapshot(this);
    //    snap.Save("res://snapshot");
    //}

    ///// <summary>
    ///// 运行脚本至下一个中断点
    ///// </summary>
    //public void Step()
    //{
    //    if (nonBlockingBuffer.IsRunning)
    //    {
    //        nonBlockingBuffer.Interrupt();
    //    }
    //    if (mainBuffer.IsRunning)
    //    {
    //        mainBuffer.Interrupt();
    //        return;
    //    }
    //    if (Global.PC < script.segments.Count)
    //    {
    //        // 脚本执行前是稳态，所有动画都已结束，在这里进行Snapshot
    //        Global.History.Add(new Snapshot(this));
    //        var @continue = false;
    //        do
    //        {
    //            @continue = script.segments[Global.PC].@continue;
    //            // 设置flag，防止执行中调用Save
    //            Global.IsExecuting = true;
    //            script.segments[Global.PC].Execute(this);
    //            Global.IsExecuting = false;
    //            Global.PC++;
    //        } while (@continue && Global.PC < script.segments.Count);
    //    }
    //    else
    //    {
    //        throw new IndexOutOfRangeException("Script Out of Bound!");
    //    }
    //}

    //public override void _Input(InputEvent @event)
    //{
    //    if (@event is InputEventKey)
    //    {
    //        if (Input.IsActionPressed("ui_accept"))
    //        {
    //            Step();
    //        }
    //        else if (Input.IsActionPressed("ui_cancel"))
    //        {
    //            Setting();
    //        }
    //        else if (Input.IsActionPressed("Save"))
    //        {
    //            if (nonBlockingBuffer.IsRunning)
    //            {
    //                nonBlockingBuffer.Interrupt();
    //            }
    //            if (mainBuffer.IsRunning)
    //            {
    //                mainBuffer.Interrupt();
    //            }
    //            DebugSnapshot();
    //        }
    //        else if (Input.IsActionPressed("ui_text_backspace"))
    //        {
    //            if (nonBlockingBuffer.IsRunning)
    //            {
    //                nonBlockingBuffer.Interrupt();
    //            }
    //            if (mainBuffer.IsRunning)
    //            {
    //                mainBuffer.Interrupt();
    //            }
    //            try
    //            {
    //                LoadSnapshot(Global.LoadHistory(1));
    //            }
    //            catch (ArgumentOutOfRangeException)
    //            {
    //                GD.Print("History is empty.");
    //            }
    //        }
    //        else if (Input.IsActionPressed("Load"))
    //        {
    //            var snap = new Snapshot("res://snapshot");
    //            LoadSnapshot(snap);
    //        }
    //    }
    //    else if (@event is InputEventMouseButton mouse)
    //    {
    //        if (mouse.ButtonIndex == MouseButton.WheelUp)
    //        {
    //            Backlog();
    //        }
    //    }
    //}

    //// 常见的传字符串消息情形，可以从脚本里调用
    //public void SwitchRuntime(string nextSubRuntimeName, string message)
    //{
    //    GetParent<Runtime>().SwitchRuntime(this, nextSubRuntimeName, message);
    //}

    //public void Office() => SwitchRuntime("Office", "");

    //public void Backlog()
    //{
    //    GetParent<Runtime>().SwitchRuntime(this, "Backlog", Global.History);
    //}

    //public void Setting() => SwitchRuntime("Setting", "AVG");

    //public void InitMiniGame(string name) => SwitchRuntime(name, Global["BreakoutData"]);

    //public void VerticalBranch(IEnumerable<string> options) => VerticalBranch(options.ToArray());

    //public void VerticalBranch(params string[] options)
    //{
    //    GetParent<Runtime>()
    //        .SwitchRuntime(
    //            this,
    //            "VerticalBranch",
    //            options.Select((option, index) => (index, option)).ToArray(),
    //            SwitchMode.Pause
    //        );
    //}

    //public void HorizontalBranch(IEnumerable<string> options) =>
    //    HorizontalBranch(options.ToArray());

    //public void HorizontalBranch(params string[] options)
    //{
    //    throw new NotImplementedException();
    //}

    //public void Goto(string label)
    //{
    //    // Execute结束后会PC++，所以这里要减1
    //    Global.PC = script.labels[label] - 1;
    //}
}
