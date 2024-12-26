namespace RingEngine.Core.General;

using Godot;
using RingEngine.Core.Animation;

public class AlphaProxy
{
    CanvasItem _Node;

    public static implicit operator float(AlphaProxy proxy) => proxy._Node.Modulate.A;

    public void Set(float value)
    {
        var modulate = _Node.Modulate;
        modulate.A = value;
        _Node.Modulate = modulate;
    }

    public AlphaProxy(CanvasItem node) => _Node = node;
}

public class PlacementProxy
{
    Node2D _Node;

    public static implicit operator Placement(PlacementProxy proxy) =>
        new(proxy._Node.Position, proxy._Node.Scale.X);

    public void Set(Placement value)
    {
        _Node.Position = value.Position;
        _Node.Scale = new(value.scale, value.scale);
    }

    public PlacementProxy(Node2D node) => _Node = node;
}

public static class NodeExtensions
{
    /// <summary>
    /// 析构时自动释放节点。
    /// </summary>
    public static void Drop(this Sprite2D node)
    {
        node.GetParent()?.RemoveChild(node);
        node.QueueFree();
    }
}

public static class CanvasItemExtensions
{
    public static AlphaProxy Alpha(this CanvasItem node) => new(node);
}

public static class Node2DExtensions
{
    public static PlacementProxy Placement(this Node2D node) => new(node);
}

public static class ControlExtensions
{
    public static PlacementProxy Placement(this Node2D node) => new(node);
}
