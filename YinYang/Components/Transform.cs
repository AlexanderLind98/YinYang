using OpenTK.Mathematics;

namespace YinYang.Components
{
    /// <summary>
    /// Represents a transformation in 3D space.
    /// </summary>
    public class Transform
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public Vector3 Position { get; set; } = Vector3.Zero;

        /// <summary>
        /// Gets or sets the rotation (in radians).
        /// </summary>
        public Vector3 Rotation { get; set; } = Vector3.Zero;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public Vector3 Scale { get; set; } = Vector3.One;

        /// <summary>
        /// Calculates the model matrix from position, rotation, and scale.
        /// </summary>
        /// <returns>The model matrix.</returns>
        public Matrix4 CalculateModel()
        {
            Matrix4 translation = Matrix4.CreateTranslation(Position);
            Matrix4 rotationX = Matrix4.CreateRotationX(Rotation.X);
            Matrix4 rotationY = Matrix4.CreateRotationY(Rotation.Y);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(Rotation.Z);
            Matrix4 scale = Matrix4.CreateScale(Scale);

            return scale * rotationX * rotationY * rotationZ * translation;
        }
    }
}