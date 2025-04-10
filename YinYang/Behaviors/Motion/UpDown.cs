namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object up and down (Y-axis) using a sine wave motion over time.
    /// </summary>
    public class UpDown : IAutoMotion
    {
        private readonly float amplitude;
        private readonly float frequency;
        
        private float elapsedTime = 0f;
        private float? initialY = null;
        
        /// <summary>
        /// Constructs an UpDown motion behavior.
        /// </summary>
        /// <param name="amplitude">Maximum distance the object moves above and below its original Y position.</param>
        /// <param name="frequency">Oscillations per second (Hz). Higher values = faster bobbing.</param>
        public UpDown(float amplitude, float frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }
        
        public void Apply(GameObject obj, float deltaTime)
        {
            // Cache the object's starting Y-position on the first frame
            if (initialY == null)
                initialY = obj.Transform.Position.Y;

            // Advance time
            elapsedTime += deltaTime;

            // Calculate angular velocity (2πf) and use it to compute the offset
            float omega = 2f * MathF.PI * frequency;
            float offsetY = amplitude * MathF.Sin(omega * elapsedTime);

            // Apply the new Y position, preserving X/Z
            var pos = obj.Transform.Position;
            pos.Y = initialY.Value + offsetY;
            obj.Transform.Position = pos;
        }
    }
}