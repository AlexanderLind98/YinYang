// HDR implemented using LearnOpenGL Giuide
// https://learnopengl.com/Advanced-Lighting/HDR

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;
using YinYang.Worlds;

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

        public override Matrix4 Execute(Camera camera, LightingManager lighting, ObjectManager objects, Matrix4 lightSpaceMatrix, World currentWorld)
        {
            if (!HDR_Enabled)
            {
                // Bypass HDR: render directly to screen
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Render normally
                Matrix4 viewProj = camera.GetViewProjection();
                foreach (var obj in objects.GameObjects)
                {
                    obj.Draw(viewProj, lightSpaceMatrix, camera, currentWorld, 0);
                }

                return lightSpaceMatrix;
            }

            // HDR enabled: render to HDR FBO
            if (!framebufferInitialized)
            {
                InitFrameBuffer();
                framebufferInitialized = true;
            }

            RenderSceneWithHDRFBO(camera, lighting, objects, lightSpaceMatrix, currentWorld);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            toneMappingShader.Use();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, colorTexture);
            toneMappingShader.SetInt("hdrBuffer", 0);

            screenQuad.Draw();

            return lightSpaceMatrix;
        }

        
        // /// <summary>
        // /// Executes the HDR render pass: renders to floating-point buffer,
        // /// then applies tone mapping to the screen.
        // /// Tone mapping convert back to standard color range (0-1) for display.
        // /// </summary>
        // public override Matrix4 Execute(Camera camera, LightingManager lighting, ObjectManager objects, Matrix4 lightSpaceMatrix, World currentWorld)
        // {
        //     // lazy initialization of framebuffer
        //     if (!framebufferInitialized)
        //     {
        //         InitFrameBuffer();
        //         framebufferInitialized = true;
        //     }
        //     
        //     // Step 1: Bind HDR FBO and render scene
        //     RenderSceneWithHDRFBO(camera, lighting, objects, lightSpaceMatrix, currentWorld);
        //
        //     // Bind default framebuffer
        //     GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        //     GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        //
        //     // Tone mapping compresses HDR brightness values into the 0-1 range for display on SDR monitors.
        //     // Typically combined with gamma correction to maintain perceived brightness.
        //     toneMappingShader.Use();
        //     GL.ActiveTexture(TextureUnit.Texture0);
        //     GL.BindTexture(TextureTarget.Texture2D, colorTexture);
        //     toneMappingShader.SetInt("hdrBuffer", 0);
        //     
        //     // Draw fullscreen quad
        //     screenQuad.Draw();
        //
        //     return lightSpaceMatrix;
        // }
        

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

            //Console.WriteLine($"[HDR] Initializing framebuffer at {width}x{height}");
            
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

        /// <summary>
        /// Renders the scene to the HDR framebuffer.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="lighting"></param>
        /// <param name="objects"></param>
        /// <param name="lightSpaceMatrix"></param>
        /// <param name="currentWorld"></param>
        private void RenderSceneWithHDRFBO(Camera camera, LightingManager lighting, ObjectManager objects, Matrix4 lightSpaceMatrix, World currentWorld)
        {
            // Render scene to HDR framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, hdrFBO);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Enable depth testing for scene rendering
            GL.Enable(EnableCap.DepthTest);

            // Bind scene objects and draw them
            foreach (var obj in objects.GameObjects)
            {
                if (obj.Renderer == null) continue;

                var model = obj.Transform.CalculateModel();
                var viewProj = camera.GetViewProjection();

                obj.Renderer.SetSun(currentWorld);
                obj.Renderer.PointLights(currentWorld);
                obj.Renderer.SpotLights(camera, currentWorld);

                obj.Renderer.Draw(viewProj, lightSpaceMatrix, model, camera, 0, currentWorld);
            }

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
