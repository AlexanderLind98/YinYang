using OpenTK.Mathematics;
using YinYang.Components;
using YinYang.Materials;
using YinYang.Worlds;

namespace YinYang.Lights;

public class PointLight : Light
{
    public Transform Transform;
    
    public PointLight(World currentWorld)
    {
        Transform = new Transform();
        currentWorld.PointLights.Add(this);
        
        // CreateVisualizer(currentWorld);
    }

    public PointLight(World currentWorld, Color4 color, float intensity = 1)
    {
        Transform = new Transform();
        currentWorld.PointLights.Add(this);
        
        LightIntensity = intensity;
        LightColor = new Vector3(color.R * LightIntensity, color.G* LightIntensity, color.B* LightIntensity);
        DefaultColor = LightColor;
        
        // CreateVisualizer(currentWorld);
    }
    
    public override Vector3 SetPosition(float x, float y, float z)
    {
        Transform.Position = new Vector3(x, y, z);
        if (Visualizer != null) Visualizer.Transform.Position = Transform.Position;
        return new Vector3(x, y, z);
    }

    private void CreateVisualizer(World currentWorld)
    {
        Visualizer = new GameObjectBuilder(currentWorld.Game)
            .Model("Sphere")
            .Material(new mat_glow())
            .Position(Transform.Position.X, Transform.Position.Y, Transform.Position.Z)
            .Scale(0.1f, 0.1f, 0.1f)
            .Build();
        
        Visualizer.Renderer.RenderInDepthPass = false;

        currentWorld.GameObjects.Add(Visualizer);
    }
}