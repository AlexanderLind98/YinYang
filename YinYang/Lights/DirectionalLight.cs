using OpenTK.Mathematics;
using YinYang.Components;
using YinYang.Materials;
using YinYang.Worlds;

namespace YinYang.Lights;

public class DirectionalLight : Light
{
    public Transform Transform;

    // public DirectionalLight(World currentWorld)
    // {
    //     Transform = new Transform();
    //     currentWorld.DirectionalLight = this;
    //     
    //     CreateVisualizer(currentWorld);
    // }

    public DirectionalLight(World currentWorld, Color4 color, float intensity = 1)
    {
        Transform = new Transform();
        //currentWorld.DirectionalLight = this;
        
        LightIntensity = intensity;
        LightColor = new Vector3(color.R * LightIntensity, color.G* LightIntensity, color.B* LightIntensity);
        DefaultColor = LightColor;
        
        CreateVisualizer(currentWorld); //TODO maybe better to have a visualizer in the world or via lightingmanager
    }

    private void CreateVisualizer(World currentWorld)
    {
        Visualizer = new GameObjectBuilder(currentWorld.Game)
            .Model("Sphere")
            .Material(new mat_glow())
            .Position(0, 0, 0)
            .Scale(0.1f, 0.1f, 0.1f)
            .Build();

        Visualizer.Renderer.RenderInDepthPass = false;
        
        currentWorld.GameObjects.Add(Visualizer);
        
        Visualizer.Transform.Position = Transform.Position;
        Visualizer.Transform.Rotation = Transform.Rotation;
    }

    public void UpdateVisualizer(World currentWorld)
    {
        if (Visualizer != null) Visualizer.Transform.Rotation = Transform.Rotation;
    }

    public override Vector3 SetPosition(float x, float y, float z)
    {
        Transform.Position = new Vector3(x, y, z);
        if (Visualizer != null) Visualizer.Transform.Position = Transform.Position;
        return Transform.Position;
    }
}