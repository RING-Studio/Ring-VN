namespace RingEngine.Core.Animation;

using System.Collections.Generic;
using System.Linq;

public class AnimationModule
{
    // 动画效果缓冲区
    public EffectBuffer mainBuffer;
    public EffectBuffer nonBlockingBuffer;

    /// <summary>
    /// 临时效果缓冲区
    /// </summary>
    public List<EffectBuffer> TempBuffers;

    public void AddTempEffect(IEffect effect)
    {
        var buffer = new EffectBuffer();
        buffer.Append(new EffectGroupBuilder().Add(effect).Build());
        TempBuffers.Add(buffer);
    }

    public bool HasPendingAnimation =>
        mainBuffer.HasPendingAnimation
        || nonBlockingBuffer.HasPendingAnimation
        || TempBuffers.Select((buffer) => buffer.HasPendingAnimation).Any();

    public AnimationModule()
    {
        mainBuffer = new();
        nonBlockingBuffer = new();
        TempBuffers = [];
    }

    public void Flush()
    {
        if (mainBuffer.IsRunning)
        {
            mainBuffer.Interrupt();
        }
        if (nonBlockingBuffer.IsRunning)
        {
            nonBlockingBuffer.Interrupt();
        }
        foreach (var buffer in TempBuffers)
        {
            if (buffer.IsRunning)
            {
                buffer.Interrupt();
            }
        }
    }

    public void Execute()
    {
        mainBuffer.Execute();
        nonBlockingBuffer.Execute();
        foreach (var buffer in TempBuffers)
        {
            buffer.Execute();
        }
        var finished = TempBuffers.Where((buffer) => !buffer.IsRunning).ToList();
        foreach (var buffer in finished)
        {
            TempBuffers.Remove(buffer);
        }
    }
}
