using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Worlds;

namespace YinYang.Rendering
{
    /// <summary>
    /// Render pass responsible for generating cube maps for reflections.
    /// </summary>
    /// <remarks>
    /// Uses an object or location of a probe to render a cube map, used in reflections.
    /// </remarks>
    public class CubeReflectionRenderPass : RenderPass
    {
        private int framebufferHandle;
        private int reflectionResolution = 512;
        private Shader reflectionShader;
        private Texture reflectionCubeTexture;
        private float nearPlane = 0.1f;
        private float farPlane = 50.0f;
        
        // private Vector3 probePosition;
        
        private bool hasRenderedReflection = false;

        /// <summary>
        /// The cube texture produced by this pass.
        /// </summary>
        public Texture ReflectionCubeTexture => reflectionCubeTexture;

        /// <summary>
        /// Initializes the framebuffer, cube texture, and shader required for reflection rendering.
        /// </summary>
        public CubeReflectionRenderPass()
        {
            //Create shader and framebuffer
            reflectionShader = new Shader(
                  "Shaders/Reflection/ReflectCapture.vert",
                "Shaders/Reflection/ReflectCapture.frag",
                "Shaders/Reflection/ReflectCapture.geom");
            framebufferHandle = GL.GenFramebuffer();
            
            //Assign handle
            int textureHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, textureHandle);
            
            //Same as generating direct shadow map, but 6x for each face in cubemap
            for (int i = 0; i < 6; ++i)
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                    0,
                    PixelInternalFormat.Rgba16f,
                    reflectionResolution,
                    reflectionResolution,
                    0,
                    PixelFormat.Rgba,
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
            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, textureHandle, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            
            reflectionCubeTexture = new CubeTexture(textureHandle);
        }

        /// <summary>
        /// Executes the shadow pass by rendering the scene from the directional light's perspective.
        /// </summary>
        /// <remarks>
        /// Produces a light-space transformation matrix which is used in subsequent passes
        /// to compare fragment depth against shadow depth for shadow testing.
        /// </remarks>
        /// <param name="context">Rendering context containing vital data</param>
        /// <param name="objects">The object manager with all scene objects.</param>
        /// <returns>The computed light-space transformation matrix.</returns>
        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            /*for (int i = 0; i < context.Lighting.PointLights.Count; i++)
            {
                //Skip none shadowing lights
                if (hasRenderedShadow && context.Lighting.PointLights[i].shadowType == Light.ShadowType.None)
                {
                    continue;
                }
            }*/
            
            RenderReflection(context, objects); 

            return null;
        }

        private void RenderReflection(RenderContext context, ObjectManager objects)
        {
            // 0. create depth cubemap transformation matrices
            Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), reflectionResolution / reflectionResolution, nearPlane, farPlane);
            List<Matrix4> reflectionTransforms = new List<Matrix4>();
            Vector3 probePos = context.Reflection.probePositions[0]; //TODO: Hard coded for now                        
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);

            // prepare gl, set viewport before and bind framebuffer
            GL.Viewport(0, 0, reflectionResolution, reflectionResolution);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);
            
            reflectionShader.Use();
            // for (int i = 0; i < 6; i++)
            //     reflectionShader.SetMatrix($"reflectMatrices[{i}]", reflectionTransforms[i]);
            reflectionShader.SetFloat("far_plane", farPlane);
            reflectionShader.SetVector3("probePos", probePos);
            
            // loop trough sides, rendering each side of the cube map
            // bind each face of the cubemap to the framebuffer
            // use seperate rendercontext viewporj. to render each face
            for (int i = 0; i < 6; ++i)
            {
                // bind face of cubemap
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0,
                    TextureTarget.TextureCubeMapPositiveX + i,
                    reflectionCubeTexture.Handle,
                    0);

                // clear
                if(context.Reflection.reflectionType == ReflectionManager.ReflectionType.Static && !hasRenderedReflection)
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                else if(context.Reflection.reflectionType != ReflectionManager.ReflectionType.Static)
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Override ViewProjection matrix for this face
                var faceContext = new RenderContext
                {
                    Camera = context.Camera,
                    Lighting = context.Lighting,
                    World = context.World,
                    DebugMode = context.DebugMode,
                    BloomSettings = context.BloomSettings,
                    Reflection = context.Reflection,
                    LightSpaceMatrix = context.LightSpaceMatrix,
                    ViewProjection = reflectionTransforms[i]
                };

                // use the switch statement to determine the type of reflection rendering
                switch (context.Reflection.reflectionType)
                {
                    case ReflectionManager.ReflectionType.None:
                        break;
                    case ReflectionManager.ReflectionType.Static:
                        if (!hasRenderedReflection)
                            objects.Render(faceContext);
                        break;
                    case ReflectionManager.ReflectionType.Dynamic:
                        objects.Render(faceContext);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            hasRenderedReflection = true;
        }
        
        /*private void RenderReflection(RenderContext context, ObjectManager objects)
        {
            // 0. create depth cubemap transformation matrices
            Matrix4 shadowProj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90.0f), reflectionResolution / reflectionResolution, nearPlane, farPlane);
            List<Matrix4> reflectionTransforms = new List<Matrix4>();
            Vector3 probePos = context.Reflection.probePositions[0]; //TODO: Hard coded for now                        
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(1.0f, 0.0f, 0.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, 1.0f, 0.0f),  new Vector3(0.0f,  0.0f,  1.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f,  0.0f, -1.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, 0.0f, 1.0f),  new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);
            reflectionTransforms.Add(Matrix4.LookAt(probePos, probePos + new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f,  0.0f)) * shadowProj);

            reflectionShader.Use();
            for (int i = 0; i < 6; i++)
                reflectionShader.SetMatrix($"reflectMatrices[{i}]", reflectionTransforms[i]);
            reflectionShader.SetFloat("far_plane", farPlane);
            reflectionShader.SetVector3("probePos", probePos);
            
            // Configure the viewport to match the shadow resolution.
            GL.Viewport(0, 0, reflectionResolution, reflectionResolution);
            
            // 1. Render scene to depth cubemap
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebufferHandle);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            switch (context.Reflection.reflectionType)
            {
                case ReflectionManager.ReflectionType.None: break;
                case ReflectionManager.ReflectionType.Static:
                {
                    if(!hasRenderedReflection)
                        objects.Render(context);
                    break;
                }
                case ReflectionManager.ReflectionType.Dynamic:
                {
                    objects.Render(context);
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
            
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            hasRenderedReflection = true;
        }*/

        /// <summary>
        /// Releases GPU resources used by this render pass.
        /// </summary>
        public override void Dispose()
        {
            reflectionShader.Dispose();
            GL.DeleteFramebuffer(framebufferHandle);
        }
    }
}