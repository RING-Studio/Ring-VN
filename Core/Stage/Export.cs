namespace RingEngine.Core.Stage.Export;

using System.Collections.Generic;
using EAL.Resource;
using static RingEngine.Core.General.AssertWrapper;

public class StageModule
{
    public Sprite Background;
    public Sprite OldBackground;
    public Characters Characters;
    public Camera Camera;
    public Sprite Mask;
    public EAL.Window Window;
}

public class Characters
{
    Dictionary<string, Sprite> _Characters;

    /// <summary>
    /// 角色渲染的前后关系，靠后的角色显示在上层。
    /// </summary>
    List<string> RenderOrder;

    public Sprite this[string name] => _Characters[name];

    public bool Has(string name) => _Characters.ContainsKey(name);

    public void Rename(string name, string newName)
    {
        Assert(this.Has(name) && !this.Has(newName));
        var character = _Characters[name];
        character.Name = newName;
        _Characters.Remove(name);
        _Characters[newName] = character;
    }

    public void Add(string name, Sprite character)
    {
        Assert(!_Characters.ContainsKey(name));
        _Characters[name] = character;
        RenderOrder.Add(name);
    }

    public Sprite Remove(string name)
    {
        Assert(this.Has(name));
        var ret = _Characters[name];
        _Characters.Remove(name);
        RenderOrder.Remove(name);
        return ret;
    }
}

public class Camera { }
