namespace RingEngine.Core.Storage;

using Godot;
using RingEngine.Core;
using RingEngine.Core.General;

public class StorageModule
{
    // 全局变量
    public StorageEngine Global { get; private set; }

    public StorageModule()
    {
        Global = new StorageEngine();
    }

    public string Serialize()
    {
        return Global.Serialize();
    }

    public void Deserialize(string json)
    {
        Global = StorageEngine.Deserialize(json);
    }
}

public class Snapshot
{
    public PackedScene GodotSceneTree;
    public string ScriptPath;
    public string Global;

    // 历史记录可以在内存中暂存，但是不能保存到磁盘
    public Snapshot[] History = [];

    Snapshot() { }

    public Snapshot(VNRuntime runtime)
    {
        GodotSceneTree = SceneTreeProxy.Serialize();
        ScriptPath = runtime.Script.Serialize();
        Global = runtime.Storage.Serialize();
        History = runtime.Storage.Global.History.ToArray();
    }

    public Snapshot(PathSTD folder) => Load(folder);

    public void Load(PathSTD folder)
    {
        GodotSceneTree = UniformLoader.Load<PackedScene>(folder + "GodotSceneTree.tscn");
        ScriptPath = UniformLoader.LoadText(folder + "scriptPath.txt");
        Global = UniformLoader.LoadText(folder + "global.json");
    }

    public void Save(PathSTD folder)
    {
        UniformLoader.Save(GodotSceneTree, folder + "GodotSceneTree.tscn");
        UniformLoader.SaveText(ScriptPath, folder + "scriptPath.txt");
        UniformLoader.SaveText(Global, folder + "global.json");
    }
}
