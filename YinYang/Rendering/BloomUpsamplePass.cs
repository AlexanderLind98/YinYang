// Upsampling pass inspired by: https://learnopengl.com/Guest-Articles/2022/Phys.-Based-Bloom
// Refactored to support mip-chain upsampling and additive blending for realistic bloom halos.

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

namespace YinYang.Rendering
{
    /// <summary>
    /// Performs additive upsampling of a bloom mip chain from smallest to largest resolution.
    /// Each mip is blurred and blended into the next larger mip level.
    /// This simulates soft glow bleeding across scales, similar to camera lens effects.
    /// </summary>
    public class BloomUpsamplePass : RenderPass
    {
        private readonly BloomMipChain mipChain;
        private readonly Shader upsampleShader;
        private readonly QuadMesh quad = new();

        public float FilterRadius = 0.005f;

        public BloomUpsamplePass(BloomMipChain mipChain)
        {
            this.mipChain = mipChain;
            upsampleShader = new Shader("shaders/fullscreen.vert", "shaders/upsample.frag");
        }

        public override Matrix4? Execute(RenderContext context, ObjectManager objects)
        {
            // activate and bind the default framebuffer
            upsampleShader.Use();
            upsampleShader.SetInt("srcTexture", 0);
            upsampleShader.SetFloat("filterRadius", FilterRadius);

            // Enable additive blending to accumulate bloom from lower mip levels (color = src + dst)
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.One, BlendingFactor.One);

            // Reverse Iterate through the mip chain from largest to smallest
            for (int i = mipChain.Mips.Count - 1; i > 0; i--)
            {
                // source mip is the lower-res mip
                var src = mipChain.Mips[i];
                //source mip is the higher-res mip
                var dst = mipChain.Mips[i - 1];

                // Set viewport to match the destination texture resolution
                GL.Viewport(0, 0, dst.IntSize.X, dst.IntSize.Y);

                // Bind shared framebuffer and attach destination mip
                mipChain.Bind();
                GL.FramebufferTexture2D(
                    FramebufferTarget.Framebuffer,
                    FramebufferAttachment.ColorAttachment0,
                    TextureTarget.Texture2D,
                    dst.Texture, 0);

                // Bind current source texture
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, src.Texture);

                quad.Draw();
            }


            // Disable blending and unbind framebuffer
            GL.Disable(EnableCap.Blend);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            return null;
        }

        public override void Dispose() => upsampleShader.Dispose();
    }
}
