using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Applies a pulsing effect to a GameObject's scale, using sine wave modulation around its initial scale.
    /// </summary>
    public class PulseScale : IAutoMotion
    {
        private readonly float amplitude;
        private readonly float frequency;
        private float elapsedTime = 0f;
        private Vector3? baseScale = null;

        /// <summary>
        /// Creates a pulsing scale effect using sine wave modulation.
        /// </summary>
        /// <param name="amplitude">Maximum scale offset above/below the original size.</param>
        /// <param name="frequency">Number of scale pulses per second.</param>
        public PulseScale(float amplitude = 0.1f, float frequency = 2f)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        /// <summary>
        /// Applies a sinusoidal scaling effect to the object each frame.
        /// </summary>
        /// <param name="obj">The GameObject whose scale will be modified.</param>
        /// <param name="deltaTime">Time since the last frame.</param>
        public void Apply(GameObject obj, float deltaTime)
        {
            // Capture base scale only once
            if (baseScale == null)
                baseScale = obj.Transform.Scale;

            elapsedTime += deltaTime;

            float omega = 2f * MathF.PI * frequency;
            float scaleFactor = 1.0f + amplitude * MathF.Sin(omega * elapsedTime);

            obj.Transform.Scale = baseScale.Value * scaleFactor;
        }
    }
}