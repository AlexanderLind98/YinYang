// a simple upsampling pass inspiret by this article: https://learnopengl.com/Guest-Articles/2022/Phys.-Based-Bloom

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    /// <summary>
    /// Performs a single upsample-and-blur step from one mip level to the next.
    /// </summary>
    public class UpsamplePass : RenderPass
    {
        public int SourceTexture; // lower-res texture (e.g. ¼ res)
        //public int TargetTexture; // higher-res texture (e.g. ½ res)
        
        private int fbo;
        private int targetTexture;
        public int TargetTexture => targetTexture;
        public Vector2i TargetSize;

        private Shader upsampleShader;
        private QuadMesh quad = new();

        public float FilterRadius = 0.005f; // Radius in UV space

        private bool initialized = false;

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            if (!initialized)
            {
                InitFBO();
                initialized = true;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.Viewport(0, 0, TargetSize.X, TargetSize.Y);

            // Enable additive blending
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One); // result = src + dst

            upsampleShader.Use();
            upsampleShader.SetInt("srcTexture", 0);
            upsampleShader.SetFloat("filterRadius", FilterRadius);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, SourceTexture);

            quad.Draw();

            GL.Disable(EnableCap.Blend);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return null;
        }

        private void InitFBO()
        {
            // Create upsample target texture
            targetTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, targetTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, TargetSize.X, TargetSize.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Create and bind framebuffer
            fbo = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, targetTexture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine($"[UpsamplePass] FBO incomplete: {status}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Load shader
            upsampleShader = new Shader("shaders/fullscreen.vert", "shaders/upsample.frag");
        }

        public override void Dispose()
        {
            GL.DeleteFramebuffer(fbo);
            GL.DeleteTexture(targetTexture); 
            upsampleShader.Dispose();        
        }
    }
}
