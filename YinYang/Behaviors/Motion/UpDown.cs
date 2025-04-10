namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object up and down (Y-axis) using a sine wave motion over time.
    /// </summary>
    public class UpDown
    {
        private readonly float amplitude;
        private readonly float frequency;
        private readonly float baseY;
        
        /// <summary>
        /// Constructs an UpDown motion behavior.
        /// </summary>
        /// <param name="amplitude">How far above and below baseY the object should move.</param>
        /// <param name="frequency">How quickly the object oscillates.</param>
        /// <param name="baseY">The vertical center of the oscillation.</param>
        public UpDown(float amplitude, float frequency, float baseY)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.baseY = baseY;
        }

        /// <summary>
        /// Applies a sine-wave up/down motion to the object's Y position each frame.
        /// </summary>
        /// <param name="obj">The target GameObject to move.</param>
        /// <param name="time">Accumulated time (in seconds) for calculating the sine wave.</param>
        public void Apply(GameObject obj, float time)
        {
            // Get the current position of the object
            var position = obj.Transform.Position;

            // Update the Y coordinate with a sine-based offset around baseY
            position.Y = baseY + amplitude * MathF.Sin(frequency * time);

            // Assign the modified position back
            obj.Transform.Position = position;
        }
    }
}