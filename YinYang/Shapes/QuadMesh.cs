namespace YinYang.Shapes
{
    public class QuadMesh : Mesh
    {
        public QuadMesh()
            : base(new float[]
                {
                    // positions        // texture coords
                    -1.0f,  1.0f, 0.0f,  0.0f, 1.0f,  // top left
                    1.0f,  1.0f, 0.0f,  1.0f, 1.0f,  // top right
                    1.0f, -1.0f, 0.0f,  1.0f, 0.0f,  // bottom right
                    -1.0f, -1.0f, 0.0f,  0.0f, 0.0f   // bottom left
                },
                new uint[]
                {
                    0, 1, 2,
                    2, 3, 0
                },
                5) // 5 floats per vertex (x, y, z, u, v)
        {
        }
    }
}