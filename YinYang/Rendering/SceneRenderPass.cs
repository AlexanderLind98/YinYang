using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;

namespace YinYang.Rendering
{
    /// <summary>
    /// Renders the scene into a HDR framebuffer using MRT to output both color and bright textures.
    /// </summary>
    public class SceneRenderPass : RenderPass
    {
        private int hdrFBO;
        private int depthRBO;
        private bool initialized = false;
        
        public int SceneColorTexture { get; private set; }
        public int BrightColorTexture { get; private set; }
        
        /// <summary>
        /// Executes the scene render pass from the camera's viewpoint.
        /// </summary>
        /// <param name="context">Rendering context containing camera, lighting, matrices, etc.</param>
        /// <param name="objects">Scene object manager containing renderable objects.</param>
        /// <returns>Returns the input light-space matrix unmodified.</returns>
        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            // // TEMP: Render directly to screen
            // GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); // <--- Draw to screen!
            // GL.Viewport(0, 0, context.Camera.RenderWidth, context.Camera.RenderHeight);
            // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // GL.Enable(EnableCap.DepthTest);
            //
            // objects.Render(context);
            //
            // return context.LightSpaceMatrix;
            
            if (!initialized)
            {
                InitFramebuffer(context);
                initialized = true;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.Viewport(0, 0, context.Camera.RenderWidth, context.Camera.RenderHeight);
            
            objects.Render(context);

            ErrorCode err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine($"[GL ERROR - SceneRenderPass] {err}");


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return context.LightSpaceMatrix;
        }


        private void InitFramebuffer(RenderContext context)
        {
            int width = context.Camera.RenderWidth;
            int height = context.Camera.RenderHeight;

            hdrFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);

            // Color
            SceneColorTexture = GL.GenTexture();
            SetupTexture(SceneColorTexture, width, height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, SceneColorTexture, 0);

            // Bright
            BrightColorTexture = GL.GenTexture();
            SetupTexture(BrightColorTexture, width, height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1,
                TextureTarget.Texture2D, BrightColorTexture, 0);

            // Draw both
            DrawBuffersEnum[] attachments = { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1 };
            GL.DrawBuffers(2, attachments);

            depthRBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthRBO);

            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"[ScenePass] Incomplete framebuffer: {status}");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void SetupTexture(int handle, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, width, height, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
        }

        public override void Dispose()
        {
            GL.DeleteFramebuffer(hdrFBO);
            GL.DeleteRenderbuffer(depthRBO);
            GL.DeleteTexture(SceneColorTexture);
            GL.DeleteTexture(BrightColorTexture);
        }
    }
}