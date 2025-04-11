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
        public int SceneTexture { get; set; }
        public int BloomTexture { get; set; }
        public float Exposure { get; set; } = 0.5f;

        private Shader blendShader = new Shader("shaders/blending.vert", "shaders/blending.frag");
        private QuadMesh screenQuad = new();

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            blendShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, SceneTexture);
            blendShader.SetInt("scene", 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, BloomTexture);
            blendShader.SetInt("bloomBlur", 1);

            blendShader.SetFloat("exposure", Exposure);

            screenQuad.Draw();

            //Console.WriteLine($"[Composite] Drawing: SceneTex={SceneTexture}, BloomTex={BloomTexture}, Exposure={Exposure}");
            
            return context.LightSpaceMatrix;
        }

        public override void Dispose()
        {
            blendShader.Dispose();
        }
    }
}