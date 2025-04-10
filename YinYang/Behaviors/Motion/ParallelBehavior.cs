using OpenTK.Windowing.Common;

namespace YinYang.Behaviors.Motion;

/// <summary>
/// A Behavior that applies multiple IAutoMotion steps in parallel,
/// so they all run at the same time.
/// </summary>
public class ParallelBehavior : Behaviour
{
    // List of motions to run in parallel
    private readonly List<IAutoMotion> motions;

    /// <summary>
    /// Creates a behavior that runs multiple motion steps simultaneously.
    /// </summary>
    /// <param name="obj">The GameObject to attach to (passed automatically).</param>
    /// <param name="window">The Game reference (passed automatically).</param>
    /// <param name="motions">One or more atomic motions to run in parallel.</param>
    public ParallelBehavior(GameObject obj, Game window, params IAutoMotion[] motions)
        : base(obj, window)
    {
        this.motions = motions.ToList();
    }

    /// <summary>
    /// Called every frame; applies all motions in parallel.
    /// </summary>
    public override void Update(FrameEventArgs args)
    {
        float deltaTime = (float)args.Time;
        foreach (var motion in motions)
        {
            motion.Apply(gameObject, deltaTime);
        }
    }
}