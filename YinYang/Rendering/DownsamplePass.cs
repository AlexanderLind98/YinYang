// a simple downsampling pass in spiret by this article: https://learnopengl.com/Guest-Articles/2022/Phys.-Based-Bloom

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    public class DownsamplePass : RenderPass
    {
        public int InputTexture { get; set; }
        private int downsampledTexture;
        public int DownsampledTexture => downsampledTexture;


        private int fbo;
        private Shader downsampleShader;
        private QuadMesh quad = new();
        private bool initialized = false;

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            if (!initialized)
            {
                Init(context);
                initialized = true;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.Viewport(0, 0, context.Camera.RenderWidth / 2, context.Camera.RenderHeight / 2);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            downsampleShader.Use();
            downsampleShader.SetInt("srcTexture", 0);
            downsampleShader.SetVector2("srcResolution", new Vector2(context.Camera.RenderWidth, context.Camera.RenderHeight));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, InputTexture);

            quad.Draw();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return null;
        }

        private void Init(RenderContext context)
        {
            int w = context.Camera.RenderWidth / 2;
            int h = context.Camera.RenderHeight / 2;

            // Create downsample target texture
            downsampledTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, downsampledTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, w, h, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Create and bind framebuffer
            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, downsampledTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine($"[DownsamplePass] FBO incomplete: {status}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Load shader
            downsampleShader = new Shader("shaders/fullscreen.vert", "shaders/downsample.frag");
        }

        public override void Dispose()
        {
            GL.DeleteFramebuffer(fbo);
            GL.DeleteTexture(downsampledTexture); 
            downsampleShader.Dispose();        
        }
    }
}
