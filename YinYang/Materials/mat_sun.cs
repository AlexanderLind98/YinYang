using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// A self-lit emissive material that does not receive or respond to lighting.
/// Used for glow effects, UI markers, or post-process inputs.
/// </summary>
public class mat_sun : Material
{
    public mat_sun() : base("Shaders/UnlitGeneric.vert", "Shaders/UnlitGeneric.frag")
    {
        uniforms.Add("color", new Vector3(25.0f, 20.0f, 5.0f));
        UpdateUniforms();
    }

    /// <summary>
    /// This material does not support lighting. This method is intentionally left blank.
    /// </summary>
    public override void PrepareLighting(RenderContext context) { }

    /// <summary>
    /// Prevents light uniforms from being set by default renderer logic.
    /// </summary>
    public override bool UsesLighting => false;
}