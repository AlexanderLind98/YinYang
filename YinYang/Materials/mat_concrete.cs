using OpenTK.Mathematics;

namespace YinYang.Materials;

public class mat_concrete : Material
{
    public mat_concrete() : base()
    {
        uniforms.Add("material.diffTex", new Texture("Textures/blank.jpg"));
        uniforms.Add("material.specTex", new Texture("Textures/blank.jpg"));
        uniforms.Add("material.ambient", new Vector3(0.2f));
        uniforms.Add("material.diffuse", new Vector3(0.2f));
        uniforms.Add("material.specular", new Vector3(0.1f));
        uniforms.Add("material.shininess", 0.5f);
        
        UpdateUniforms();
    }
}

