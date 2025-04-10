using OpenTK.Mathematics;
using YinYang.Components;

namespace YinYang.Lights
{
    public class Light
    {
        public Vector3 LightColor { get; set; } = Vector3.One;
        protected Vector3 DefaultColor = Vector3.One;
        protected float LightIntensity { get; set; } = 1.0f;

        public float Constant = 1.0f;
        public float Linear = 0.09f;
        public float Quadratic = 0.032f;

        public GameObject? Visualizer;
   
        /// <summary>
        /// optional transfrom
        /// </summary>
        public virtual Transform? Transform => null;

        /// <summary>
        /// Toggles light color between default and zero (off).
        /// </summary>
        public void ToggleLight()
        {
            LightColor = LightColor == DefaultColor ? Vector3.Zero : DefaultColor;
        }

        /// <summary>
        /// Subclasses must override if they use a transform.
        /// </summary>
        /// <param name="pitchDegrees">Rotation around the X-axis in degrees.</param>
        /// <param name="yawDegrees">Rotation around the Y-axis in degrees.</param>
        /// <param name="rollDegrees">Rotation around the Z-axis in degrees.</param>
        public virtual void SetRotationInDegrees(float pitchDegrees, float yawDegrees, float rollDegrees) { }

        /// <summary>
        /// Sets the position of the light. 
        /// </summary>
        public virtual Vector3 SetPosition(float x, float y, float z)
        {
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Converts rotation expressed in degrees to a normalized forward direction vector.
        /// </summary>
        /// <param name="rotationDegrees">Euler angles in degrees.</param>
        /// <returns>Normalized direction vector.</returns>
        protected Vector3 GetDirectionFromDegrees(Vector3 rotationDegrees)
        {
            Vector3 radians = new Vector3(
                MathHelper.DegreesToRadians(rotationDegrees.X),
                MathHelper.DegreesToRadians(rotationDegrees.Y),
                MathHelper.DegreesToRadians(rotationDegrees.Z)
            );

            float pitch = radians.X;
            float yaw = radians.Y;

            float x = MathF.Cos(pitch) * MathF.Cos(yaw);
            float y = MathF.Sin(pitch);
            float z = MathF.Cos(pitch) * MathF.Sin(yaw);

            return Vector3.Normalize(new Vector3(-x, -y, -z));
        }
    }
}
