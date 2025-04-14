// Downsampling pass inspired by: https://learnopengl.com/Guest-Articles/2022/Phys.-Based-Bloom
// Refactored to support multi-mip chain bloom for physically-based light diffusion.

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    /// <summary>
    /// Builds a chain of progressively smaller mip textures by downsampling an HDR brightness texture.
    /// Each mip is half the size of the previous one. The result is a blurred chain of textures,
    /// simulating how bright light diffuses in the eye or camera lens.
    /// </summary>
    public class BloomDownsamplePass : RenderPass
    {
        private readonly BloomMipChain mipChain;
        private readonly Shader downsampleShader;
        private readonly QuadMesh quad = new();

        public int InputTexture { get; set; }

        public BloomDownsamplePass(BloomMipChain mipChain)
        {
            this.mipChain = mipChain;
            downsampleShader = new Shader("shaders/fullscreen.vert", "shaders/PostProcessing/downsample.frag");
        }

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            // activete shader and set the input texture to 0
            downsampleShader.Use();
            downsampleShader.SetInt("srcTexture", 0);

            // Start with the original input texture
            int input = InputTexture;

            // For each mip level in the chain
            for (int i = 0; i < mipChain.Mips.Count; i++)
            {
                var mip = mipChain.Mips[i];

                // Set the viewport to the target mip resolution
                GL.Viewport(0, 0, mip.IntSize.X, mip.IntSize.Y);

                // Bind FBO and attach current mip texture
                mipChain.Bind();
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D,
                    mip.Texture, 0);

                // Send the source texture resolution to the shader for sampling
                downsampleShader.SetVector2("srcResolution", mip.Size);
                downsampleShader.SetInt("srcTexture", 0);

                // Bind the current input texture (previous mip)
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, input);

                quad.Draw();

                // The current mip becomes the input for the next iteration
                input = mip.Texture;
            }

            // Restore default framebuffer
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return null;
        }

        public override void Dispose() => downsampleShader.Dispose();
    }
}
