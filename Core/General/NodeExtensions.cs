namespace RingEngine.Core.General;

using System;
using System.Collections.Generic;
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

public class PlacementProxy : IEquatable<PlacementProxy>
{
    Node2D _Node;

    public PlacementProxy(Node2D node) => _Node = node;

    public static implicit operator Placement(PlacementProxy proxy) =>
        new(proxy._Node.Position, proxy._Node.Scale.X);

    public void Set(Placement value)
    {
        _Node.Position = value.Position;
        _Node.Scale = new(value.scale, value.scale);
    }

    public static bool operator ==(PlacementProxy left, PlacementProxy right) =>
        EqualityComparer<PlacementProxy>.Default.Equals(left, right);

    public static bool operator !=(PlacementProxy left, PlacementProxy right) => !(left == right);

    public override bool Equals(object obj) => this.Equals(obj as PlacementProxy);

    public bool Equals(PlacementProxy other) =>
        other is not null && EqualityComparer<Placement>.Default.Equals(this, other);

    public override int GetHashCode() => HashCode.Combine((Placement)this);
}

public class ControlPlacementProxy : IEquatable<ControlPlacementProxy>
{
    Control _Node;

    public ControlPlacementProxy(Control node) => _Node = node;

    public static implicit operator Placement(ControlPlacementProxy proxy) =>
        new(proxy._Node.Position, proxy._Node.Scale.X);

    public void Set(Placement value)
    {
        _Node.Position = value.Position;
        _Node.Scale = new(value.scale, value.scale);
    }

    public static bool operator ==(ControlPlacementProxy left, ControlPlacementProxy right) => EqualityComparer<ControlPlacementProxy>.Default.Equals(left, right);
    public static bool operator !=(ControlPlacementProxy left, ControlPlacementProxy right) => !(left == right);

    public override bool Equals(object obj) => this.Equals(obj as ControlPlacementProxy);
    public bool Equals(ControlPlacementProxy other) => other is not null && EqualityComparer<Placement>.Default.Equals(this, other);
    public override int GetHashCode() => HashCode.Combine((Placement)this);
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
    public static ControlPlacementProxy Placement(this Control node) => new(node);
}
