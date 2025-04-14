using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Lights;
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
        private Matrix4 lightSpaceMatrix;
        private bool hasRenderedShadow = false;

        /// <summary>
        /// The depth texture produced by this shadow pass.
        /// </summary>
        public Texture ShadowDepthTexture => shadowDepthTexture;

        /// <summary>
        /// Initializes the framebuffer, depth texture, and shader required for shadow rendering.
        /// </summary>
        public ShadowRenderPass(LightingManager lightingManager)
        {
            // Load the shader responsible for writing depth values only
            shadowShader = new Shader("Shaders/Shadow/DirDepth.vert", "Shaders/Shadow/DirDepth.frag");

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
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
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
    public override Matrix4? Execute(RenderContext context, ObjectManager objects)
    {
        // GL.CullFace(TriangleFace.Front);
        
        //As light projection is still needed, we capture it once, and then only return identity to save on resource usage
        if (hasRenderedShadow && context.Lighting.Sun.shadowType == Light.ShadowType.None)
        {
            return Matrix4.Identity;
        }
        
        // Orthographic projection to simulate infinite directional light projection.
        Matrix4 lightProjection = Matrix4.CreateOrthographicOffCenter(-50.0f, 50.0f, -50f, 50f, 0.1f, 50.0f);

        Vector3 camPos = context.Camera.Position;
        Vector3 offSet = new Vector3(20.0f, 20.0f, 20.0f);
        Vector3 lightPosition = (camPos + offSet);
        
        context.Lighting.Sun.SetPosition(lightPosition.X, lightPosition.Y, lightPosition.Z);
        
        // Create a view matrix from the light's position looking along its rotation vector.
        Matrix4 lightView = Matrix4.LookAt(
            lightPosition,
            context.Lighting.Sun.Transform.Position + context.Lighting.Sun.Transform.Rotation,
            Vector3.UnitY);

        // Combine projection and view to form the light-space matrix.
        lightSpaceMatrix = lightView * lightProjection;

        // Activate the shader and set the light-space matrix.
        shadowShader.Use();
        shadowShader.SetMatrix("lightSpaceMatrix", lightSpaceMatrix);

        // Configure the viewport to match the shadow resolution.
        GL.Viewport(0, 0, shadowResolution, shadowResolution);

        // Bind the depth-only framebuffer and clear it.
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
        
        //Do not clear the depth buffer if we are static
        if(context.Lighting.Sun.shadowType == Light.ShadowType.Static && !hasRenderedShadow)
            GL.Clear(ClearBufferMask.DepthBufferBit);
        else if(context.Lighting.Sun.shadowType != Light.ShadowType.Static)
            GL.Clear(ClearBufferMask.DepthBufferBit);

        // Handle shadow type and render all scene geometry from the light's point of view
        switch (context.Lighting.Sun.shadowType)
        {
            case Light.ShadowType.None: break; //Do not render shadows
            case Light.ShadowType.Static: //Render shadows, but just once
            {
                if (!hasRenderedShadow)
                    objects.RenderDepth(shadowShader);
                break;
            }
            case Light.ShadowType.Dynamic: //No conditions, render shadows every frame
            {
                objects.RenderDepth(shadowShader);
                break;
            }
            default: throw new ArgumentOutOfRangeException();
        }

        // Unbind the framebuffer to return to the default target.
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        var err = GL.GetError();
        if (err != ErrorCode.NoError)
            Console.WriteLine($"[GL ERROR] after {nameof(ShadowRenderPass)}: {err}");

        hasRenderedShadow = true;

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