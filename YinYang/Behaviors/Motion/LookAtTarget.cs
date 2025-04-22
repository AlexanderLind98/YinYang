using OpenTK.Mathematics;
using YinYang.Behaviors.Motion;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Makes an object rotate so it looks at a fixed point in world space.
    /// </summary>
    public class LookAtTarget : IAutoMotion
    {
        private Vector3 targetPosition;

        /// <summary>
        /// Create a new look-at motion.
        /// </summary>
        /// <param name="targetPosition">The point to look at.</param>
        public LookAtTarget(Vector3 targetPosition)
        {
            this.targetPosition = targetPosition;
        }

        public void Apply(GameObject obj, float time)
        {
            // Find direction to the target
            Vector3 direction = targetPosition - obj.Transform.Position;

            // Ignore if the target is too close
            if (direction.Length < 0.001f)
                return;

            direction.Normalize();

            // Calculate yaw (left/right) and pitch (up/down)
            float yaw = MathF.Atan2(direction.X, direction.Z);
            float pitch = MathF.Asin(direction.Y);

            // Convert to degrees
            float yawDeg = MathHelper.RadiansToDegrees(yaw);
            float pitchDeg = MathHelper.RadiansToDegrees(pitch);

            // Set rotation (roll is always 0)
            obj.SetRotationInDegrees(pitchDeg, yawDeg, 0f);
        }
    }
}