using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Materials;
using YinYang.Particles;

namespace YinYang.Worlds;

public class CaveScene : World
{
    private GameObject cave;
    private GameObject CavePillar;
    private GameObject lightStone;
    private GameObject mask;

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
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.44f, -1.11f, 2.11f)
                .RotationDegrees(-34.37f, 61.30f, 0.0f)
                .Scale(0.22f, 0.22f, 0.22f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-2.07f, 2.11f, -12.38f)
                .RotationDegrees(0.0f, 17.18f, 97.4f)
                .Scale(5.75f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock005")
                .Material(new mat_GroundRock())
                .Position(-5.27f, -0.65f, -14.85f)
                .RotationDegrees(-2.86f, 17.18f, 1.14f)
                .Scale(4.86f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(0.72f, 1.24f, -18.15f)
                .RotationDegrees(22.91f, 0f, 0f)
                .Scale(1.23f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.09f, 1.21f, -29.72f)
                .RotationDegrees(-57.29f, 509.93f, 0f)
                .Scale(0.45f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock006")
                .Material(new mat_GroundRock())
                .Position(-5.08f, 0.10f, -30.42f)
                .RotationDegrees(0f, 154.69f, 0f)
                .Scale(5f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-5.019f, 0.17f, -30.75f)
                .RotationDegrees(0f, 0f, 0f)
                .Scale(1.85f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-5.46f, -0.2f, -29.82f)
                .RotationDegrees(0f, -171.88f, 0f)
                .Scale(1.73f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-18.41f, -1.94f, -29.75f)
                .RotationDegrees(34.37f, -120.32f, 0f)
                .Scale(6.15f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(0.93f, -0.55f, -18.18f)
                .RotationDegrees(0, 0f, 0f)
                .Scale(1.87f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-2.65f, -0.41f, -31.70f)
                .RotationDegrees(0, -143.23f, 0f)
                .Scale(2.78f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(14.35f, 10.07f, -28.14f)
                .RotationDegrees(-143.23f, -97.40f, -11.45f)
                .Scale(6.73f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(15.98f, 5.11f, -31.63f)
                .RotationDegrees(0, 217.72f, 74.48f)
                .Scale(5.29f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(11.13f, 6.58f, -26.94f)
                .RotationDegrees(34.37f, -63.02f, 0f)
                .Scale(1f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(10.88f, 7.23f, -27.75f)
                .RotationDegrees(-45.83f, 57.29f, 0f)
                .Scale(0.64f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock001")
                .Material(new mat_GroundRock())
                .Position(13.18f, 0.26f, -25.40f)
                .RotationDegrees(-3.43f, -1.71f, 3.43f)
                .Scale(6.2f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock002")
                .Material(new mat_GroundRock())
                .Position(13.27f, 0.33f, -27.50f)
                .RotationDegrees(0, 252.10f, 0f)
                .Scale(5.33f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(12.62f, 0.09f, -29.41f)
                .RotationDegrees(-4.01f, 83.65f, -14.32f)
                .Scale(1.18f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(11.66f, -0.24f, -30.28f)
                .RotationDegrees(0f, 103.13f, 0f)
                .Scale(1.78f)
                .Build());
        
        mask =
            new GameObjectBuilder(Game)
                .ModelTBN("mask")
                .Material(new mat_mirror())
                .Position(-2.08f, 0.436f, -29.80f)
                .RotationDegrees(28.64f, 0f, 0f)
                .Scale(2,2,2)
                .Build();

        GameObjects.Add(mask);
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
        
        reflectionManager.AddProbe(mask.Transform.Position);
        
        var magicParticles = new GameObject(Game);
        // over first lightstone
        //magicParticles.Transform.Position = new Vector3(5.75f, 0.7f, 2.98f); 
        magicParticles.Transform.Position = new Vector3(0,0,0); 
        // 5000 partikler
        magicParticles.AddComponent<MagicParticleSystem>(500); 
        GameObjects.Add(magicParticles);

    }

    public override void HandleInput(KeyboardState input)
    {
        base.HandleInput(input);
        
        PointLights[0].SetPosition(MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z);
        // reflectionManager.probePositions[0] = mask.Transform.Position;
        
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
            Game.DebugMode = 0; // Full lighting
        }

        if (input.IsKeyPressed(Keys.R))
        {
            lightingManager.Sun.ToggleLight();
        }

        Editor?.Update();
    }
}