using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// A basic concrete material with minimal reflectivity.
/// Suitable for scenes that use lighting and shadow, but with low specular response.
/// </summary>
public class mat_BigRock : Material
{
    public mat_BigRock() : base()
    {
        uniforms.Add("material.diffTex", new Texture("Textures/BigRock_Diff.jpg"));
        uniforms.Add("material.specTex", new Texture("Textures/BigRock_Spec.jpg"));
        uniforms.Add("material.normTex", new Texture("Textures/BigRock_Norm.jpg"));
        uniforms.Add("material.ambient", new Vector3(0.2f));
        uniforms.Add("material.diffuse", new Vector3(0.2f));
        uniforms.Add("material.specular", new Vector3(0.7f));
        uniforms.Add("material.shininess", 10.0f);

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