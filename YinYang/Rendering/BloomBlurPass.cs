using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    /// <summary>
    /// Applies a multi-pass Gaussian blur to the bright areas extracted from the scene.
    /// </summary>
    public class BloomBlurPass : RenderPass
    {
        public int InputBrightTexture { get; set; }
        public int BlurredBloomTexture { get; private set; }

        private int[] pingpongFBO = new int[2];
        private int[] pingpongBuffer = new int[2];
        private Shader blurShader = new Shader("shaders/bloomblur.vert", "shaders/bloomblur.frag");
        private QuadMesh screenQuad = new();

        private bool initialized = false;

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            if (!initialized)
            {
                InitPingPongBuffers();
                initialized = true;
            }

            bool horizontal = true, first = true;
            int passes = 20;

            blurShader.Use();
            for (int i = 0; i < passes; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingpongFBO[horizontal ? 1 : 0]);
                blurShader.SetInt("horizontal", horizontal ? 1 : 0);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, first ? InputBrightTexture : pingpongBuffer[horizontal ? 0 : 1]);

                screenQuad.Draw();
                horizontal = !horizontal;
                if (first) first = false;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            BlurredBloomTexture = pingpongBuffer[horizontal ? 0 : 1];

            return null;
        }

        private void InitPingPongBuffers()
        {
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            int w = viewport[2], h = viewport[3];

            GL.GenFramebuffers(2, pingpongFBO);
            GL.GenTextures(2, pingpongBuffer);

            for (int i = 0; i < 2; i++)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, pingpongFBO[i]);
                GL.BindTexture(TextureTarget.Texture2D, pingpongBuffer[i]);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, w, h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, pingpongBuffer[i], 0);
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public override void Dispose()
        {
            GL.DeleteFramebuffers(2, pingpongFBO);
            GL.DeleteTextures(2, pingpongBuffer);
            blurShader.Dispose();
        }
    }
}
