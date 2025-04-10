using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Pulses the object's scale a fixed number of full sine wave cycles.
    /// Each cycle consists of a full up-down pulse.
    /// </summary>
    public class PulseScaleXTimes : IFiniteMotion, IResetMotion
    {
        private readonly float amplitude;
        private readonly float frequency;
        private readonly int maxCycles;

        private float elapsedTime = 0f;
        private Vector3? baseScale = null;
        private bool isDone = false;
        private int completedCycles = 0;
        private float previousSin = 0f;

        public PulseScaleXTimes(float amplitude, float frequency, int cycles = 1)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.maxCycles = cycles;
        }

        public void Apply(GameObject obj, float deltaTime)
        {
            if (isDone) return;

            if (baseScale == null)
                baseScale = obj.Transform.Scale;

            elapsedTime += deltaTime;

            float omega = 2f * MathF.PI * frequency;
            float sin = MathF.Sin(omega * elapsedTime);
            float scaleFactor = 1.0f + amplitude * sin;

            obj.Transform.Scale = baseScale.Value * scaleFactor;

            // Detect cycle: sin passes through 0 going upward (start of new cycle)
            if (previousSin < 0 && sin >= 0)
            {
                completedCycles++;
                if (completedCycles >= maxCycles)
                    isDone = true;
            }

            previousSin = sin;
        }

        public bool IsDone => isDone;

        public void Reset()
        {
            elapsedTime = 0f;
            baseScale = null;
            isDone = false;
            completedCycles = 0;
            previousSin = 0f;
        }
    }
}