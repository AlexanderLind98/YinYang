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

            // Load compute shader from file
            computeShader = new Shader("Shaders/volumetricLight.comp", ShaderType.ComputeShader);

            // Create 2D floating-point texture for storing volumetric result
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
        /// <param name="context">Rendering context containing camera, lighting, matrices, etc.</param>
        /// <param name="objects">Scene object manager containing renderable objects.</param>
        /// <returns>Returns the input light-space matrix unmodified.</returns>
        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            computeShader.Use();

            // Bind the output image the compute shader will write to (binding = 0)
            GL.BindImageTexture(0, volumetricTexture, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba16f);

            // Get view and projection matrices and combine to form world-space to clip-space transform
            Matrix4 view = context.Camera.GetView();
            Matrix4 proj = context.Camera.GetProjection();
            // this order is opposit, but it workds, maybe because of the shader being a compute shader
            Matrix4 viewProjection = proj * view; 
            // Used to reconstruct rays from screen-space
            Matrix4 inverseViewProj = Matrix4.Invert(viewProjection); 
            
            // Console.WriteLine("----- Volumetric Debug -----");
            // Console.WriteLine($"Camera Pos = {context.Camera.Position}");
            // Vector3 degrees = context.World.DirectionalLight.Transform.GetRotationInDegrees();
            // Console.WriteLine($"Sun Euler (degrees) = {degrees}");
            // Vector3 forward = context.World.DirectionalLight.GetDirectionFromDegrees(degrees);
            // Console.WriteLine($"Sun Forward = {forward}");
            // Console.WriteLine($"viewProjection = {viewProjection}");
            // Console.WriteLine($"inverseViewProjection = {inverseViewProj}");

            // Send all camera-related uniforms to shader
            computeShader.SetMatrix("viewProjection", viewProjection);
            computeShader.SetMatrix("inverseProjection", inverseViewProj);
            computeShader.SetVector3("cameraPos", context.Camera.Position);

            // Calculate sun direction in world space from its Euler rotation
            var sunRot = context.World.DirectionalLight.Transform.GetRotationInDegrees();
            var sunDir = context.World.DirectionalLight.GetDirectionFromDegrees(sunRot);
            computeShader.SetVector3("lightDir", sunDir);
            computeShader.SetVector3("lightColor", context.World.DirectionalLight.LightColor);

            // Set raymarching and scattering parameters
            computeShader.SetFloat("density", 0.04f);  // ,1
            computeShader.SetFloat("scatteringStrength", 8.0f); //,2
            computeShader.SetInt("stepCount", 256);
            computeShader.SetFloat("nearPlane", 0.1f);
            computeShader.SetFloat("farPlane", 50.0f);

            // Bind the depth map for occlusion testing in raymarching
            GL.ActiveTexture(TextureUnit.Texture0);
            context.World.depthMap.Use();
            computeShader.SetInt("depthMap", 0);

            // Launch compute shader: 1 thread per 8x8 block of pixels
            GL.DispatchCompute((resolutionX + 7) / 8, (resolutionY + 7) / 8, 1);

            // Ensure the shader writes are visible before other passes use the texture
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