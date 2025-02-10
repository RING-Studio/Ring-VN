namespace RingEngine.Core.Stage;

using System.Collections.Generic;
using System.Linq;
using Godot;
using RingEngine.Core.General;

/// <summary>
/// 封装了Stage需要的访问SceneTree的操作。
/// </summary>
public static class Canvas
{
    static uint BackgroundCounter = 0;

    public static IEnumerable<Sprite2D> Backgrounds =>
        SceneTreeProxy.Backgrounds.GetChildren().Select(x => x as Sprite2D);
    public static IEnumerable<Sprite2D> Characters =>
        SceneTreeProxy.Characters.GetChildren().Select(x => x as Sprite2D);
    public static IEnumerable<Sprite2D> Masks =>
        SceneTreeProxy.Masks.GetChildren().Select(x => x as Sprite2D);

    public static Sprite2D AddBG(Texture2D texture, Placement placement)
    {
        var bg = new Sprite2D()
        {
            Name = $"BG{BackgroundCounter++}",
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
            Centered = false,
        };
        SceneTreeProxy.Backgrounds.AddChild(bg);
        return bg;
    }

    public static Sprite2D AddCharacter(string name, Texture2D texture, Placement placement)
    {
        var character = new Sprite2D()
        {
            Name = name,
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
            Centered = false,
        };
        SceneTreeProxy.Characters.AddChild(character);
        return character;
    }

    public static Sprite2D AddMask(string name, Texture2D texture, Placement placement)
    {
        var mask = new Sprite2D()
        {
            Name = name,
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
            Centered = false
        };
        SceneTreeProxy.Masks.AddChild(mask);
        return mask;
    }
}
