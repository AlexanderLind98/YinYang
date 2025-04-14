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
        private bool bloomEnabled = false;
        public int SceneTexture { get; set; }
        public int BloomTexture { get; set; }
        public float Exposure { get; set; } = 0.1f;

        private Shader blendShader = new Shader("shaders/fullscreen.vert", "shaders/PostProcessing/blending.frag");
        private QuadMesh screenQuad = new();

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            blendShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, SceneTexture);
            blendShader.SetInt("scene", 0);

            blendShader.SetInt("bloomEnabled", bloomEnabled ? 1 : 0);
            if (bloomEnabled)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, BloomTexture);
                blendShader.SetInt("bloomBlur", 1);
            }

            blendShader.SetFloat("exposure", context.BloomSettings.Exposure);
            blendShader.SetFloat("bloomStrength", context.BloomSettings.BloomStrength);

            screenQuad.Draw();
            
            return context.LightSpaceMatrix;
        }

        /// <summary>
        /// Sets whether bloom should be applied during final composition.
        /// </summary>
        public void SetBloomEnabled(bool enabled)
        {
            bloomEnabled = enabled;
        }

        public override void Dispose()
        {
            blendShader.Dispose();
        }
    }
}