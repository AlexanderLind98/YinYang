using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Shapes;

namespace YinYang;

/// <summary>
/// Renders debug overlays such as the shadow depth map in a screen-space quad.
/// Intended for visual debugging of offscreen buffers and render internals.
/// </summary>
public class DebugOverlay : IDisposable
{
    private Shader debugShader;
    private Mesh quad;

    /// <summary>
    /// Initializes the debug renderer with a fullscreen quad and shader.
    /// </summary>
    public DebugOverlay()
    {
        debugShader = new Shader("Shaders/shadowDebugQuad.vert", "Shaders/shadowDebugQuad.frag");
        quad = new QuadMesh();
    }

    /// <summary>
    /// Draws a debug visualization of the given depth texture in the bottom-left corner.
    /// Temporarily disables depth test and culling, and restores GL state after rendering.
    /// </summary>
    /// <param name="depthMap">The depth texture to visualize.</param>
    /// <param name="screenSize">The full resolution of the screen.</param>
    public void Draw(Texture depthMap, Vector2i screenSize)
    {
        using (new GLStateScope())
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.Viewport(0, 0, screenSize.X / 4, screenSize.Y / 4);

            debugShader.Use();
            debugShader.SetInt("depthMap", 9);
            depthMap.Use(TextureUnit.Texture9);

            quad.Draw();
        }
    }

    /// <summary>
    /// Disposes the debug renderer and releases GPU resources.
    /// </summary>
    public void Dispose()
    {
        debugShader.Dispose();
    }

    /// <summary>
    /// Utility class that captures and restores OpenGL state affected by debug rendering.
    /// Restores depth test, culling, active program, VAO, and viewport.
    /// Used via 'using' to guarantee state reset.
    /// </summary>
    /// <remarks>
    /// Sealed to prevent subclassing — this scope is not intended to be extended or modified externally.
    /// </remarks>
    private sealed class GLStateScope : IDisposable
    {
        private readonly bool depthTestEnabled;
        private readonly bool cullFaceEnabled;
        private readonly int[] viewport = new int[4];
        private readonly int program;
        private readonly int vao;

        /// <summary>
        /// Captures relevant OpenGL state.
        /// </summary>
        public GLStateScope()
        {
            depthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
            cullFaceEnabled = GL.IsEnabled(EnableCap.CullFace);
            GL.GetInteger(GetPName.Viewport, viewport);
            GL.GetInteger(GetPName.CurrentProgram, out program);
            GL.GetInteger(GetPName.VertexArrayBinding, out vao);
        }

        /// <summary>
        /// Restores OpenGL state to what it was before the debug draw.
        /// </summary>
        public void Dispose()
        {
            if (depthTestEnabled) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
            if (cullFaceEnabled) GL.Enable(EnableCap.CullFace); else GL.Disable(EnableCap.CullFace);

            GL.UseProgram(program);
            GL.BindVertexArray(vao);
            GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
} 
