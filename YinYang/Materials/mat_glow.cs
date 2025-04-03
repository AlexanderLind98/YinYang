using OpenTK.Mathematics;

namespace YinYang.Materials;

public class mat_glow : Material
{
    public mat_glow() : base("Shaders/UnlitGeneric.vert", "Shaders/UnlitGeneric.frag")
    {
        uniforms.Add("color", new Vector3(1));
        
        UpdateUniforms();
    }
}