using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Smoothly scales a GameObject from its current Transform.Scale
    /// to a specified target scale over a defined duration.
    /// </summary>
    public class ScaleTo : IAutoMotion
    {
        private readonly Vector3 targetScale;
        private readonly float duration;
        private float elapsed;
        private Vector3 startScale;
        private bool started = false;

        /// <summary>
        /// Constructs a motion step that interpolates the object's scale
        /// from its current size to a given target over a certain time.
        /// </summary>
        /// <param name="targetScale">The final desired scale for the object.</param>
        /// <param name="duration">How long (seconds) the scaling should take.</param>
        public ScaleTo(Vector3 targetScale, float duration)
        {
            this.targetScale = targetScale;
            this.duration = duration;
        }

        /// <summary>
        /// On first apply, captures the object's current scale.
        /// Each frame, linearly interpolates from that scale
        /// toward the target scale over the given duration.
        /// </summary>
        /// <param name="obj">The object whose scale is being changed.</param>
        /// <param name="deltaTime">Time passed since last frame, in seconds.</param>
        public void Apply(GameObject obj, float deltaTime)
        {
            if (!started)
            {
                startScale = obj.Transform.Scale;
                started = true;
            }

            elapsed += deltaTime;
            float t = Math.Clamp(elapsed / duration, 0f, 1f);

            // LERP each scale component
            float sx = MathHelper.Lerp(startScale.X, targetScale.X, t);
            float sy = MathHelper.Lerp(startScale.Y, targetScale.Y, t);
            float sz = MathHelper.Lerp(startScale.Z, targetScale.Z, t);

            obj.Transform.Scale = new Vector3(sx, sy, sz);
        }

        /// <summary>
        /// Indicates whether we've completed the full duration of the scaling.
        /// </summary>
        public bool IsDone => elapsed >= duration;
    }
}
