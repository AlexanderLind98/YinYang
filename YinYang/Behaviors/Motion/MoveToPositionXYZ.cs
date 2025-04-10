using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Smoothly moves a GameObject from its start position
    /// to a specified target position (XYZ) over a set duration.
    /// </summary>
    public class MoveToPositionXYZ : IFiniteMotion, IResetMotion
    {
        private readonly Vector3 targetPosition;
        private readonly float duration;
        private float elapsed;
        private Vector3 startPosition;
        private bool started = false;

        /// <summary>
        /// Constructs a motion step that linearly interpolates
        /// the object's position from its current location to a new target.
        /// </summary>
        /// <param name="targetPosition">Final position in world space.</param>
        /// <param name="duration">
        /// How many seconds the move should take.
        /// </param>
        public MoveToPositionXYZ(Vector3 targetPosition, float duration)
        {
            this.targetPosition = targetPosition;
            this.duration = duration;
        }

        /// <summary>
        /// On first call, captures the object's initial position.
        /// Each frame, linearly interpolates from the initial position
        /// toward the target over the specified duration.
        /// </summary>
        /// <param name="obj">The GameObject to move.</param>
        /// <param name="deltaTime">Time in seconds since last frame.</param>
        public void Apply(GameObject obj, float deltaTime)
        {
            if (!started)
            {
                // Record the starting position once
                startPosition = obj.Transform.Position;
                started = true;
            }
            
            // Accumulate time
            elapsed += deltaTime;
            
            // Calculate interpolation factor from 0..1
            float t = Math.Clamp(elapsed / duration, 0f, 1f);

            // LERP from start to target position
            var newPos = Vector3.Lerp(startPosition, targetPosition, t);
            obj.Transform.Position = newPos;
        }

        /// <summary>
        /// Returns true once we've reached or exceeded the duration,
        /// meaning the object should be at its target.
        /// </summary>
        public bool IsDone => elapsed >= duration;
        
        public void Reset()
        {
            elapsed = 0f;
            started = false;
        }
    }
}
