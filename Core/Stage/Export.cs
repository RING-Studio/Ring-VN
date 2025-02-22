namespace RingEngine.Core.Stage;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;
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

public class OldCharactersProxy
{
    const string suffix = "_old";
    public Characters Characters;

    public OldCharactersProxy(Characters characters)
    {
        Characters = characters;
    }

    public Sprite2D this[string name] => Characters[name + suffix];

    public bool Has(string name) => Characters.Has(name + suffix);

    public Sprite2D Remove(string name) => Characters.Remove(name + suffix);
}

public class Characters : IEnumerable<Sprite2D>
{
    Dictionary<string, Sprite2D> _Characters = [];

    public Sprite2D this[string name] => _Characters[name];

    /// <summary>
    /// 用于访问标记为old的角色。
    /// </summary>
    public OldCharactersProxy Old => new(this);

    public bool Has(string name) => _Characters.ContainsKey(name);

    public void Rename(string name, string newName)
    {
        Assert(this.Has(name) && !this.Has(newName));
        var character = _Characters[name];
        character.Name = newName;
        _Characters.Remove(name);
        _Characters[newName] = character;
    }

    /// <summary>
    /// 将角色标记为old，角色名会添加后缀"_old"。
    /// </summary>
    public void MarkAsOld(string name)
    {
        Rename(name, name + "_old");
    }

    public void Add(string name, Sprite2D character)
    {
        Assert(!_Characters.ContainsKey(name));
        _Characters[name] = character;
    }

    public Sprite2D Remove(string name)
    {
        Assert(this.Has(name));
        var ret = _Characters[name];
        _Characters.Remove(name);
        return ret;
    }

    public IEnumerator<Sprite2D> GetEnumerator()
    {
        foreach (var (_, character) in _Characters)
        {
            yield return character;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

public class Camera { }
