using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// Chrome material with high specular intensity and shininess.
/// </summary>
public class mat_water : Material
{
    public mat_water() : base("Shaders/LitGeneric.vert", "Shaders/Water.frag")
    {
        //uniform samplerCube environmentCubemap;
        uniforms.Add("waterMat.normTex", new Texture("Textures/Water_Normal_RFlip.png"));
        uniforms.Add("waterMat.color", new Vector3(0.5f, 0.8f, 1.0f));
        uniforms.Add("waterMat.tintColor", new Vector3(0.2f, 0.8f, 0.5f));
        uniforms.Add("time", 0.1f);

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
