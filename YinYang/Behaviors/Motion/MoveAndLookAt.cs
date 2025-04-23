using OpenTK.Mathematics;
using YinYang.Behaviors;
using YinYang.Components;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object to a target position while rotating to always look at a fixed point.
    /// </summary>
    public class MoveAndLookAt : IFiniteMotion, IResetMotion
    {
        private readonly Vector3 targetPosition;
        private readonly Vector3 lookAtTarget;
        private readonly float duration;

        private float elapsed = 0f;
        private Vector3 startPosition;
        private bool started = false;

        public MoveAndLookAt(Vector3 targetPosition, Vector3 lookAtTarget, float duration)
        {
            this.targetPosition = targetPosition;
            this.lookAtTarget = lookAtTarget;
            this.duration = duration;
        }

        public void Apply(GameObject obj, float deltaTime)
        {
            if (!started)
            {
                startPosition = obj.Transform.Position;
                started = true;
            }

            elapsed += deltaTime;
            float t = Math.Clamp(elapsed / duration, 0f, 1f);
            obj.Transform.Position = Vector3.Lerp(startPosition, targetPosition, t);

            // Calculate direction to target
            Vector3 direction = Vector3.Normalize(lookAtTarget - obj.Transform.Position);

            // Convert direction vector to pitch and yaw angles
            float pitch = MathHelper.RadiansToDegrees(MathF.Asin(direction.Y));
            float yaw = MathHelper.RadiansToDegrees(MathF.Atan2(direction.Z, direction.X));

            obj.SetRotationInDegrees(pitch, yaw, 0f);
        }

        public bool IsDone => elapsed >= duration;

        public void Reset()
        {
            elapsed = 0f;
            started = false;
        }
    }
}