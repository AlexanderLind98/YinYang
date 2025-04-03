using OpenTK.Graphics.OpenGL4;

namespace YinYang
{
    public class Mesh : IDisposable
    {
        private int vao;
        private int vbo;
        private int ebo;
        private int indexCount;
        private int vertexStride; // in bytes

        /// <summary>
        /// Constructs a mesh with explicit vertex and index data.
        /// </summary>
        /// <param name="vertices">Array of vertex data.</param>
        /// <param name="indices">Index array for drawing.</param>
        /// <param name="vertexStrideFloats">Number of floats per vertex.</param>
        public Mesh(float[] vertices, uint[] indices, int vertexStrideFloats)
        {
            if (vertices == null || vertices.Length == 0)
                throw new ArgumentException("Vertices cannot be null or empty.", nameof(vertices));
            if (indices == null || indices.Length == 0)
                throw new ArgumentException("Indices cannot be null or empty.", nameof(indices));

            indexCount = indices.Length;
            vertexStride = vertexStrideFloats * sizeof(float);
            GenerateBuffers(vertices, indices);
        }

        /// <summary>
        /// Constructs a mesh with vertex data only. Indices are automatically created in sequence.
        /// </summary>
        /// <param name="vertices">Array of vertex data.</param>
        /// <param name="vertexStrideFloats">Number of floats per vertex.</param>
        public Mesh(float[] vertices, int vertexStrideFloats)
            : this(vertices, CreateSequentialIndices(vertices.Length / vertexStrideFloats), vertexStrideFloats)
        { }

        private static uint[] CreateSequentialIndices(int vertexCount)
        {
            uint[] indices = new uint[vertexCount];
            for (uint i = 0; i < vertexCount; i++)
                indices[i] = i;
            return indices;
        }

        private void GenerateBuffers(float[] vertices, uint[] indices)
        {
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            // Upload vertex data.
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            // Upload index data.
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Define vertex attribute layout.
            // Position: location 0 (3 floats)
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexStride, 0);
            GL.EnableVertexAttribArray(0);

            // Teksturkoordinater: location 1 (2 floats)
            if (vertexStride > 3 * sizeof(float))
            {
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexStride, 3 * sizeof(float));
                GL.EnableVertexAttribArray(1);
            }

            // Normaler: location 2 (3 floats)
            if (vertexStride > 5 * sizeof(float))
            {
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexStride, 5 * sizeof(float));
                GL.EnableVertexAttribArray(2);
            }


            GL.BindVertexArray(0);
        }

        public void Draw()
        {
            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteVertexArray(vao);
        }
    }
}