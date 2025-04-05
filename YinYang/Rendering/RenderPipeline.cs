using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;

namespace YinYang.Rendering
{
    /// <summary>
    /// Coordinates all rendering passes including shadow map generation and preparation for scene rendering.
    /// </summary>
    /// <remarks>
    /// Currently handles directional shadow rendering and manages the associated depth framebuffer.
    /// </remarks>
    public class RenderPipeline
    {
        private int shadowFramebufferHandle;
        private int shadowMapResolution = 4096;
        private Shader shadowDepthShader;

        /// <summary>
        /// The texture holding the final shadow depth map used for lighting and shading.
        /// </summary>
        public Texture ShadowDepthTexture { get; private set; }

        /// <summary>
        /// Initializes the shadow rendering pipeline by creating the framebuffer and depth texture.
        /// </summary>
        public void InitializeShadowPass()
        {
            // Load the shader used for rendering the scene from the light's point of view.
            shadowDepthShader = new Shader("Shaders/DirDepth.vert", "Shaders/DirDepth.frag");

            // Create a framebuffer object specifically for shadow depth rendering.
            shadowFramebufferHandle = GL.GenFramebuffer();

            // Create the depth texture to store distances from the light source.
            int shadowDepthTextureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, shadowDepthTextureHandle);
            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                PixelInternalFormat.DepthComponent,
                shadowMapResolution,
                shadowMapResolution,
                0,
                PixelFormat.DepthComponent,
                PixelType.Float,
                IntPtr.Zero);

            // Set texture sampling and wrapping behavior.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 1, 1, 1, 1 });

            // Bind the depth texture to the framebuffer.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowFramebufferHandle);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                shadowDepthTextureHandle,
                0);

            // Disable color rendering to this framebuffer.
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);

            // Unbind for now.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Wrap the OpenGL texture in a reusable Texture class instance.
            ShadowDepthTexture = new Texture(shadowDepthTextureHandle);
        }

        /// <summary>
        /// Executes the directional shadow rendering pass.
        /// </summary>
        /// <param name="lighting">Access to the directional light's transform.</param>
        /// <param name="objects">Objects to render to the shadow map.</param>
        /// <returns>The light space matrix used for shadow projection in the scene pass.</returns>
        /// <remarks>
        /// The light space matrix transforms world-space coordinates into the light's clip space.
        /// It's used to sample the shadow depth map during scene rendering to determine occlusion.
        /// </remarks>
        public Matrix4 RenderShadowPass(LightingManager lighting, ObjectManager objects)
        {
            // Construct an orthographic projection for the directional light's shadow volume.
            Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10f, 10f, 0.1f, 50.0f);

            // Create a view matrix that looks from the light's position towards its direction.
            Matrix4 lightView = Matrix4.LookAt(
                lighting.Sun.Transform.Position,
                lighting.Sun.Transform.Position + lighting.Sun.Transform.Rotation,
                Vector3.UnitY);

            // Combine the projection and view matrices to create the light-space transform.
            // This matrix converts world-space coordinates into the shadow map's coordinate space.
            Matrix4 lightSpaceMatrix = lightView * lightProjection;

            // Use the shadow depth shader to render depth from the light's perspective.
            shadowDepthShader.Use();
            shadowDepthShader.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);

            // Bind the shadow framebuffer and configure the viewport for shadow resolution.
            GL.Viewport(0, 0, shadowMapResolution, shadowMapResolution);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, shadowFramebufferHandle);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            // Perform a depth-only rendering pass for all scene objects.
            objects.RenderDepth(shadowDepthShader);

            // Unbind the framebuffer to return to the default.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            return lightSpaceMatrix;
        }

        /// <summary>
        /// Dispose of the shadow rendering resources.
        /// </summary>
        public void Dispose()
        {
            shadowDepthShader.Dispose();
            GL.DeleteFramebuffer(shadowFramebufferHandle);
        }
    }
}
