using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Behaviors.CollisionEvents;
using YinYang.Behaviors.Motion;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Materials;
using YinYang.Particles;

namespace YinYang.Worlds;

public class CaveScene2 : World
{
    private GameObject cave;
    private GameObject CavePillar;
    private GameObject lightStone;
    private GameObject mask;
    private GameObject collider;
    private GameObject water;
    private GameObject waterfall;

    private int camKeyFrame = 0;

    private Vector3 sunPos = new(58.65f, 55.80f, 5.0f);

    public CaveScene2(Game game) : base(game)
    {
        WorldName = game.Title + " Cave Scene";
        
        DirectionalLight.SetRotationInDegrees(-17.12f, -13.68f, -9.05f);
        // DirectionalLight.LightColor = new Vector3(5, 5, 2.5f);
        SunColor = new Vector3(0);
        // DirectionalLight.UpdateDefaultColor();
        lightingManager.Sun.ToggleLight();

        SkyColor = Color4.Black;
        DirectionalLight.Transform.Position = new Vector3(-14.29f, 17.64f, -47.01f);
        // DirectionalLight.UpdateVisualizer(this);
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
        
        DirectionalLight.Visualizer.Transform.Position = sunPos;
        DirectionalLight.Visualizer.Transform.Scale = new Vector3(2.5f);
        
        cave = new GameObjectBuilder(Game)
            .ModelTBN("Cave/CaveNew")
            .Material(new  mat_cliffStone())
            .Position(0f, 0, 0f)
            .Build();

        CavePillar = new GameObjectBuilder(Game)
            .ModelTBN("Cave/CavePillar")
            .Material(new mat_cliffStone())
            .Position(0, 0f, 0f)
            .Build();
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigMask")
                .Material(new mat_BigMask())
                .Position(0f, 0f, 0f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/CaveExit")
                .Material(new mat_cliffStone())
                .Position(0f, 0f, 0f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/CliffOutdoors")
                .Material(new mat_cliffStone())
                .Position(0f, 0f, 0f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/CliffOutdoors01")
                .Material(new mat_cliffStone())
                .Position(0f, 0f, 0f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/SandGround")
                .Material(new mat_Sand())
                .Position(0f, 0f, 0f)
                .Build());
        
        water =
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/Water_Plane")
                .Material(new mat_water())
                .Position(0f, 0f, 0f)
                .Build();
        
        lightStone = new GameObjectBuilder(Game)
            .Model("Cave/LightStone")
            .Material(new mat_lightStone())
            .Position(5.75f, 0.7f, 2.98f)
            .RotationDegrees(-40.0f, 45.8f, 11.45f)
            .Scale(1.37f, 1.37f, 1.37f)
            .Build();

        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(-8.21f, -0.927f, -0.021f)
                .RotationDegrees(-8.5f, 68.75f, -68.75f)
                .Scale(4.3f, 4.3f, 4.3f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(2.21f, -0.955f, -8.229f)
                .RotationDegrees(0.0f, 252.101f, 0.0f)
                .Scale(4.04f, 4.04f, 4.04f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/BigRock")
                .Material(new mat_BigRock())
                .Position(1.32f, -0.499f, -7.663f)
                .RotationDegrees(17.188f, 154.698f, 0.0f)
                .Scale(2.25f, 2.25f, 2.25f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock001")
                .Material(new mat_GroundRock())
                .Position(0.07f, -0.23f, -6.18f)
                .RotationDegrees(-22.91f, 263.56f, 34.377f)
                .Scale(4.31f, 4.31f, 4.31f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .Model("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(0.391f, 0.772f, -6.517f)
                .RotationDegrees(34.377f, 40.1f, 0.0f)
                .Scale(0.66f, 0.66f, 0.66f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .Model("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-0.597f, 1.297f, -6.539f)
                .RotationDegrees(28.647f, -51.566f, 0.0f)
                .Scale(1f, 1f, 1f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock003")
                .Material(new mat_GroundRock())
                .Position(-4.09f, -0.80f, 5.21f)
                .RotationDegrees(-12.6f, -5.72f, 0.0f)
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
                .RotationDegrees(-5.72f, -309.39f, 0.0f)
                .Scale(1.76f, 1.76f, 1.76f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.40f, -0.97f, 1.68f)
                .RotationDegrees(57.29f, 108.86f, 0.0f)
                .Scale(0.53f, 0.53f, 0.53f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .Model("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.75f, -0.82f, 2.41f)
                .RotationDegrees(-22.91f, -120.32f, 0.0f)
                .Scale(0.52f, 0.52f, 0.52f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.44f, -1.11f, 2.11f)
                .RotationDegrees(34.37f, 61.30f, 0.0f)
                .Scale(0.22f, 0.22f, 0.22f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.44f, -1.11f, 2.11f)
                .RotationDegrees(34.37f, 61.30f, 0.0f)
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
                .RotationDegrees(2.86f, 17.18f, 1.14f)
                .Scale(4.86f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(0.72f, 1.24f, -18.15f)
                .RotationDegrees(-22.91f, 0f, 0f)
                .Scale(1.23f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(-5.09f, 1.21f, -29.72f)
                .RotationDegrees(57.29f, 509.93f, 0f)
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
                .RotationDegrees(-34.37f, -120.32f, 0f)
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
                .RotationDegrees(143.23f, -97.40f, -11.45f)
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
                .RotationDegrees(-34.37f, -63.02f, 0f)
                .Scale(1f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/LightStone")
                .Material(new mat_lightStone())
                .Position(10.88f, 7.23f, -27.75f)
                .RotationDegrees(45.83f, 57.29f, 0f)
                .Scale(0.64f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/GroundRock001")
                .Material(new mat_GroundRock())
                .Position(13.18f, 0.26f, -25.40f)
                .RotationDegrees(3.43f, -1.71f, 3.43f)
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
                .RotationDegrees(4.01f, 83.65f, -14.32f)
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
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-56.61f, -7.57f, -45.20f)
                .RotationDegrees(-40.10f, -80.21f, 0f)
                .Scale(5.54f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-57.40f, -9.39f, -51.38f)
                .RotationDegrees(0f, 10.59f, 0f)
                .Scale(10.59f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-51.37f, -8.62f, -35.14f)
                .RotationDegrees(28.64f, 126.05f, 0f)
                .Scale(15.91f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-57.49f, -8.15f, -69.14f)
                .RotationDegrees(-11.45f, -103.13f, 17.18f)
                .Scale(5.35f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-25.73f, -7.96f, -39.54f)
                .RotationDegrees(-22.91f, -11.45f, 11.45f)
                .Scale(9f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-21.31f, -6.79f, -84.70f)
                .RotationDegrees(45.83f, 0f, 0f)
                .Scale(10.73f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-61.82f, 11.03f, -72.88f)
                .RotationDegrees(-11.45f, -103.13f, 17.18f)
                .Scale(5.35f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-60.32f, 10.47f, -42.91f)
                .RotationDegrees(0f, 0f, -22.91f)
                .Scale(7.43f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-26.83f, 5.46f, -30.63f)
                .RotationDegrees(0f, 0f, 0f)
                .Scale(2.97f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-44.83f, 6.28f, -29.81f)
                .RotationDegrees(0f, 177.61f, 0f)
                .Scale(7.06f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-0.74f, -6.46f, -52.06f)
                .RotationDegrees(0f, 126.05f, 18.20f)
                .Scale(18.20f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock02")
                .Material(new mat_MossRock02())
                .Position(-13.17f, -8.56f, -80.74f)
                .RotationDegrees(0f, -74.48f, -80.74f)
                .Scale(15.07f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-13.17f, -8.56f, -80.74f)
                .RotationDegrees(0f, -74.48f, 0f)
                .Scale(15.07f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-6.39f, -7.44f, -64.03f)
                .RotationDegrees(0f, 103.13f, -64.06f)
                .Scale(8.50f)
                .Build());
        
        GameObjects.Add(
            new GameObjectBuilder(Game)
                .ModelTBN("Cave/MossRock01")
                .Material(new mat_MossRock01())
                .Position(-8.83f, -8.63f, -70.62f)
                .RotationDegrees(0f, 0f, 0f)
                .Scale(9.18f)
                .Build());
        
        waterfall =
            new GameObjectBuilder(Game)
                .Model("Cave/Temple_Waterfall")
                .Material(new mat_waterfall())
                .Position(0f, 0f, 0f)
                .Build();
        
        mask =
            new GameObjectBuilder(Game)
                .ModelTBN("mask")
                .Material(new mat_mirror())
                .Position(-2.08f, 0.436f, -29.80f)
                .RotationDegrees(-28.64f, 0f, 0f)
                .Scale(2,2,2)
                .Build();
        
        mask.Renderer.RenderInReflectionPass = false;
        water.Renderer.RenderInReflectionPass = false;
        waterfall.Renderer.RenderInReflectionPass = false;

        collider = new GameObject(Game);
        collider.Transform.Position = new Vector3(-30, 5.18f, -16f);
        collider.AddComponent<DistCollider>();

        GameObjects.Add(mask);
        GameObjects.Add(water);
        GameObjects.Add(waterfall);
        GameObjects.Add(cave);
        GameObjects.Add(CavePillar);
        GameObjects.Add(lightStone);
        GameObjects.Add(collider);
        
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

        Vector3 maskReflectionLoc = mask.Transform.Position + (Vector3.UnitZ * 2.0f);
        reflectionManager.AddProbe(maskReflectionLoc);
        
        PlaceParticles();
        
        //MoveCam();
    }
    
    private void PlaceParticles()
    {
        var magicParticlesRight = new GameObject(Game);
        magicParticlesRight.Transform.Position = new Vector3(5.75f, 0.7f, 3.0f);
        magicParticlesRight.AddComponent<MagicParticleSystem>(100); 
        GameObjects.Add(magicParticlesRight);
        
        var magicParticlesFront = new GameObject(Game);
        magicParticlesFront.Transform.Position = new Vector3(0.4f, 0.7f, -6.5f);
        magicParticlesFront.AddComponent<MagicParticleSystem>(100); 
        GameObjects.Add(magicParticlesFront);
        
        var magicParticlesLeft = new GameObject(Game);
        magicParticlesLeft.Transform.Position = new Vector3(-5.5f, -1.0f, 1.5f);
        magicParticlesLeft.AddComponent<MagicParticleSystem>(100); 
        GameObjects.Add(magicParticlesLeft);
        
        var magicParticlesCenter = new GameObject(Game);
        magicParticlesCenter.Transform.Position = new Vector3(-0.7f, 1.25f, -18.2f);
        magicParticlesCenter.AddComponent<MagicParticleSystem>(100); 
        GameObjects.Add(magicParticlesCenter);
        
        var magicParticlesMask = new GameObject(Game);
        magicParticlesMask.Transform.Position = new Vector3(-5.1f, 1.2f, -30.0f);
        magicParticlesMask.AddComponent<MagicParticleSystem>(100); 
        GameObjects.Add(magicParticlesMask);
        
        var magicParticlesCorner = new GameObject(Game);
        magicParticlesCorner.Transform.Position = new Vector3(11.13f, 6.5f, -27.0f);
        magicParticlesCorner.AddComponent<MagicParticleSystem>(100); 
        GameObjects.Add(magicParticlesCorner);
        
        //
        var WaterfallParticlesRight = new GameObject(Game);
        WaterfallParticlesRight.Transform.Position = new Vector3(-52.3f, -8.0f, -65.7f);
        WaterfallParticlesRight.AddComponent<WaterfallMistParticleSystem>(10000);
        GameObjects.Add(WaterfallParticlesRight);
        
        var WaterfallParticlesMid = new GameObject(Game);
        WaterfallParticlesMid.Transform.Position = new Vector3(-48, -8.0f, -57.4f); 
        WaterfallParticlesMid.AddComponent<WaterfallMistParticleSystem>(10000);
        GameObjects.Add(WaterfallParticlesMid);
        
        var WaterfallParticlesLeft = new GameObject(Game);
        WaterfallParticlesLeft.Transform.Position = new Vector3(-52, -8.0f, -49.5f); 
        WaterfallParticlesLeft.AddComponent<WaterfallMistParticleSystem>(10000);
        GameObjects.Add(WaterfallParticlesLeft);
    }

    public override void HandleInput(KeyboardState input)
    {
        base.HandleInput(input);
        
        PointLights[0].SetPosition(MainCamera.Position.X, MainCamera.Position.Y, MainCamera.Position.Z);
        // reflectionManager.probePositions[0] = mask.Transform.Position;
        
        DirectionalLight.Visualizer.Transform.Position = sunPos + MainCamera.Position;
        
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

        if (input.IsKeyPressed(Keys.End))
        {
            Console.WriteLine(camKeyFrame + " Pos is: " + MainCamera.Position);
            Console.WriteLine(camKeyFrame + " Forward is: " + MainCamera.Front);

            camKeyFrame++;
        }

        Editor?.Update();
    }
    
     private void MoveCam()
    {
        var maskPos = new Vector3(-2.08f, 0.436f, -29.80f);
        
        var cameraObj = cameraManager.Camera.GameObject;

        // set initial
        cameraObj.SetRotationInDegrees(0, -90, 0);
        
        // move sequence
        cameraObj.AddComponent<SequentialBehavior>
        (
            new LogMotion("Starting CamMove"),
            new WaitXSeconds(1f),

            new LogMotion("Looking side to side"),
            new SequentialMotion
            (
                new TurnXYZDegrees(new Vector3(0, -60, 0), 1.5f),
                new WaitXSeconds(0.5f),
                new TurnXYZDegrees(new Vector3(0, 120, 0), 1.5f),
                new WaitXSeconds(1.0f)
            ),
            
            new LogMotion("Turning forward again"),
            new TurnXYZDegrees(new Vector3(0, -60, 0), 1.0f),
            new WaitXSeconds(0.2f),

            new LogMotion("Steps 1–3: move to mask while turning to look at it"),
            new SequentialMotion(
                new LogMotion("Step 1"),
                new MoveAndLookAt(
                    new Vector3(4.38f, 0.61f, -1.84f),
                    new Vector3(-2.08f, 0.436f, -29.80f),
                    1.25f
                ),
                new LogMotion("Step 2"),
                new MoveAndLookAt(
                    new Vector3(11.10f, 1.47f, -8.69f),
                    new Vector3(-2.08f, 0.436f, -29.80f),
                    1.5f
                ),
                new LogMotion("Step 3"),
                new MoveAndLookAt(
                    new Vector3(5.65f, 1.82f, -21.28f),
                    new Vector3(-2.08f, 0.436f, -29.80f),
                    2f
                )
            ),

           
                new LogMotion("Step 4"),
                new MoveToPositionXYZ(new Vector3(1.21f, 2.67f, -26.63f), 2f),
           
            
            new LogMotion("step 5-7, Looking at mask"), // pos (-2.08f, 0.436f, -29.80f);
            new SequentialMotion
            (
                new LogMotion("Step 5"),
                new MoveAndLookAt(
                    new Vector3(-1.82f, 2.67f, -25.35f),
                    new Vector3(-2.08f, 0.436f, -29.80f),
                    2f
                ),
                new LogMotion("Step 6"),
                new MoveAndLookAt(
                    new Vector3(-5.17f, 2.67f, -26.16f),
                    new Vector3(-2.08f, 0.436f, -29.80f),
                    2f
                ),
                new LogMotion("Step 7"),
                new MoveAndLookAt(
                    new Vector3(-11.22f, 2.39f, -23.77f),
                    new Vector3(-2.08f, 0.436f, -29.80f),
                    2f
                )
            ),

            new LogMotion("Step 8: turn to face exit"),
            new TurnXYZDegrees((0,-179,0),2f),

            new LogMotion("Step 8–11: moving to exit"),
            new ParallelMotion(
                new LookAtTargetTimed(new Vector3(-18.78f, 2.04f, -21.19f), 8f),
                new SequentialMotion(
                    new LogMotion("Step 8"),
                    new MoveToPositionXYZ(new Vector3(-18.78f, 2.04f, -21.19f), 2f),
                    new LogMotion("Step 9"),
                    new MoveToPositionXYZ(new Vector3(-25.65f, 3.94f, -14.91f), 2f), 
                    new LogMotion("Step 10"),
                    new MoveToPositionXYZ(new Vector3(-33.11f, 8.28f, -20.36f), 2f), // look at temple
                    new LogMotion("Step 11"),
                    new MoveToPositionXYZ(new Vector3(-35.93f, 7.68f, -38.26f), 2f)
                )
            ),

            new LogMotion("Pausing to look at temple"), // Temple Position is: (-52.58f, 17.96f, -57.38f)
            new LookAtTargetTimed(new Vector3(-52.58f, 17.96f, -57.38f), 3f),
            new TurnXYZDegrees((-45,0,0),2f),
            new WaitXSeconds(1f),
            new TurnXYZDegrees((0,45,0),2f),
            new WaitXSeconds(1f),
            new TurnXYZDegrees((45,0,0),2f),
            new WaitXSeconds(1f),

            new LogMotion("Step 12–16: move while looking at temple"),
            new SequentialMotion
            (
                new LogMotion("Step 12"),
                new MoveAndLookAt(
                    new Vector3(-25.73f, 8.11f, -32.39f),
                    new Vector3(-52.58f, 17.96f, -57.38f),
                    2f
                ),
                new LogMotion("Step 13"),
                new MoveAndLookAt(
                    new Vector3(-16.98f, 4.54f, -35.47f),
                    new Vector3(-52.58f, 17.96f, -57.38f),
                    2f
                ),
                new LogMotion("Step 14"),
                new MoveAndLookAt(
                    new Vector3(-10.14f, -2.05f, -44.77f),
                    new Vector3(-52.58f, 17.96f, -57.38f),
                    2f
                ),
                new LogMotion("Step 15"),
                new MoveAndLookAt(
                    new Vector3(-14.36f, -3.10f, -55.40f),
                    new Vector3(-52.58f, 17.96f, -57.38f),
                    2f
                ),
                new LogMotion("Step 16"),
                new MoveAndLookAt(
                    new Vector3(-15.95f, -4.24f, -65.03f),
                    new Vector3(-52.58f, 17.96f, -57.38f),
                    2f
                )
            ),
            new LogMotion("CamMove sequence finished")
        );
    }
}