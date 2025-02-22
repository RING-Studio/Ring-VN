namespace RingEngine.EAL.Old;

using System.Collections.Generic;
using Godot;
using RingEngine.Core.General;

public partial class Canvas : Node2D
{
    Dictionary<string, Sprite2D> childs = [];
    public Sprite2D Mask;
    public Sprite2D BG;
    public Sprite2D ZombieBG;

    public Sprite2D this[string name] => childs[name];

    public override void _Ready()
    {
        if (GetChildCount() > 0)
        {
            // 存档恢复
            foreach (var child in GetChildren())
            {
                switch (child.Name)
                {
                    case "BG":
                        BG = child as Sprite2D;
                        break;
                    case "Mask":
                        Mask = child as Sprite2D;
                        break;
                    default:
                        childs[child.Name] = child as Sprite2D;
                        break;
                }
            }
            return;
        }
        // 占位BG
        BG = new Sprite2D
        {
            Name = "BG",
            Texture = GD.Load<Texture2D>("res://assets/Runtime/black.png"),
            ZIndex = -1,
            Centered = false,
            Position = Placement.BG.Position,
            Scale = new Vector2(Placement.BG.scale, Placement.BG.scale),
        };
        AddChild(BG);
        BG.Owner = this;
        Mask = new Sprite2D
        {
            Name = "Mask",
            Texture = null,
            ZIndex = 1,
            Centered = false,
        };
        AddChild(Mask);
        Mask.Owner = this;
    }

    public PackedScene Serialize()
    {
        var scene = new PackedScene();
        scene.Pack(this);
        return scene;
    }
}
