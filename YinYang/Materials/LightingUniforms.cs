using OpenTK.Mathematics;
using YinYang.Rendering;
using YinYang.Worlds;

namespace YinYang.Materials;

/// <summary>
/// Provides shared helper methods for applying lighting uniforms to materials.
/// </summary>
public static class LightingUniforms
{
    /// <summary>
    /// Applies all standard lighting uniforms to the given material,
    /// including sun, point lights, spot lights, and shadows.
    /// </summary>
    public static void ApplyStandardLighting(Material mat, RenderContext context)
    {
        var world = context.World;
        var camera = context.Camera;

        mat.SetUniform("lightSpaceMatrix", context.LightSpaceMatrix);
        mat.SetUniform("shadowMap", world.depthMap);
        mat.SetUniform("bloomThresholdMin", context.BloomSettings.BloomThresholdMin);
        mat.SetUniform("bloomThresholdMax", context.BloomSettings.BloomThresholdMax);
        
        // if there is a cube map, set the cube map and far plane
        if (context.World.depthCubeMap != null)
        {
            mat.SetUniform("cubeMap", context.World.depthCubeMap);
            mat.SetUniform("far_plane", 50.0f);
        }


        mat.SetUniform("dirLight.direction", world.DirectionalLight.Transform.Rotation);
        mat.SetUniform("dirLight.ambient", world.GetSkyColor() / 2);
        mat.SetUniform("dirLight.diffuse", world.DirectionalLight.LightColor);
        mat.SetUniform("dirLight.specular", world.DirectionalLight.LightColor);

        mat.SetUniform("numPointLights", world.PointLights.Count);
        for (int i = 0; i < world.PointLights.Count; i++)
        {
            var light = world.PointLights[i];
            mat.SetUniform($"pointLights[{i}].position", light.Transform.Position);
            mat.SetUniform($"pointLights[{i}].ambient", world.GetSkyColor() / 255);
            mat.SetUniform($"pointLights[{i}].diffuse", light.LightColor);
            mat.SetUniform($"pointLights[{i}].specular", light.LightColor);
            mat.SetUniform($"pointLights[{i}].constant", light.Constant);
            mat.SetUniform($"pointLights[{i}].linear", light.Linear);
            mat.SetUniform($"pointLights[{i}].quadratic", light.Quadratic);
        }

        mat.SetUniform("numSpotLights", world.SpotLights.Count);
        for (int i = 0; i < world.SpotLights.Count; i++)
        {
            var light = world.SpotLights[i];
            if (i == 0)
            {
                mat.SetUniform($"spotLights[{i}].position", camera.Position);
                mat.SetUniform($"spotLights[{i}].direction", Vector3.Normalize(camera.Front));
            }
            else
            {
                mat.SetUniform($"spotLights[{i}].position", light.Transform.Position);
                mat.SetUniform($"spotLights[{i}].direction", light.Transform.Rotation);
            }

            mat.SetUniform($"spotLights[{i}].cutOff", (float)MathHelper.Cos(MathHelper.DegreesToRadians(light.InnerRadius)));
            mat.SetUniform($"spotLights[{i}].outerCutOff", (float)MathHelper.Cos(MathHelper.DegreesToRadians(light.OuterRadius)));
            mat.SetUniform($"spotLights[{i}].ambient", world.GetSkyColor() / 255);
            mat.SetUniform($"spotLights[{i}].diffuse", light.LightColor);
            mat.SetUniform($"spotLights[{i}].specular", light.LightColor);
            mat.SetUniform($"spotLights[{i}].constant", light.Constant);
            mat.SetUniform($"spotLights[{i}].linear", light.Linear);
            mat.SetUniform($"spotLights[{i}].quadratic", light.Quadratic);
        }
    }
}
