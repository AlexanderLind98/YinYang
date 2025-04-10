namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object forward and backward (Z-axis) using a sine wave motion over time.
    /// </summary>
    public class FowardBackward : IAutoMotion 
    {
        private readonly float amplitude;
        private readonly float frequency;
        private float elapsedTime = 0f;
        private float? initialZ = null;
        
        /// <summary>
        /// Constructs a FowardBackward motion behavior.
        /// </summary>
        /// <param name="amplitude">Maximum distance the object moves above and below its original Y position.</param>
        /// <param name="frequency">Oscillations per second (Hz). Higher values = faster bobbing.</param>
        public FowardBackward(float amplitude, float frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        public void Apply(GameObject obj, float deltaTime)
        {
            // gewt the initial Z position on the first frame
            if (initialZ == null)
                initialZ = obj.Transform.Position.Z;

            // Progress animation time
            elapsedTime += deltaTime;

            // Calculate sine wave based on frequency and elapsed time
            float omega = 2f * MathF.PI * frequency;
            float offsetZ = amplitude * MathF.Sin(omega * elapsedTime);

            // Apply to Z only, preserving X and Y
            var pos = obj.Transform.Position;
            pos.Z = initialZ.Value + offsetZ;
            obj.Transform.Position = pos;
        }
    }
}