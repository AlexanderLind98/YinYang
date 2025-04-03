namespace YinYang.Shapes
{
    public class TriangleMesh : Mesh
    {
        public TriangleMesh()
            : base(new float[]
            {
                // positions only
                -0.5f, -0.5f, 0.0f,  // bottom left
                0.5f, -0.5f, 0.0f,  // bottom right
                0.0f,  0.5f, 0.0f   // top
            }, 3) // 3 floats per vertex
        {
        }
    }
}