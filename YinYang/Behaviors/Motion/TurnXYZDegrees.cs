using OpenTK.Mathematics;

namespace YinYang.Behaviors.Motion
{
    /// <summary>
    /// Smoothly rotates a GameObject by a relative Euler offset (in degrees) over a set duration.
    /// </summary>
    public class TurnXYZDegrees : IFiniteMotion, IResetMotion
    {
        private readonly Vector3 degreesToTurn;
        private readonly float duration;

        private float elapsed;
        private Vector3 StartRotationInDegrees;
        private Vector3 targetRotationInDegrees;
        private bool started = false;

        /// <summary>
        /// Rotates the object by a relative offset (in degrees) from its current rotation.
        /// </summary>
        /// <param name="degreesToTurn">Rotation to apply (pitch, yaw, roll in degrees).</param>
        /// <param name="duration">How long the rotation should take in seconds.</param>
        public TurnXYZDegrees(Vector3 degreesToTurn, float duration)
        {
            this.degreesToTurn = degreesToTurn;
            this.duration = duration;
        }

        public void Apply(GameObject obj, float deltaTime)
        {
            if (!started)
            {
                StartRotationInDegrees = obj.Transform.GetRotationInDegrees();
                targetRotationInDegrees = StartRotationInDegrees + degreesToTurn;
                started = true;
            }

            elapsed += deltaTime;
            float t = Math.Clamp(elapsed / duration, 0f, 1f);

            float rx = MathHelper.Lerp(StartRotationInDegrees.X, targetRotationInDegrees.X, t);
            float ry = MathHelper.Lerp(StartRotationInDegrees.Y, targetRotationInDegrees.Y, t);
            float rz = MathHelper.Lerp(StartRotationInDegrees.Z, targetRotationInDegrees.Z, t);

            obj.Transform.SetRotationInDegrees(rx, ry, rz);
        }

        public bool IsDone => elapsed >= duration;

        public void Reset()
        {
            elapsed = 0f;
            started = false;
        }
    }
}