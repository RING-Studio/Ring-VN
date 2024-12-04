namespace RingEngine.Core.Storage;

using EAL;
using Godot;
using RingEngine.Core;
using RingEngine.Core.General;
using RingEngine.EAL.SceneTree;

public class Snapshot
{
    public PackedScene GodotSceneTree;
    public string Global;

    // 历史记录可以在内存中暂存，但是不能保存到磁盘
    public Snapshot[] History = [];

    Snapshot() { }

    public Snapshot(VNRuntime runtime)
    {
        GodotSceneTree = SceneTreeProxy.Serialize();
        Global = runtime.Storage.Global.Serialize();
        History = runtime.Storage.Global.History.ToArray();
    }

    public Snapshot(PathSTD folder)
    {
        Load(folder);
    }

    public void Load(PathSTD folder)
    {
        GodotSceneTree = EAL.Resource.UniformLoader.Load<PackedScene>(
            folder + "GodotSceneTree.tscn"
        );
        Global = EAL.Resource.UniformLoader.LoadText(folder + "global.json");
    }

    public void Save(PathSTD folder)
    {
        EAL.Resource.UniformLoader.Save(GodotSceneTree, folder + "GodotSceneTree.tscn");
        EAL.Resource.UniformLoader.SaveText(Global, folder + "global.json");
    }
}
