namespace RingEngine.Core.Stage;

using System.Collections.Generic;
using System.Linq;
using Godot;
using RingEngine.Core.Animation;
using RingEngine.Core.General;
using static RingEngine.Core.General.AssertWrapper;

public class StageModule
{
    public Sprite2D Background;
    public Sprite2D OldBackground;
    public Characters Characters;
    public Sprite2D Mask;
    public Camera Camera;
    public EAL.Window Window;

    public StageModule()
    {
        Characters = new Characters();
        Background = Canvas.AddBG(
            UniformLoader.Load<Texture2D>("res://assets/Runtime/black.png"),
            Placement.BG
        );
    }

    public void Deserialize()
    {
        foreach (var character in Canvas.Characters)
        {
            Characters.Add(character.Name, character);
        }
        Background = Canvas.Backgrounds.ToArray()[0];
        Mask = Canvas.Masks.ToArray()[0];
    }
}

public class Characters
{
    Dictionary<string, Sprite2D> _Characters = [];

    /// <summary>
    /// 角色渲染的前后关系，靠后的角色显示在上层。
    /// </summary>
    List<string> RenderOrder = [];

    public Sprite2D this[string name] => _Characters[name];

    public bool Has(string name) => _Characters.ContainsKey(name);

    public void Rename(string name, string newName)
    {
        Assert(this.Has(name) && !this.Has(newName));
        var character = _Characters[name];
        character.Name = newName;
        _Characters.Remove(name);
        _Characters[newName] = character;
    }

    public void Add(string name, Sprite2D character)
    {
        Assert(!_Characters.ContainsKey(name));
        _Characters[name] = character;
        RenderOrder.Add(name);
    }

    public Sprite2D Remove(string name)
    {
        Assert(this.Has(name));
        var ret = _Characters[name];
        _Characters.Remove(name);
        RenderOrder.Remove(name);
        return ret;
    }
}

public class Camera { }
