namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Moves an object forward and backward (Z-axis) using a sine wave motion over time.
    /// </summary>
    public class FowardBackward // 
    {
        private readonly float amplitude;
        private readonly float frequency;
        private readonly float baseZ;
        
        /// <summary>
        /// Constructs a FowardBackward motion behavior.
        /// </summary>
        /// <param name="amplitude">How far forward/backward of baseZ the object should move.</param>
        /// <param name="frequency">How quickly the object oscillates.</param>
        /// <param name="baseZ">The depth-axis center of the oscillation.</param>
        public FowardBackward(float amplitude, float frequency, float baseZ)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.baseZ = baseZ;
        }

        /// <summary>
        /// Applies a sine-wave forward/backward motion to the object's Z position each frame.
        /// </summary>
        /// <param name="obj">The target GameObject to move.</param>
        /// <param name="time">Accumulated time (in seconds) for calculating the sine wave.</param>
        public void Apply(GameObject obj, float time)
        {
            // gewt the current position of the object
            var position = obj.Transform.Position;

            // Update the Z coordinate with a sine-based offset around baseZ
            position.Z = baseZ + amplitude * MathF.Sin(frequency * time);

            // Assign the modified position back
            obj.Transform.Position = position;
        }
    }
}