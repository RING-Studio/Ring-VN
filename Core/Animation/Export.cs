namespace RingEngine.Core.Animation;

public class AnimationModule
{
    // 动画效果缓冲区
    public EffectBuffer mainBuffer;
    public EffectBuffer nonBlockingBuffer;

    public bool HasPendingAnimation =>
        mainBuffer.HasPendingAnimation || nonBlockingBuffer.HasPendingAnimation;

    public AnimationModule()
    {
        mainBuffer = new();
        nonBlockingBuffer = new();
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
    }

    public void Execute()
    {
        mainBuffer.Execute();
        nonBlockingBuffer.Execute();
    }
}
