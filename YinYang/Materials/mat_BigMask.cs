using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// A basic concrete material with minimal reflectivity.
/// Suitable for scenes that use lighting and shadow, but with low specular response.
/// </summary>
public class mat_BigMask : Material
{
    public mat_BigMask() : base()
    {
        uniforms.Add("material.diffTex", new Texture("Textures/StoneMask_Diffuse.png"));
        uniforms.Add("material.specTex", new Texture("Textures/StoneMask_Specular.png"));
        uniforms.Add("material.normTex", new Texture("Textures/StoneMask_Normal.png"));
        uniforms.Add("material.ambient", new Vector3(0.2f));
        uniforms.Add("material.diffuse", new Vector3(0.2f));
        uniforms.Add("material.specular", new Vector3(0.7f));
        uniforms.Add("material.shininess", 75.0f);

        UpdateUniforms();
    }

    /// <summary>
    /// Uses standard lighting setup for simple diffuse material.
    /// Delegates to base implementation, unless specialized lighting is needed.
    /// </summary>
    public override void PrepareLighting(RenderContext context)
    {
        LightingUniforms.ApplyStandardLighting(this, context);
    }
}