using OpenTK.Mathematics;

namespace YinYang
{
    /// <summary>
    /// Represents a 3D model containing geometry data such as vertices, normals, texture coordinates, and indices.
    /// </summary>
    public class Model 
    {
        /// <summary>
        /// Stores the 3D vertex positions of the model.
        /// </summary>
        public List<Vector3> Vertices { get; set; } = new List<Vector3>();

        /// <summary>
        /// Stores the normal vectors used for lighting calculations.
        /// </summary>
        public List<Vector3> Normals { get; set; } = new List<Vector3>();

        /// <summary>
        /// Stores the 2D texture coordinates for mapping textures onto the model.
        /// </summary>
        public List<Vector2> TextureCoords { get; set; } = new List<Vector2>();

        /// <summary>
        /// Stores the indices defining the order in which vertices are drawn.
        /// </summary>
        public List<uint> Indices { get; set; } = new List<uint>();

        /// <summary>
        /// Stores indices referencing the texture coordinates used in the geometry.
        /// </summary>
        public List<int> TextureIndices { get; set; } = new List<int>();

        /// <summary>
        /// Stores indices referencing the normals used in the model's geometry.
        /// </summary>
        public List<int> NormalIndices { get; set; } = new List<int>();
    }
}