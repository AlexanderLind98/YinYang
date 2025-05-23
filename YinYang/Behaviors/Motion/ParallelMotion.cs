namespace YinYang.Behaviors.Motion;

public class ParallelMotion : IFiniteMotion, IResetMotion
{
    private readonly List<IAutoMotion> motions;
    private bool isDone = false;

    public ParallelMotion(params IAutoMotion[] motions)
    {
        this.motions = motions.ToList();
    }

    public void Apply(GameObject obj, float deltaTime)
    {
        if (isDone)
            return;

        bool allDone = true;

        foreach (var motion in motions)
        {
            motion.Apply(obj, deltaTime);

            if (motion is IFiniteMotion step && !step.IsDone)
                allDone = false;
        }

        isDone = allDone;
    }

    public bool IsDone => isDone;
    
    public void Reset()
    {
        isDone = false;

        foreach (var motion in motions)
        {
            if (motion is IResetMotion resettable)
                resettable.Reset();
        }
    }
}