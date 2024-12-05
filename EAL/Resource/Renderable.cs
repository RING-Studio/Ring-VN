namespace RingEngine.EAL.Resource;

using Godot;
using RingEngine.Core.Animation;
using RingEngine.EAL.Animation;

public interface IRenderable
{
    //TODO: 如果有了Node接口Name应该移过去
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Scale { get; set; }
    public float Alpha { get; set; }
    public Placement Placement
    {
        // 目前Scale只支持等比缩放，如果有需求则TODO
        get => new(Position, Scale.X);
        set
        {
            Position = value.Position;
            Scale = new(value.scale, value.scale);
        }
    }
}

public class Sprite : IRenderable, ITweenable
{
    public Sprite2D _Sprite;
    public virtual string Name
    {
        get => _Sprite.Name;
        set => _Sprite.Name = value;
    }
    public Vector2 Position
    {
        get => _Sprite.Position;
        set => _Sprite.Position = value;
    }
    public Vector2 Scale
    {
        get => _Sprite.Scale;
        set => _Sprite.Scale = value;
    }
    public float Alpha
    {
        get => _Sprite.Modulate.A;
        set
        {
            var modulate = _Sprite.Modulate;
            modulate.A = value;
            _Sprite.Modulate = modulate;
        }
    }
    public Texture Texture
    {
        get => new(_Sprite.Texture);
        set => _Sprite.Texture = value;
    }

    /// <summary>
    /// DO NOT USE in production code!!! Only for test purpose.
    /// </summary>
    public Sprite() { }
    public Sprite(Sprite2D sprite) => _Sprite = sprite;

    public static implicit operator Sprite2D(Sprite renderable) => renderable._Sprite;

    /// <summary>
    /// 析构时自动释放节点。
    /// </summary>
    public void Drop()
    {
        _Sprite.GetParent()?.RemoveChild(_Sprite);
        _Sprite.QueueFree();
    }
}

public class Widget : IRenderable, ITweenable
{
    public Control _Control;
    public string Name
    {
        get => _Control.Name;
        set => _Control.Name = value;
    }
    public Vector2 Position
    {
        get => _Control.Position;
        set => _Control.Position = value;
    }
    public Vector2 Scale
    {
        get => _Control.Scale;
        set => _Control.Scale = value;
    }
    public float Alpha
    {
        get => _Control.Modulate.A;
        set
        {
            var modulate = _Control.Modulate;
            modulate.A = value;
            _Control.Modulate = modulate;
        }
    }

    public Widget(Control control)
    {
        _Control = control;
    }

    public static implicit operator Control(Widget renderable) => renderable._Control;
}

public class TextBox : Widget
{
    public string Text
    {
        get => ((RichTextLabel)_Control).Text;
        set => ((RichTextLabel)_Control).Text = value;
    }
    public float VisibleRatio
    {
        get => ((RichTextLabel)_Control).VisibleRatio;
        set => ((RichTextLabel)_Control).VisibleRatio = value;
    }

    public TextBox(RichTextLabel control)
        : base(control) { }
}

public class Texture
{
    public Texture2D _Texture;

    public Texture(Texture2D texture) => _Texture = texture;

    public static implicit operator Texture2D(Texture texture) => texture._Texture;
}
