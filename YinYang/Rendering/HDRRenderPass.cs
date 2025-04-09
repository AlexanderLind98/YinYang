// HDR implemented using LearnOpenGL Giuide
// https://learnopengl.com/Advanced-Lighting/HDR

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    /// <summary>
    /// Render pass that draws the scene to a floating-point framebuffer (HDR),
    /// then performs tone mapping and gamma correction to output to screen.
    /// </summary>
    /// <remarks>
    /// HDR rendering allows for a wider range of colors and brightness levels, then LDR(Low Dynamic Range(0-1).
    /// </remarks>
    public class HDRRenderPass : RenderPass
    {
        private int hdrFBO;
        private int colorTexture;
        private int depthRBO;

        private Shader toneMappingShader;
        
        // Fullscreen quad for tone mapping output
        private readonly QuadMesh screenQuad = new QuadMesh();
        
        public bool HDR_Enabled { get; set; } = true;
        private bool framebufferInitialized = false;

        /// <summary>
        /// Initializes the HDR framebuffer and tone mapping shader.
        /// </summary>
        public HDRRenderPass()
        {
            // Load tone mapping shader
            toneMappingShader = new Shader("shaders/tonemap.vert", "shaders/tonemap.frag");
        }

        /// <summary>
        /// Executes the HDR render pass by rendering to a floating-point framebuffer
        /// and then applying tone mapping and gamma correction to display the final image.
        /// </summary>
        /// <param name="context">Rendering context containing camera, lighting, matrices, etc.</param>
        /// <param name="objects">Scene object manager containing renderable objects.</param>
        /// <returns>The current frame’s light-space matrix (used for shadow mapping).</returns>
        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            if (!HDR_Enabled)
            {
                // Bypass HDR, draw directly to screen
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                objects.Render(context);
                return context.LightSpaceMatrix;
            }

            if (!framebufferInitialized)
            {
                InitFrameBuffer();
                framebufferInitialized = true;
            }

            // Step 1: Render to HDR framebuffer
            RenderSceneToHDRFramebuffer(context, objects);

            // Step 2: Apply tone mapping to default framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            toneMappingShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            toneMappingShader.SetInt("hdrBuffer", 0);
            
            screenQuad.Draw();
       
            //TODO: FIXME
            /*var err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine($"[GL ERROR] after {nameof(HDRRenderPass)}: {err}");*/

            return context.LightSpaceMatrix;
        }

        /// <summary>
        /// Renders the scene to the HDR framebuffer using the current frame context.
        /// </summary>
        /// <param name="context">The frame-wide render context containing camera, matrices, lighting, etc.</param>
        /// <param name="objects">The object manager containing all renderable entities.</param>
        private void RenderSceneToHDRFramebuffer(RenderContext context, ObjectManager objects)
        {
            // Step 1: Bind HDR framebuffer and clear it
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Enable depth testing while rendering the 3D scene
            GL.Enable(EnableCap.DepthTest);

            // Draw all scene objects into the HDR framebuffer
            objects.Render(context);
        }
       

        /// <summary>
        /// Initializes the HDR framebuffer and its attachments.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void InitFrameBuffer() 
        {
            // Get the current viewport size
            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            int width = viewport[2];
            int height = viewport[3];
            
            // Create the HDR framebuffer object
            hdrFBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
            
            // Create a floating-point texture for HDR rendering.
            // RGBA16F format allows each channel to exceed 1.0, enabling high-brightness colors (e.g. bloom, sunlight).
            colorTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba16f,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.Float,
                IntPtr.Zero);
            
            // Set texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Create a depth renderbuffer used for depth testing during scene rendering.
            // We do not sample this in shaders, so a renderbuffer is faster and sufficient.
            depthRBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthRBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.DepthComponent24, width, height);

            // Framebuffer is the container that captures color + depth during offscreen rendering.
            // We bind the colorTexture and depthRBO as its attachments.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
            
            // Attach the color texture and depth renderbuffer to the framebuffer
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorTexture, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthRBO);

            // Check if framebuffer is complete, else throw an hard error
            var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (status != FramebufferErrorCode.FramebufferComplete)
                throw new Exception($"[HDR] Framebuffer incomplete: {status}");

            // Unbind the framebuffer and renderbuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public override void Dispose()
        {
            GL.DeleteFramebuffer(hdrFBO);
            GL.DeleteTexture(colorTexture);
            GL.DeleteRenderbuffer(depthRBO);
            toneMappingShader.Dispose();
        }
    }
}