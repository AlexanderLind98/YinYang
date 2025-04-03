namespace YinYang.Shapes
{
    public class QuadMesh : Mesh
    {
        public QuadMesh()
            : base(new float[]
                {
                    // positions         // texture coords
                    0.5f,  0.5f, 0.0f,   1.0f, 1.0f, // top right
                    0.5f, -0.5f, 0.0f,   1.0f, 0.0f, // bottom right
                    -0.5f, -0.5f, 0.0f,   0.0f, 0.0f, // bottom left
                    -0.5f,  0.5f, 0.0f,   0.0f, 1.0f  // top left
                },
                new uint[]
                {
                    0, 1, 3,   // first triangle
                    1, 2, 3    // second triangle
                },
                5) // 5 floats per vertex
        {
        }
    }
}