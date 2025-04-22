using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion;

/// <summary>
/// Continuously orients the camera to face the given world‐space point.
/// Never “finishes” until you stop/reset it.
/// </summary>
public class CamContinuousTrackBehavior : IAutoMotion, IResetMotion
{
    private readonly Vector3 targetPosition;
    public CamContinuousTrackBehavior(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    // No duration—runs indefinitely
    public void Apply(GameObject obj, float deltaTime)
    {
        // Compute vector from camera to target
        var dir = targetPosition - obj.Transform.Position;
        if (dir.LengthSquared < 0.0001f) return;
        dir = dir.Normalized();

        // Yaw: left/right, Pitch: up/down
        float yaw   = MathF.Atan2(dir.X, dir.Z);
        float pitch = MathF.Asin(dir.Y);

        obj.Transform.SetRotationInDegrees(
            MathHelper.RadiansToDegrees(pitch),
            MathHelper.RadiansToDegrees(yaw),
            0f
        );
    }

    // We never “finish” automatically, so IsDone stays false
    public bool IsDone => false;

    public void Reset() { /* no state to reset */ }
}