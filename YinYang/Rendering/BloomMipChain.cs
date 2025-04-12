// a bloom mip chain is inspiret by this article: https://learnopengl.com/Guest-Articles/2022/Phys.-Based-Bloom

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Shapes;

/// <summary>
/// Represents a single mip level in the bloom chain.
/// Contains size info and texture handle.
/// </summary>
public class BloomMip
{
    public Vector2 Size;
    public Vector2i IntSize;
    public int Texture;
}

/// <summary>
/// Manages a chain of downsampled bloom mip levels and a shared framebuffer for rendering into them.
/// </summary>
public class BloomMipChain
{
    public int FBO { get; private set; }
    public List<BloomMip> Mips { get; private set; } = new();
    public int MipCount => Mips.Count;

    /// <summary>
    /// Initializes the mip chain with the specified number of downscaled levels.
    /// </summary>
    /// <param name="baseWidth">Initial width of the top-level mip (usually screen width)</param>
    /// <param name="baseHeight">Initial height of the top-level mip (usually screen height)</param>
    /// <param name="mipLevels">How many mips to generate (e.g., 5 = full, ½, ¼, ⅛, 1⁄16)</param>
    public bool Init(int baseWidth, int baseHeight, int mipLevels)
    {
        // Generate a shared framebuffer
        FBO = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

        // Start from full resolution
        Vector2 size = new Vector2(baseWidth, baseHeight);
        Vector2i intSize = new Vector2i(baseWidth, baseHeight);

        // iterate through the mip levels
        for (int i = 0; i < mipLevels; i++)
        {
            // Only scale down after the first mip
            if (i > 0)
            {
                size *= 0.5f;
                intSize /= 2;
            }

            // Create a floating point texture for HDR bloom (high precision color)
            int tex = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, tex);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, intSize.X, intSize.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);

            // Use linear filtering for blur and interpolation
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Clamp to edge to avoid border artifacts during blur
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);


            // Store this mip level
            Mips.Add(new BloomMip
            {
                Texture = tex,
                Size = size,
                IntSize = intSize
            });
        }

        // Unbind framebuffer after setup
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        return true;
    }

    /// <summary>
    /// Binds the shared framebuffer for rendering.
    /// </summary>
    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
    }

    /// <summary>
    /// Destroys all textures and the framebuffer. Call this when cleaning up.
    /// </summary>
    public void Dispose()
    {
        foreach (var mip in Mips)
            GL.DeleteTexture(mip.Texture);

        GL.DeleteFramebuffer(FBO);
    }
}