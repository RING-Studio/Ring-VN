namespace RingEngine.Core.Animation;

using System;
using System.Collections.Generic;
using Godot;

public class Placement : IEquatable<Placement>
{
    static float eps = 0.0001f;
    public float x;
    public float y;
    public float scale;

    public Vector2 Position => new(x, y);

    public Placement(double x, double y, double scale)
    {
        this.x = (float)x;
        this.y = (float)y;
        this.scale = (float)scale;
    }

    public Placement(Vector2 position, double scale)
    {
        this.x = position.X;
        this.y = position.Y;
        this.scale = (float)scale;
    }

    public static Placement BG => new(0.0, 0.0, 1.0);

    public override bool Equals(object obj) => this.Equals(obj as Placement);

    public bool Equals(Placement other) =>
        other is not null && Math.Abs(this.x - other.x) < eps
        && Math.Abs(this.y - other.y) < eps
        && Math.Abs(this.scale - other.scale) < eps;

    public override int GetHashCode() => HashCode.Combine(this.x, this.y, this.scale);

    public static bool operator ==(Placement left, Placement right) =>
        EqualityComparer<Placement>.Default.Equals(left, right);

    public static bool operator !=(Placement left, Placement right) => !(left == right);
}
