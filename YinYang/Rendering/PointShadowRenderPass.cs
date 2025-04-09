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
    public class PointShadowRenderPass : RenderPass
    {
        private int framebufferHandle;
        private int shadowResolution = 2048;
        private Shader shadowShader;
        private Texture shadowDepthCubeTexture;
        private float nearPlane = 0.1f;
        private float farPlane = 50.0f;

        /// <summary>
        /// The depth texture produced by this shadow pass.
        /// </summary>
        public Texture ShadowDepthCubeTexture => shadowDepthCubeTexture;

        /// <summary>
        /// Initializes the framebuffer, depth texture, and shader required for shadow rendering.
        /// </summary>
        public PointShadowRenderPass()
        {
            //Create shader and framebuffer
            shadowShader = new Shader("Shaders/PointDepth.vert", "Shaders/PointDepth.frag", "Shaders/PointDepth.geom");
            framebufferHandle = GL.GenFramebuffer();
            
            //Assign handle
            int textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, textureHandle);
            
            //Same as generating direct shadow map, but 6x for each face in cubemap
            for (int i = 0; i < 6; ++i)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    PixelInternalFormat.DepthComponent,
                    shadowResolution,
                    shadowResolution,
                    0,
                    PixelFormat.DepthComponent,
                    PixelType.Float,
                    IntPtr.Zero);
            //Texture wrap & filtering params
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            // attach depth texture as FBO's deph buffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, textureHandle, 0);
            GL.DrawBuffer(DrawBufferMode.None);
            GL.ReadBuffer(ReadBufferMode.None);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
            shadowDepthCubeTexture = new CubeTexture(textureHandle);
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
        
        // 0. create depth cubemap transformation matrices
        Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), shadowResolution / shadowResolution, nearPlane, farPlane);
        List<Matrix4> shadowTransforms = new List<Matrix4>();
        Vector3 lightPos = context.Lighting.PointLights[0].Transform.Position;                        
        /*shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f,  1.0f, 0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f,  1.0f, 0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f,  1.0f, 0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f,  1.0f, 0.0f)) * shadowProj);*/
        
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
        shadowTransforms.Add(Matrix4.LookAt(lightPos, lightPos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);

        shadowShader.Use();
        for (int i = 0; i < 6; i++)
            shadowShader.SetMatrix($"shadowMatrices[{i}]", shadowTransforms[i]);
        shadowShader.SetFloat("far_plane", farPlane);
        shadowShader.SetVector3("lightPos", lightPos);
        
        // Configure the viewport to match the shadow resolution.
        GL.Viewport(0, 0, shadowResolution, shadowResolution);
        
        // 1. Render scene to depth cubemap
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
        GL.Clear(ClearBufferMask.DepthBufferBit);
        
        objects.RenderDepth(shadowShader);
        
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        return null;
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