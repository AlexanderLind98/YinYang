namespace YinYang.Behaviors.Motion;

/// <summary>
/// Atomic motion interface for applying motion to GameObjects.
/// </summary>
public interface IAutoMotion
{
    void Apply(GameObject obj, float time);
}
