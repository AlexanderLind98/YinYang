using OpenTK.Mathematics;
using YinYang.Components;
using YinYang.Materials;
using YinYang.Worlds;

namespace YinYang.Lights;

public class SpotLight : Light
{
    public Transform Transform;
    public float InnerRadius = 12.5f;
    public float OuterRadius = 15.5f;
    
    public SpotLight(World currentWorld)
    {
        Transform = new Transform();
        currentWorld.SpotLights.Add(this);
        
        CreateVisualizer(currentWorld);
    }
    
    public SpotLight(World currentWorld, Color4 color, float intensity = 1, float innerRadius = 12.5f, float outerRadius = 15.5f)
    {
        Transform = new Transform();
        currentWorld.SpotLights.Add(this);
        
        LightIntensity = intensity;
        LightColor = new Vector3(color.R * LightIntensity, color.G* LightIntensity, color.B* LightIntensity);
        DefaultColor = LightColor;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        
        CreateVisualizer(currentWorld);
    }
    
    public SpotLight(World currentWorld, Color4 color, Vector3 position, Vector3 rotation, float intensity = 1, float innerRadius = 12.5f, float outerRadius = 15.5f)
    {
        Transform = new Transform
        {
            Position = position,
            Rotation = rotation
        };

        currentWorld.SpotLights.Add(this);
        
        LightIntensity = intensity;
        LightColor = new Vector3(color.R * LightIntensity, color.G* LightIntensity, color.B* LightIntensity);
        DefaultColor = LightColor;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        
        CreateVisualizer(currentWorld);
    }

    public SpotLight(World currentWorld, Color4 color, Transform transform, float intensity = 1, float innerRadius = 12.5f, float outerRadius = 15.5f)
    {
        Transform = transform;
        currentWorld.SpotLights.Add(this);
        
        LightIntensity = intensity;
        LightColor = new Vector3(color.R * LightIntensity, color.G* LightIntensity, color.B* LightIntensity);
        DefaultColor = LightColor;
        InnerRadius = innerRadius;
        OuterRadius = outerRadius;
        
        CreateVisualizer(currentWorld);
    }
    
    protected void CreateVisualizer(World currentWorld)
    {
        if (currentWorld.SpotLights.Count == 1)
            return;

        Visualizer = new GameObjectBuilder(currentWorld.Game)
            .Model("Arrow")
            .Material(new mat_concrete())
            .Position(0, 0, 0)
            .Scale(0.1f, 0.1f, 0.1f)
            .Build();

        currentWorld.GameObjects.Add(Visualizer);

        // Apply the direction to the visualizer arrow (convert direction to Euler angles)
        Visualizer.Transform.Position = Transform.Position;
        Visualizer.Transform.Rotation = new Vector3(Transform.Rotation.X, Transform.Rotation.Y - (float)Math.PI / 2, Transform.Rotation.Z);
    }
    
    public void UpdateVisualizer(World currentWorld)
    {
        if (Visualizer != null)
        {
            Visualizer.Transform.Position = Transform.Position;
            Visualizer.Transform.Rotation = Transform.Rotation;
        }
    }
}