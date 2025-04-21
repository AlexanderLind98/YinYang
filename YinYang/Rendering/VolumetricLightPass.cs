using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;

namespace YinYang.Rendering
{
    /// <summary>
    /// Compute shader-based volumetric lighting pass.
    /// Dispatches a raymarching shader that accumulates light scattering from the sun.
    /// </summary>
    public class VolumetricLightPass : RenderPass
    {
        private Shader computeShader;
        private int volumetricTexture;
        private int resolutionX, resolutionY;

        /// <summary>
        /// Handle to the output texture containing the volumetric scattering result.
        /// </summary>
        public int VolumetricTexture => volumetricTexture;

        /// <summary>
        /// Initializes the compute shader and allocates the target texture.
        /// </summary>
        public VolumetricLightPass(int screenWidth, int screenHeight)
        {
            resolutionX = screenWidth;
            resolutionY = screenHeight;

            computeShader = new Shader("Shaders/volumetricLight.comp", ShaderType.ComputeShader);

            volumetricTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, volumetricTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f,
                screenWidth, screenHeight, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        /// <summary>
        /// Executes the volumetric light raymarching pass.
        /// </summary>
        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            computeShader.Use();

            // Bind output image (binding = 0 in compute shader)
            GL.BindImageTexture(0, volumetricTexture, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);

            // Provide required matrix uniforms
            Matrix4 view = context.Camera.GetView();
            Matrix4 proj = context.Camera.GetProjection();
            Matrix4 inverseViewProj = Matrix4.Invert(view * proj);
            computeShader.SetMatrix("inverseViewProjection", inverseViewProj);
            computeShader.SetMatrix("lightSpaceMatrix", context.LightSpaceMatrix);

            // Set camera and lighting parameters
            computeShader.SetVector3("cameraPos", context.Camera.Position);
            Vector3 sunDir = context.Lighting.Sun.GetDirectionFromDegrees(
                context.Lighting.Sun.Transform.GetRotationInDegrees()
            );
            computeShader.SetVector3("lightDir", sunDir);
            computeShader.SetVector3("lightColor", new Vector3(1.0f, 0.9f, 0.5f));

            // Raymarch parameters
            computeShader.SetFloat("density", 0.04f);
            computeShader.SetFloat("scatteringStrength", 8.0f);
            computeShader.SetInt("stepCount", 256);
            computeShader.SetFloat("nearPlane", 0.1f);
            computeShader.SetFloat("farPlane", 50.0f);

            // Bind depth map (used for visibility in light space)
            GL.ActiveTexture(TextureUnit.Texture0);
            context.World.depthMap.Use();
            computeShader.SetInt("depthMap", 0);

            // Dispatch one thread group per 8x8 tile of pixels
            GL.DispatchCompute((resolutionX + 7) / 8, (resolutionY + 7) / 8, 1);

            // Ensure writes are visible to subsequent passes
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            return context.LightSpaceMatrix;
        }

        /// <summary>
        /// Releases GPU resources used by this pass.
        /// </summary>
        public override void Dispose()
        {
            computeShader.Dispose();
            GL.DeleteTexture(volumetricTexture);
        }
    }
}
