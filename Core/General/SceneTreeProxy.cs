namespace RingEngine.Core.General;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RingEngine.Core.Audio;
using static RingEngine.Core.General.AssertWrapper;

/// <summary>
/// 代理类，用于访问一个Node下的Child Nodes。
/// </summary>
public partial class SubTree : IEnumerable<Node>
{
    protected Node Root;

    public SubTree(Node root)
    {
        Assert(root != null, "SubTree Root cannot be null.");
        Root = root;
    }

    public Node this[int index] => Root.GetChild(index);
    public Node this[string name]
    {
        get
        {
            Assert(Root.HasNode(name), $"{Root.Name} does not have child: {name}");
            return Root.GetNode(name);
        }
        set
        {
            if (Root.HasNode(name))
            {
                var old = Root.GetNode(name);
                old.QueueFree();
            }
            Root.AddChild(value);
            value.Name = name;
        }
    }

    public IEnumerator<Node> GetEnumerator() => Root.GetChildren().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

/// <summary>
/// 代理类，用于访问一个Node下的Child Nodes，所有Child Node均为类型<typeparamref name="T"/>。
/// </summary>
/// <typeparam name="T">子节点类型</typeparam>
public partial class SubTree<T> : SubTree, IEnumerable<T>
    where T : Node
{
    public SubTree(Node root)
        : base(root) { }

    public new T this[int index] => base[index] as T;
    public new T this[string name]
    {
        get => base[name] as T;
        set
        {
            Assert(value is T, $"Value is not of type {typeof(T).Name}");
            base[name] = value;
        }
    }

    public new IEnumerator<T> GetEnumerator() =>
        Root.GetChildren().Select(node => node as T).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

/// <summary>
/// SceneTree代理类，场景树结构管理。初始化的时候需要实例化一次这个类来创建场景树结构。
/// </summary>
public class SceneTreeProxy
{
    static SceneTree SceneTree => Engine.GetMainLoop() as SceneTree;
    static Node Root => SceneTree.Root;

    public static RingIO RingIO => Root.GetNode<RingIO>("RingIO");

    const string RuntimeRootName = "VNRuntime";
    const string RuntimeScenePath = "res://scenes/VNRuntime.tscn";

    static Node RuntimeRoot => Root.GetNode<Node>(RuntimeRootName);

    // Stage Part
    static Node2D StageRoot => RuntimeRoot.GetNode<Node2D>("Stage");
    public static SubTree<Sprite2D> Backgrounds => new(StageRoot.GetNode("Backgrounds"));
    public static SubTree<Sprite2D> Characters => new(StageRoot.GetNode("Characters"));
    public static SubTree<Sprite2D> Masks => new(StageRoot.GetNode("Masks"));

    // UI Part
    public static Control UIRoot => RuntimeRoot.GetNode<Control>("UI");
    public static Control ThemeRoot => UIRoot.GetNode<Control>("Theme");

    // Branch Part
    const string BranchPath = "res://scenes/Branch.tscn";
    public static PackedScene BranchScene => UniformLoader.Load<PackedScene>(BranchPath);

    // Audio Part
    // 如果方法继续增加需要创建单独的Audio类来做转换，现在就直接这样了。
    public static AudioPlayer BGM => new(RuntimeRoot.GetNode<AudioStreamPlayer>("BGM"));
    public static AudioPlayer SE => new(RuntimeRoot.GetNode<AudioStreamPlayer>("SE"));
    public static AudioPlayer Voice => new(RuntimeRoot.GetNode<AudioStreamPlayer>("Voice"));

    public SceneTreeProxy()
    {
        if (Root.HasNode(RuntimeRootName))
        {
            return;
        }
        var runtimeRoot = UniformLoader.Load<PackedScene>(RuntimeScenePath).Instantiate();
        runtimeRoot.Name = RuntimeRootName;

        Root.AddChild(runtimeRoot);
    }

    internal static void SetOwner(Node owner, Node root)
    {
        if (owner != root)
        {
            root.Owner = owner;
        }
        foreach (var child in root.GetChildren())
        {
            SetOwner(owner, child);
        }
    }

    public static PackedScene Serialize()
    {
        SetOwner(RuntimeRoot, RuntimeRoot);
        var pack = new PackedScene();
        pack.Pack(RuntimeRoot);
        return pack;
    }

    public static void Deserialize(PackedScene pack)
    {
        var runtime = pack.Instantiate();
        RuntimeRoot.QueueFree();
        Root.RemoveChild(RuntimeRoot);
        Root.AddChild(runtime);
        Assert(runtime.Name == RuntimeRootName);
    }
}
