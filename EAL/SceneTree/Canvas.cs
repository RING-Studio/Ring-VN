namespace RingEngine.EAL.SceneTree;

using Godot;
using RingEngine.Core.Animation;
using RingEngine.EAL.Resource;

/// <summary>
/// 封装了Stage需要的访问SceneTree的操作。
/// </summary>
public static class Canvas
{
    static uint BackgroundCounter = 0;

    public static Sprite AddBG(Texture2D texture, Placement placement)
    {
        var bg = new Sprite2D()
        {
            Name = $"BG{BackgroundCounter++}",
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
        };
        SceneTreeProxy.Backgrounds.AddChild(bg);
        return new Sprite(bg);
    }

    public static Sprite AddCharacter(string name, Texture2D texture, Placement placement)
    {
        var character = new Sprite2D()
        {
            Name = name,
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
        };
        SceneTreeProxy.Characters.AddChild(character);
        return new Sprite(character);
    }

    public static Sprite AddMask(string name, Texture2D texture, Placement placement)
    {
        var mask = new Sprite2D()
        {
            Name = name,
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
        };
        SceneTreeProxy.Masks.AddChild(mask);
        return new Sprite(mask);
    }
}
