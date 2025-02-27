namespace RingEngine.Core.Animation;

using System.Threading.Tasks;
using Godot;
using RingEngine.Core;
using RingEngine.Core.General;
using RingEngine.Core.Stage;

/// <summary>
/// 需要全局信息的效果
/// </summary>
public interface ITransition
{
    /// <summary>
    /// 应用当前效果
    /// </summary>
    public Task Run(VNRuntime runtime);
}

/// <summary>
/// <para>背景转场效果</para>
/// 执行时，舞台上只有背景，无角色立绘，无UI。以当前背景为旧背景。
/// </summary>
public abstract class BGTransition : ITransition
{
    public Texture2D NewBG;

    /// <summary>
    /// 设置要替换的新背景
    /// </summary>
    public BGTransition SetNewBG(Texture2D newBG)
    {
        NewBG = newBG;
        return this;
    }

    public abstract Task Run(VNRuntime runtime);
}

public class DissolveTrans : BGTransition
{
    public Texture2D mask;
    public double duration;

    public DissolveTrans(double duration = 2)
    {
        this.mask = UniformLoader.Load<Texture2D>("res://assets/Runtime/black.png");
        this.duration = duration;
    }

    public override async Task Run(VNRuntime runtime)
    {
        var stage = runtime.Stage;

        stage.Mask?.Drop();
        stage.Mask = Canvas.AddMask("Mask", mask, Placement.BG);
        stage.Mask.Alpha().Set(0);

        await stage.Mask.Apply(OpacityEffect.Dissolve(duration / 2));
        stage.Background.Texture = NewBG;
        await stage.Mask.Apply(OpacityEffect.Fade(duration / 2));

        stage.Mask.Drop();
        stage.Mask = null;
    }
}

public class ImageTrans : BGTransition
{
    public Texture2D Mask;
    public Texture2D Control;
    public double Duration;
    public bool Reversed;
    public double Smooth;

    public ImageTrans(double duration = 2, bool reversed = false, double smooth = 0.2)
    {
        Mask = UniformLoader.Load<Texture2D>("res://assets/Runtime/black.png");
        Control = UniformLoader.Load<Texture2D>("res://assets/Runtime/rule_10.png");
        Duration = duration;
        Reversed = reversed;
        Smooth = smooth;
    }

    public ImageTrans(
        PathSTD controlPath,
        double duration = 2,
        bool reversed = false,
        double smooth = 0.2
    )
    {
        Mask = UniformLoader.Load<Texture2D>("res://assets/Runtime/black.png");
        Control = UniformLoader.Load<Texture2D>(controlPath);
        Duration = duration;
        Reversed = reversed;
        Smooth = smooth;
    }

    public override async Task Run(VNRuntime runtime)
    {
        var stage = runtime.Stage;

        // 初始化Mask
        stage.Mask?.Drop();
        stage.Mask = Canvas.AddMask("Mask", Mask, Placement.BG);

        // 初始化Shader
        var material = new ShaderMaterial
        {
            Shader = UniformLoader.Load<Shader>("res://Shaders/mask.gdshader"),
        };
        material.SetShaderParameter("progress", 0.0);
        material.SetShaderParameter("smooth_size", Smooth);
        material.SetShaderParameter("control", Control);
        material.SetShaderParameter("reversed", Reversed);
        stage.Mask.Material = material;

        // Mask On
        await MethodInterpolation.New(
            progress =>
                ((ShaderMaterial)stage.Mask.Material).SetShaderParameter("progress", progress),
            0.0,
            1.0,
            Duration / 2
        ).Apply();

        stage.Background.Texture = NewBG;

        // Mask Off
        await MethodInterpolation.New(
            progress =>
                ((ShaderMaterial)stage.Mask.Material).SetShaderParameter("progress", progress),
            1.0,
            0.0,
            Duration / 2
        ).Apply();

        stage.Mask.Drop();
        stage.Mask = null;
    }
}
