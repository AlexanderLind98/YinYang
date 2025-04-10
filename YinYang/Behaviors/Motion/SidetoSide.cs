namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object side to side (X-axis) using a sine wave motion over time.
    /// </summary>
    public class SidetoSide // 
    {
        private readonly float amplitude;
        private readonly float frequency;
        private readonly float baseX;
        
        /// <summary>
        /// Constructs a SidetoSide motion behavior.
        /// </summary>
        /// <param name="amplitude">How far left/right of baseX the object should move.</param>
        /// <param name="frequency">How quickly the object oscillates.</param>
        /// <param name="baseX">The horizontal center of the oscillation.</param>
        public SidetoSide(float amplitude, float frequency, float baseX)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.baseX = baseX;
        }

        /// <summary>
        /// Applies a sine-wave left/right motion to the object's X position each frame.
        /// </summary>
        /// <param name="obj">The target GameObject to move.</param>
        /// <param name="time">Accumulated time (in seconds) for calculating the sine wave.</param>
        public void Apply(GameObject obj, float time)
        {
            // get the current position of the object
            var position = obj.Transform.Position;

            // Update the X coordinate with a sine-based offset around baseX
            position.X = baseX + amplitude * MathF.Sin(frequency * time);

            // Assign the modified position back
            obj.Transform.Position = position;
        }
    }
}