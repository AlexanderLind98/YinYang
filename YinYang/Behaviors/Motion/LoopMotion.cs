namespace YinYang.Behaviors.Motion;

/// <summary>
/// Repeats a step (or a group) either infinitely or a fixed number of times.
/// </summary>
public class LoopMotion : IFiniteMotion
{
    private readonly IAutoMotion motion;
    private readonly int? maxLoops;

    private int currentLoop = 0;
    private bool isDone = false;

    /// <summary>
    /// Creates a looping motion that repeats forever.
    /// </summary>
    /// <param name="motion">The motion step or group to repeat.</param>
    public LoopMotion(IAutoMotion motion)
    {
        this.motion = motion;
        this.maxLoops = null; 
    }

    /// <summary>
    /// Creates a looping motion that repeats a specific number of times.
    /// </summary>
    /// <param name="motion">The motion step or group to repeat.</param>
    /// <param name="loopCount">How many times to repeat the motion.</param>
    public LoopMotion(IAutoMotion motion, int loopCount)
    {
        this.motion = motion;
        this.maxLoops = loopCount;
    }

    public void Apply(GameObject obj, float deltaTime)
    {
        if (isDone)
            return;

        motion.Apply(obj, deltaTime);

        if (motion is IFiniteMotion step && step.IsDone)
        {
            currentLoop++;

            // Reset the step (if it supports it)
            if (motion is IResetMotion resettable)
                resettable.Reset();

            // Stop if we've looped enough
            if (maxLoops.HasValue && currentLoop >= maxLoops.Value)
                isDone = true;
        }
    }

    public bool IsDone => maxLoops.HasValue && isDone;
}