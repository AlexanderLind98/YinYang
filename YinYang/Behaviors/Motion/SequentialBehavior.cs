using OpenTK.Windowing.Common;

namespace YinYang.Behaviors.Motion;

/// <summary>
/// A behavior that executes a list of IAutoMotion steps in sequence,
/// one at a time. Each step runs until it signals completion via IsDone.
/// </summary>
public class SequentialBehavior : Behaviour, IResetMotion
{
    // Ordered list of motion steps
    private readonly List<IAutoMotion> motions;

    // Index of the currently active step
    private int currentStepIndex = 0;

    /// <summary>
    /// Creates a behavior that executes multiple motion steps sequentially.
    /// </summary>
    /// <param name="obj">The GameObject this behavior is attached to.</param>
    /// <param name="window">The game instance (provided automatically).</param>
    /// <param name="motions">Motion steps to run in sequence.</param>
    public SequentialBehavior(GameObject obj, Game window, params IAutoMotion[] motions)
        : base(obj, window)
    {
        this.motions = motions.ToList();
    }

    /// <summary>
    /// Updates the current step. If it's complete, moves to the next one.
    /// </summary>
    /// <param name="args">Timing information from the frame update.</param>
    public override void Update(FrameEventArgs args)
    {
        if (currentStepIndex >= motions.Count)
            return; // All steps are done

        var current = motions[currentStepIndex];
        current.Apply(gameObject, (float)args.Time);

        // Check if the current step is done, and advance if so
        if (current is IFiniteMotion step && step.IsDone)
        {
            currentStepIndex++;
        }
    }
    
    public void Reset()
    {
        currentStepIndex = 0;

        foreach (var motion in motions)
        {
            if (motion is IResetMotion resettable)
                resettable.Reset();
        }
    }

}