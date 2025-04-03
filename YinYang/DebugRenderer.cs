using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Shapes;

namespace YinYang;

public class DebugRenderer
{
    private Shader debugShader;
    private Mesh quad;

    public DebugRenderer()
    {
        debugShader = new Shader("Shaders/shadowDebugQuad.vert", "Shaders/shadowDebugQuad.frag");
        quad = new QuadMesh();
    }

    public void Draw(Texture depthMap, Vector2i screenSize)
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.CullFace);

        GL.Viewport(0, 0, screenSize.X / 4, screenSize.Y / 4);

        debugShader.Use();
        debugShader.SetInt("depthMap", 0);
        depthMap.Use(); // binder shadowMap til Texture0
        quad.Draw();

        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.DepthTest);
    }
}