using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// Chrome material with high specular intensity and shininess.
/// </summary>
public class mat_mirror : Material
{
    public mat_mirror() : base("Shaders/LitGeneric.vert", "Shaders/ReflectiveGeneric.frag")
    {
        //uniform samplerCube environmentCubemap;
        uniforms.Add("metallic", 1.0f);
        uniforms.Add("roughness", 0.99f);

        UpdateUniforms();
    }

    /// <summary>
    /// Set all lighting uniforms required by this chrome mat
    /// </summary>
    public override void PrepareLighting(RenderContext context)
    {
        LightingUniforms.ApplyStandardLighting(this, context);
    }
    
    public override bool IsReflective => true;
    public override bool UsesLighting => false;
}
