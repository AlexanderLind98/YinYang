using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace YinYang
{
    public class Texture
    {
        protected int handle;
        public int Handle => handle;

        public Texture(string path)
        {
            // Generate a texture handle and bind it to TextureUnit 0.
            handle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, handle);

            // Ensure the image is flipped vertically to match OpenGL's coordinate system.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image data from file using STBImageSharp.
            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                // Upload the image data to the GPU.
                GL.TexImage2D(TextureTarget.Texture2D, 
                    level: 0, 
                    internalformat: PixelInternalFormat.Rgba, 
                    width: image.Width, 
                    height: image.Height, 
                    border: 0, 
                    format: PixelFormat.Rgba, 
                    type: PixelType.UnsignedByte, 
                    pixels: image.Data);
            }

            // Generate mipmaps for the texture.
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            // Set texture wrapping parameters.
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            // GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            // Set texture filtering parameters.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
        
        public Texture(int existingHandle)
        {
            handle = existingHandle;
        }

        protected Texture()
        {
            
        }

        /// <summary>
        /// Binds this texture to the specified texture unit (defaults to TextureUnit 0).
        /// </summary>
        public virtual void Use(TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
