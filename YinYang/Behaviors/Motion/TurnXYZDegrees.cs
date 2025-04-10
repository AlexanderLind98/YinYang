using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Smoothly rotates a GameObject from its current rotation
    /// to a specified absolute rotation (in degrees) over a set duration.
    /// </summary>
    public class TurnXYZDegrees : IFiniteMotion, IResetMotion
    {
        private readonly Vector3 targetRotationDeg;
        private readonly float duration;
        private float elapsed;
        private Vector3 startRotationDeg;
        private bool started = false;

        /// <summary>
        /// Constructs a motion step that rotates the object's Euler angles
        /// from its current orientation to a target (pitch, yaw, roll in degrees).
        /// </summary>
        /// <param name="targetRotationDeg">Vector3 describing final rotation in degrees.</param>
        /// <param name="duration">Seconds over which the rotation occurs.</param>
        public TurnXYZDegrees(Vector3 targetRotationDeg, float duration)
        {
            this.targetRotationDeg = targetRotationDeg;
            this.duration = duration;
        }

        /// <summary>
        /// On first call, captures the object's current rotation (in degrees).
        /// Each frame, linearly interpolates from that start rotation
        /// toward the target rotation over the given duration.
        /// </summary>
        /// <param name="obj">The GameObject being rotated.</param>
        /// <param name="deltaTime">Seconds elapsed since last frame.</param>
        public void Apply(GameObject obj, float deltaTime)
        {
            if (!started)
            {
                startRotationDeg = obj.Transform.GetRotationInDegrees();
                started = true;
            }

            elapsed += deltaTime;
            float t = Math.Clamp(elapsed / duration, 0f, 1f);

            // LERP each angle
            float rx = MathHelper.Lerp(startRotationDeg.X, targetRotationDeg.X, t);
            float ry = MathHelper.Lerp(startRotationDeg.Y, targetRotationDeg.Y, t);
            float rz = MathHelper.Lerp(startRotationDeg.Z, targetRotationDeg.Z, t);

            // Assign new rotation
            obj.Transform.SetRotationInDegrees(rx, ry, rz);
        }

        /// <summary>
        /// Indicates whether we've reached the target rotation timewise.
        /// </summary>
        public bool IsDone => elapsed >= duration;
        
        public void Reset()
        {
            elapsed = 0f;
            started = false;
        }
    }
}
