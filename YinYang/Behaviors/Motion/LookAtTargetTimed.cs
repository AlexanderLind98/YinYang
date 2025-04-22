using OpenTK.Mathematics;
using YinYang;
using YinYang.Behaviors.Motion;

public class LookAtTargetTimed : IFiniteMotion, IResetMotion
{
    private readonly Vector3 targetPos;
    private readonly float duration;

    private float elapsed = 0f;
    private Vector3 startRot;
    private Vector3 targetRot;
    private bool started = false;

    public LookAtTargetTimed(Vector3 targetPos, float duration)
    {
        this.targetPos = targetPos;
        this.duration = duration;
    }

    public void Apply(GameObject obj, float deltaTime)
    {
        if (!started)
        {
            started = true;
            elapsed = 0f;

            // Rotation when we begin
            startRot = obj.Transform.GetRotationInDegrees();

            // Compute target direction and angles
            Vector3 dir = targetPos - obj.Transform.Position;
            if (dir.Length < 0.001f) dir = new Vector3(0, 0, -1);
            dir.Normalize();

            float yaw = MathF.Atan2(dir.X, dir.Z);
            float pitch = MathF.Asin(dir.Y);
            targetRot = new Vector3(
                MathHelper.RadiansToDegrees(pitch),
                MathHelper.RadiansToDegrees(yaw),
                0
            );
        }

        elapsed += deltaTime;
        float t = Math.Clamp(elapsed / duration, 0f, 1f);

        Vector3 lerped = Vector3.Lerp(startRot, targetRot, t);
        obj.Transform.SetRotationInDegrees(lerped.X, lerped.Y, lerped.Z);
    }

    public bool IsDone => elapsed >= duration;

    public void Reset()
    {
        elapsed = 0f;
        started = false;
    }
}