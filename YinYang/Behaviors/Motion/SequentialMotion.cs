namespace YinYang.Behaviors.Motion;

/// <summary>
/// Runs a series of IAutoMotion steps one after the other.
/// </summary>
public class SequentialMotion : IFiniteMotion, IResetMotion
{
    private readonly List<IAutoMotion> motions;
    private int currentStep = 0;

    /// <summary>
    /// Creates a new sequence of motion steps to run one after another.
    /// </summary>
    /// <param name="motions">The ordered steps to execute.</param>
    public SequentialMotion(params IAutoMotion[] motions)
    {
        this.motions = motions.ToList();
    }

    /// <summary>
    /// Applies the current step. Advances when the step is complete.
    /// </summary>
    public void Apply(GameObject obj, float deltaTime)
    {
        if (IsDone) return;

        var current = motions[currentStep];
        current.Apply(obj, deltaTime);

        if (current is IFiniteMotion fm && fm.IsDone)
        {
            currentStep++;
        }
    }

    /// <summary>
    /// Returns true if all steps in the sequence have completed.
    /// </summary>
    public bool IsDone => currentStep >= motions.Count;

    /// <summary>
    /// Resets the entire sequence, allowing it to be run again from the beginning.
    /// </summary>
    public void Reset()
    {
        currentStep = 0;

        foreach (var m in motions)
        {
            if (m is IResetMotion resettable)
                resettable.Reset();
        }
    }
}