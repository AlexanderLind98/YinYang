using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    /// <summary>
    /// Composites the HDR scene texture and blurred bloom texture using exposure tone mapping.
    /// </summary>
    public class CompositePass : RenderPass
    {
        public bool BloomEnabled = true;
        public int SceneTexture { get; set; }
        public int BloomTexture { get; set; }
        public float Exposure { get; set; } = 0.1f;

        private Shader blendShader = new Shader("shaders/fullscreen.vert", "shaders/blending.frag");
        private QuadMesh screenQuad = new();

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            blendShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, SceneTexture);
            blendShader.SetInt("scene", 0);

            if (BloomEnabled)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, BloomTexture);
                blendShader.SetInt("bloomBlur", 1);
            }

            blendShader.SetFloat("exposure", Exposure);
            blendShader.SetInt("bloomEnabled", BloomEnabled ? 1 : 0);

            screenQuad.Draw();
            
            return context.LightSpaceMatrix;
        }

        public override void Dispose()
        {
            blendShader.Dispose();
        }
    }
}