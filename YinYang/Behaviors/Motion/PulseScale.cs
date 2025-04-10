using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Scales an object up and down around a base scale using a sine wave pattern.
    /// </summary>
    public class PulseScale : IAutoMotion
    {
        private readonly Vector3 baseScale;
        private readonly float amplitude;
        private readonly float frequency;

        /// <summary>
        /// Constructs a PulseScale motion that oscillates an object's scale.
        /// </summary>
        /// <param name="baseScale">The starting/center scale around which to oscillate.</param>
        /// <param name="amplitude">How far above/below the base scale the object can grow or shrink.</param>
        /// <param name="frequency">How quickly the object's scale oscillates (cycles per second).</param>
        public PulseScale(Vector3 baseScale, float amplitude = 0.1f, float frequency = 2f)
        {
            this.baseScale = baseScale;
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        /// <summary>
        /// Applies a sinus scale factor to the object each frame, causing it to pulse around the specified base scale.
        /// </summary>
        /// <param name="obj">The GameObject whose scale will be modified.</param>
        /// <param name="time">The elapsed time (in seconds) used to calculate the sinusoidal scaling.</param>
        public void Apply(GameObject obj, float time)
        {
            // Calculate the scale factor using a sine wave
            float scaleFactor = 1.0f + amplitude * MathF.Sin(time * frequency);
            
            // Apply the scale factor to the object's scale
            obj.Transform.Scale = baseScale * scaleFactor;
        }
    }
}