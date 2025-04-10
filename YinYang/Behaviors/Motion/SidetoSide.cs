namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object side to side (X-axis) using a sine wave motion over time.
    /// </summary>
    public class SidetoSide : IAutoMotion
    {
        private readonly float amplitude;
        private readonly float frequency;
        private float elapsedTime = 0f;
        private float? initialX = null;
        
        /// <summary>
        /// Constructs a SidetoSide motion behavior.
        /// </summary>
        /// <param name="amplitude">Maximum distance the object moves above and below its original Y position.</param>
        /// <param name="frequency">Oscillations per second (Hz). Higher values = faster bobbing.</param>
        public SidetoSide(float amplitude, float frequency)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
        }

        public void Apply(GameObject obj, float deltaTime)
        {
            // get the initial X position on the first frame
            if (initialX == null)
                initialX = obj.Transform.Position.X;

            // Accumulate time for continuous sine function
            elapsedTime += deltaTime;

            // Compute angular frequency and evaluate sine
            float omega = 2f * MathF.PI * frequency;
            float offsetX = amplitude * MathF.Sin(omega * elapsedTime);

            // Update only X component
            var pos = obj.Transform.Position;
            pos.X = initialX.Value + offsetX;
            obj.Transform.Position = pos;
        }
    }
}