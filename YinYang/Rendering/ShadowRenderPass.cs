using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Worlds;

namespace YinYang.Rendering
{
    /// <summary>
    /// Render pass responsible for generating the directional light shadow depth map.
    /// </summary>
    /// <remarks>
    /// This pass renders the scene from the sun's perspective into a depth-only framebuffer,
    /// creating a shadow map used for directional shadowing in the main scene pass.
    /// </remarks>
    public class ShadowRenderPass : RenderPass
    {
        private int framebufferHandle;
        private int shadowResolution = 4096;
        private Shader shadowShader;
        private Texture shadowDepthTexture;

        /// <summary>
        /// The depth texture produced by this shadow pass.
        /// </summary>
        public Texture ShadowDepthTexture => shadowDepthTexture;

        /// <summary>
        /// Initializes the framebuffer, depth texture, and shader required for shadow rendering.
        /// </summary>
        public ShadowRenderPass()
        {
            // Load the shader responsible for writing depth values only
            shadowShader = new Shader("Shaders/DirDepth.vert", "Shaders/DirDepth.frag");

            // Create a framebuffer object that will render depth from the light's perspective
            framebufferHandle = GL.GenFramebuffer();

            // Create a 2D depth texture to store the light's depth map
            int textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureHandle);
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.DepthComponent,
                shadowResolution,
                shadowResolution,
                0,
                PixelFormat.DepthComponent,
                PixelType.Float,
                IntPtr.Zero);

            // Set texture parameters to avoid interpolation and ensure edge clamp
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[] { 1, 1, 1, 1 });

            // Attach the depth texture to the framebuffer and disable color writes
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, textureHandle, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            // Wrap the raw texture handle in a reusable abstraction
            shadowDepthTexture = new Texture(textureHandle);
        }

         /// <summary>
    /// Executes the shadow pass by rendering the scene from the directional light's perspective.
    /// </summary>
    /// <remarks>
    /// Produces a light-space transformation matrix which is used in subsequent passes
    /// to compare fragment depth against shadow depth for shadow testing.
    /// </remarks>
    /// <param name="camera">The active camera.</param>
    /// <param name="lighting">The lighting manager containing sun and light data.</param>
    /// <param name="objects">The object manager with all scene objects.</param>
    /// <param name="lightSpaceInput">Input light-space matrix (typically identity).</param>
    /// <param name="currentWorld">The current world instance. (Not used in this pass but required by the signature.)</param>
    /// <returns>The computed light-space transformation matrix.</returns>
    public override Matrix4 Execute(Camera camera, LightingManager lighting, ObjectManager objects, Matrix4 lightSpaceInput, World currentWorld)
    {
        // Orthographic projection to simulate infinite directional light projection.
        Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-10.0f, 10.0f, -10f, 10f, 0.1f, 50.0f);

        // Create a view matrix from the light's position looking along its rotation vector.
        Matrix4 lightView = Matrix4.LookAt(
            lighting.Sun.Transform.Position,
            lighting.Sun.Transform.Position + lighting.Sun.Transform.Rotation,
            Vector3.UnitY);

        // Combine projection and view to form the light-space matrix.
        Matrix4 lightSpaceMatrix = lightView * lightProjection;

        // Activate the shader and set the light-space matrix.
        shadowShader.Use();
        shadowShader.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);

        // Configure the viewport to match the shadow resolution.
        GL.Viewport(0, 0, shadowResolution, shadowResolution);

        // Bind the depth-only framebuffer and clear it.
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
        GL.Clear(ClearBufferMask.DepthBufferBit);

        // Render all scene geometry from the light's point of view.
        objects.RenderDepth(shadowShader);

        // Unbind the framebuffer to return to the default target.
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        // Return the transformation matrix for use in the main lighting pass.
        return lightSpaceMatrix;
    }

        /// <summary>
        /// Releases GPU resources used by this render pass.
        /// </summary>
        public override void Dispose()
        {
            shadowShader.Dispose();
            GL.DeleteFramebuffer(framebufferHandle);
        }
    }
}