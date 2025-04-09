using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// Chrome material with high specular intensity and shininess.
/// </summary>
public class mat_chrome : Material
{
    public mat_chrome() : base()
    {
        uniforms.Add("material.diffTex", new Texture("Textures/blank.jpg"));
        uniforms.Add("material.specTex", new Texture("Textures/blank.jpg"));
        uniforms.Add("material.ambient", new Vector3(0.4f));
        uniforms.Add("material.diffuse", new Vector3(0.6f));
        uniforms.Add("material.specular", new Vector3(0.774f));
        uniforms.Add("material.shininess", 96.8f);

        UpdateUniforms();
    }

    /// <summary>
    /// Set all lighting uniforms required by this chrome mat
    /// </summary>
    public override void PrepareLighting(RenderContext context)
    {
        LightingUniforms.ApplyStandardLighting(this, context);
    }
}
