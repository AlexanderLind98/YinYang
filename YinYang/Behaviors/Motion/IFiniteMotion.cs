namespace YinYang.Behaviors.Motion;

public interface IFiniteMotion : IAutoMotion
{
    bool IsDone { get; }
}