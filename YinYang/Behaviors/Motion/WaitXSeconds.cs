namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Behavior step that pauses for a specified duration.
    /// </summary>
    public class WaitXSeconds : IFiniteMotion, IResetMotion
    {
        private readonly float duration;
        private float elapsed;

        /// <summary>
        /// Creates a wait step for the specified number of seconds.
        /// </summary>
        /// <param name="duration">
        /// How many seconds to wait before continuing.
        /// </param>
        public WaitXSeconds(float duration)
        {
            this.duration = duration;
        }

        /// <summary>
        /// Adds the elapsed time to an internal counter.
        /// </summary>
        /// <param name="obj">The target GameObject (unused here).</param>
        /// <param name="deltaTime">Seconds passed since last frame.</param>
        public void Apply(GameObject obj, float deltaTime)
        {
            // Increment internal timer
            elapsed += deltaTime;
        }

        /// <summary>
        /// Returns how many seconds have elapsed so far in this wait step.
        /// </summary>
        public float Elapsed => elapsed;

        /// <summary>
        /// Indicates whether the desired wait time has been reached.
        /// </summary>
        public bool IsDone => elapsed >= duration;
        
        public void Reset()
        {
            elapsed = 0f;
        }
    }
}