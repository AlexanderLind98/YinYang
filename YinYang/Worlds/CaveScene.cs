using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Materials;

namespace YinYang.Worlds;

public class CaveScene : World
{
    private GameObject cave;
    private GameObject CavePillar;
    private GameObject lightStone;

    public CaveScene(Game game) : base(game)
    {
        WorldName = game.Title + " Scene Test World";

        SkyColor = Color4.Black;
        lightingManager.Sun.ToggleLight();
        Editor = new EditorTool(this);
    }

    public override string DebugLabel
    {
        get
        {
            return Game.DebugMode switch
            {
                1 => "Shadowmap",
                _ => "Combined"
            };
        }
    }
    
    protected override void ConstructWorld()
    {
        base.ConstructWorld();
        
        cave = new GameObjectBuilder(Game)
            .ModelTBN("Cave/MainCave")
            // .Model("SmoothCube")
            .Material(new  mat_cliffStone())
            .Position(0f, 0, 0f)
            .Build();

        CavePillar = new GameObjectBuilder(Game)
            .ModelTBN("Cave/CavePillar")
            .Material(new mat_cliffStone())
            .Position(0, 0f, 0f)
            .Build();
        
        lightStone = new GameObjectBuilder(Game)
            .Model("Cave/LightStone")
            .Material(new mat_lightStone())
            .Position(5.75f, 0.7f, 2.98f)
            .RotationDegrees(40.0f, 45.8f, 11.45f)
            .Scale(1.37f, 1.37f, 1.37f)
            .Build();

        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-8.21f, -0.927f, -0.021f)
                .RotationDegrees(8.5f, 68.75f, -68.75f)
                .Scale(4.3f, 4.3f, 4.3f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(2.21f, -0.955f, -8.229f)
                .RotationDegrees(-0.0f, 252.101f, 0.0f)
                .Scale(4.04f, 4.04f, 4.04f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-1.32f, -0.499f, -7.663f)
                .RotationDegrees(17.188f, 154.698f, 0.0f)
                .Scale(2.25f, 2.25f, 2.25f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock001")
                .Material(new mat_GroundRock())
                .Position(0.07f, -0.23f, -6.18f)
                .RotationDegrees(22.91f, 263.56f, 34.377f)
                .Scale(4.31f, 4.31f, 4.31f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .Model("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(0.391f, 0.772f, -6.517f)
                .RotationDegrees(-34.377f, 40.1f, 0.0f)
                .Scale(0.66f, 0.66f, 0.66f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .Model("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-0.597f, 1.297f, -6.539f)
                .RotationDegrees(-28.647f, -51.566f, 0.0f)
                .Scale(1f, 1f, 1f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock003")
                .Material(new mat_GroundRock())
                .Position(-4.09f, -0.80f, 5.21f)
                .RotationDegrees(12.6f, -5.72f, 0.0f)
                .Scale(4.67f, 4.67f, 4.67f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock006")
                .Material(new mat_GroundRock())
                .Position(5.07f, -0.69f, 3.22f)
                .RotationDegrees(0.0f, -162.14f, 0.0f)
                .Scale(3.28f, 3.28f, 3.28f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(6.75f, -0.97f, 2.25f)
                .RotationDegrees(5.72f, -309.39f, 0.0f)
                .Scale(1.76f, 1.76f, 1.76f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.40f, -0.97f, 1.68f)
                .RotationDegrees(-57.29f, 108.86f, 0.0f)
                .Scale(0.53f, 0.53f, 0.53f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .Model("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.75f, -0.82f, 2.41f)
                .RotationDegrees(22.91f, -120.32f, 0.0f)
                .Scale(0.52f, 0.52f, 0.52f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.44f, -1.11f, 2.11f)
                .RotationDegrees(-34.37f, 61.30f, 0.0f)
                .Scale(0.22f, 0.22f, 0.22f)
                .Build());
        
        GameObject mirrorSphere =
            new GameObjectBuilder(Game)
                .ModelTBN("mask")
                .Material(new mat_mirror())
                .Position(0, 0, 0)
                .Scale(2,2,2)
                .Build();
        
        reflectionManager.AddProbe(mirrorSphere.Transform.Position);

        GameObjects.Add(mirrorSphere);
        GameObjects.Add(cave);
        GameObjects.Add(CavePillar);
        GameObjects.Add(lightStone);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();
        
        new PointLight(this, Color4.Goldenrod, 0.2f);
        PointLights[0].SetPosition(MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z);
        PointLights[0].shadowType = Light.ShadowType.Dynamic;

        new PointLight(this, Color4.Goldenrod, 0.2f);
        PointLights[1].SetPosition(-5.62f, -0.77f, 1.99f);

        new PointLight(this, Color4.Goldenrod, 0.2f);
        PointLights[2].SetPosition(5.83f, 0.60f, 2.92f);
        
        new PointLight(this, Color4.Goldenrod, 0.2f);
        PointLights[3].SetPosition(-0.50f, 1.55f, -6.28f);
    }

    public override void HandleInput(KeyboardState input)
    {
        base.HandleInput(input);
        
        PointLights[0].SetPosition(MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z);
        
        if (input.IsKeyPressed(Keys.D1))
        {
            Game.DebugMode = 1; // Ambient
        }

        if (input.IsKeyPressed(Keys.D2))
        {
            Game.DebugMode = 2; // Diffuse
        }

        if (input.IsKeyPressed(Keys.D3))
        {
            Game.DebugMode = 3; // Specular
        }

        if (input.IsKeyPressed(Keys.D4))
        {
            /*staticCube.Renderer.Material = new mat_gold_simple();
            staticCube.Renderer.Material.UpdateUniforms();

            rotatingCube.Renderer.Material = new mat_gold_simple();
            rotatingCube.Renderer.Material.UpdateUniforms();*/

            Game.DebugMode = 0; // Full lighting
        }

        if (input.IsKeyPressed(Keys.R))
        {
            lightingManager.Sun.ToggleLight();
        }

        Editor?.Update();
    }
}