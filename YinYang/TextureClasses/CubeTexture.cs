using OpenTK.Graphics.OpenGL4;

namespace YinYang;

public class CubeTexture : Texture
{
    public CubeTexture(int existingHandle)
    {
        handle = existingHandle;
    }
    
    public override void Use(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.TextureCubeMap, handle);
    }
}