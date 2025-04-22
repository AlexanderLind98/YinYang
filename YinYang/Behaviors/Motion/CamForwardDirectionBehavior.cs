using OpenTK.Mathematics;
using YinYang;
using YinYang.Behaviors.Motion;

public class CamForwardDirectionBehavior : IFiniteMotion
{
    private readonly Vector3 direction;
    private readonly float duration;

    private float elapsed = 0f;
    private bool started = false;

    private Vector3 startDirection;
    private Vector3 targetDirection;

    public CamForwardDirectionBehavior(Vector3 direction, float duration)
    {
        this.direction = direction.Normalized();
        this.duration = duration;
    }

    public void Apply(GameObject obj, float deltaTime)
    {
        if (!started)
        {
            started = true;
            elapsed = 0f;

            // Get current forward direction from rotation
            var rot = obj.Transform.GetRotationInDegrees();
            float yaw = MathHelper.DegreesToRadians(rot.Y);
            float pitch = MathHelper.DegreesToRadians(rot.X);
            startDirection = new Vector3(
                MathF.Cos(pitch) * MathF.Cos(yaw),
                MathF.Sin(pitch),
                MathF.Cos(pitch) * MathF.Sin(yaw)
            ).Normalized();

            targetDirection = direction;
        }

        elapsed += deltaTime;
        float t = Math.Clamp(elapsed / duration, 0f, 1f);

        Vector3 dir = Vector3.Lerp(startDirection, targetDirection, t).Normalized();

        float yawNew = MathF.Atan2(dir.X, dir.Z);
        float pitchNew = MathF.Asin(dir.Y);

        obj.Transform.SetRotationInDegrees(
            MathHelper.RadiansToDegrees(pitchNew),
            MathHelper.RadiansToDegrees(yawNew),
            0f
        );
    }

    public bool IsDone => elapsed >= duration;
}
