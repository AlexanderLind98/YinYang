using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion;

public class MoveAndLookAt : IFiniteMotion, IResetMotion
{
    private readonly Vector3 endPos;
    private readonly Vector3 lookAtPos;
    private readonly float duration;

    private Vector3 startPos;
    private float elapsed;
    private bool started = false;

    public MoveAndLookAt(Vector3 endPos, Vector3 lookAtPos, float duration)
    {
        this.endPos     = endPos;
        this.lookAtPos  = lookAtPos;
        this.duration   = duration;
    }

    public void Apply(GameObject obj, float dt)
    {
        if (!started)
        {
            started  = true;
            elapsed  = 0f;
            startPos = obj.Transform.Position;
        }

        // 1) Move
        elapsed += dt;
        float t = Math.Clamp(elapsed / duration, 0f, 1f);
        obj.Transform.Position = Vector3.Lerp(startPos, endPos, t);

        // 2) Always look at lookAtPos
        Vector3 dir = lookAtPos - obj.Transform.Position;
        if (dir.LengthSquared > 0.0001f)
        {
            dir.Normalize();
            float yaw   = MathF.Atan2(dir.X, dir.Z);
            float pitch = MathF.Asin(dir.Y);
            obj.Transform.SetRotationInDegrees(
                MathHelper.RadiansToDegrees(pitch),
                MathHelper.RadiansToDegrees(yaw),
                0f
            );
        }
    }

    public bool IsDone => elapsed >= duration;

    public void Reset()
    {
        started = false;
        elapsed = 0f;
    }
}