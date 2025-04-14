using OpenTK.Mathematics;
using YinYang.Rendering;

namespace YinYang.Materials;

/// <summary>
/// A self-lit emissive material that does not receive or respond to lighting.
/// Used for glow effects, UI markers, or post-process inputs.
/// </summary>
public class mat_lightStone : Material
{
    public mat_lightStone() : base("Shaders/LitGeneric.vert", "Shaders/LightStone.frag")
    {
        uniforms.Add("color", new Vector3(1, 0.785f, 0.2f));
        uniforms.Add("glow", new Vector4(1, 0.7f, 0f,25.0f));
        UpdateUniforms();
    }

    /// <summary>
    /// This material does not support lighting. This method is intentionally left blank.
    /// </summary>
    public override void PrepareLighting(RenderContext context) { }
}