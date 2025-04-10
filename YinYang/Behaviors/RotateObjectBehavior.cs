using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace YinYang.Behaviors;

/// <summary>
/// Rotates a GameObject continuously around a specified axis with a given speed.
/// </summary>
public class RotateObjectBehavior : Behaviour
{
    private readonly Vector3 _rotationAxis;
    private readonly float _rotationSpeed;

    /// <summary>
    /// Creates a rotation behavior for the GameObject.
    /// </summary>
    /// <param name="gameObject">The GameObject to rotate.</param>
    /// <param name="window">The Game instance.</param>
    /// <param name="rotationAxis">The axis of rotation (e.g., Vector3.UnitY).</param>
    /// <param name="rotationSpeed">Speed in degrees per second. Use negative values for reverse direction.</param>
    public RotateObjectBehavior(GameObject gameObject, Game window, Vector3 rotationAxis, float rotationSpeed = 90f)
        : base(gameObject, window)
    {
        _rotationAxis = rotationAxis.Normalized(); // Always normalize to avoid scaling issues
        _rotationSpeed = rotationSpeed / 10;
    }

    public override void Update(FrameEventArgs args)
    {
        var rotationDegrees = gameObject.Transform.GetRotationInDegrees();

        rotationDegrees += _rotationAxis * _rotationSpeed * (float)args.Time;

        gameObject.SetRotationInDegrees(rotationDegrees.X, rotationDegrees.Y, rotationDegrees.Z);
    }
}