namespace RingEngine.Core.Stage;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using RingEngine.Core.General;
using static RingEngine.Core.General.AssertWrapper;

/// <summary>
/// 封装了Stage需要的访问SceneTree的操作。
/// </summary>
public static class Canvas
{
    public enum ImageType
    {
        BG,
        Character,
        Mask,
    }

    static uint BackgroundCounter = 0;

    static Dictionary<Sprite2D, ImageType> DetachedNodes = [];

    public static SubTree<Sprite2D> Backgrounds => SceneTreeProxy.Backgrounds;
    public static SubTree<Sprite2D> Characters => SceneTreeProxy.Characters;
    public static SubTree<Sprite2D> Masks => SceneTreeProxy.Masks;

    public static void Attach(Sprite2D node)
    {
        Assert(DetachedNodes.ContainsKey(node));
        switch (DetachedNodes[node])
        {
            case ImageType.BG:
                Backgrounds.AddChild(node);
                break;
            case ImageType.Character:
                Characters.AddChild(node);
                break;
            case ImageType.Mask:
                Masks.AddChild(node);
                break;
            default:
                Unreachable();
                break;
        }
    }

    public static Sprite2D AddBG(Texture2D texture, Placement placement, bool detached = false)
    {
        var bg = new Sprite2D()
        {
            Name = $"BG{BackgroundCounter++}",
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
            Centered = false,
        };
        if (!detached)
        {
            Backgrounds.AddChild(bg);
        }
        else
        {
            DetachedNodes.Add(bg, ImageType.BG);
        }
        return bg;
    }

    public static Sprite2D AddCharacter(
        string name,
        Texture2D texture,
        Placement placement,
        bool detached = false
    )
    {
        var character = new Sprite2D()
        {
            Name = name,
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
            Centered = false,
        };
        if (!detached)
        {
            Characters.AddChild(character);
        }
        else
        {
            DetachedNodes.Add(character, ImageType.Character);
        }

        return character;
    }

    public static Sprite2D AddMask(
        string name,
        Texture2D texture,
        Placement placement,
        bool detached = false
    )
    {
        var mask = new Sprite2D()
        {
            Name = name,
            Texture = texture,
            Position = placement.Position,
            Scale = new Vector2(placement.scale, placement.scale),
            Centered = false,
        };
        if (!detached)
        {
            Masks.AddChild(mask);
        }
        else
        {
            DetachedNodes.Add(mask, ImageType.Mask);
        }

        return mask;
    }
}
