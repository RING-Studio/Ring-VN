namespace RingEngine.EAL.SceneTree;

using Godot;
using RingEngine.EAL.Resource;
using static RingEngine.Core.General.AssertWrapper;

/// <summary>
/// SceneTree代理类，场景树结构管理。初始化的时候需要实例化一次这个类来创建场景树结构。
/// </summary>
public class SceneTreeProxy
{
    static SceneTree SceneTree => Engine.GetMainLoop() as SceneTree;
    static Node Root => SceneTree.Root;

    public static RingIO RingIO => Root.GetNode<RingIO>("RingIO");

    const string RuntimeRootName = "VNRuntime";

    public static Node RuntimeRoot => Root.GetNode<Node>(RuntimeRootName);

    // Stage Part
    public static Node2D StageRoot => RuntimeRoot.GetNode<Node2D>("Stage");
    public static Node2D Backgrounds => StageRoot.GetNode<Node2D>("Backgrounds");
    public static Node2D Characters => StageRoot.GetNode<Node2D>("Characters");
    public static Node2D Masks => StageRoot.GetNode<Node2D>("Masks");

    // UI Part
    public static Control UIRoot => RuntimeRoot.GetNode<Control>("UI");
    public static TextureRect ChapterNameBack => UIRoot.GetNode<TextureRect>("./ChapterNameBack");
    public static RichTextLabel ChapterNameBox =>
        UIRoot.GetNode<RichTextLabel>("./ChapterNameBack/ChapterName");
    public static RichTextLabel TextBox =>
        UIRoot.GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer/TextBox");
    public static RichTextLabel CharacterNameBox =>
        UIRoot.GetNode<RichTextLabel>("./TextBoxBack/MarginContainer/MarginContainer2/TextBox");

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
        var runtimeRoot = (
            (PackedScene)UniformLoader.LoadScene("res://EAL/SceneTree/VNRuntime.tscn")
        ).Instantiate();
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

    public static GDScenePack Serialize()
    {
        SetOwner(RuntimeRoot, RuntimeRoot);
        var pack = new PackedScene();
        pack.Pack(RuntimeRoot);
        return new(pack);
    }

    public static void Deserialize(GDScenePack pack)
    {
        var runtime = ((PackedScene)pack).Instantiate();
        RuntimeRoot.QueueFree();
        Root.RemoveChild(RuntimeRoot);
        Root.AddChild(runtime);
        Assert(runtime.Name == RuntimeRootName);
    }
}
