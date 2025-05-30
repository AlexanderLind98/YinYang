using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors.Motion;
using YinYang.Lights;
using YinYang.Materials;

namespace YinYang.Worlds;

public class PointShadowTestWorld : World
{
    private GameObject room;
    private GameObject monkey;
    private GameObject rotatingCube;

    public PointShadowTestWorld(Game game) : base(game)
    {
        WorldName = game.Title + " Light Test World";

        SkyColor = Color4.CornflowerBlue;
        // SunColor = Vector3.Zero;
        // DirectionalLight.LightColor = SunColor;
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
        
        GameObjects.Add(new GameObjectBuilder(Game)
            .Model("Ground")
            .Material(new  mat_concrete())
            .Position(0f, -3f, 0f)
            .Scale(2, 2, 2)
            .Build());
        
        room = new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new  mat_concrete())
            .Position(0f, -2f, 0f)
            .Scale(1, 1, 1)
            .Build();

        monkey = new GameObjectBuilder(Game)
            .Model("Monkey")
            .Material(new mat_concrete())
            .Position(2f, 0f, 0f)
            .RotationDegrees(45,25,0)
            .Build();

        monkey.AddComponent<ParallelBehavior>
        (
            new IAutoMotion[] 
            {
                new UpDown(0.1f, 1f),
                new SidetoSide(0.2f, 0.5f),
                new FowardBackward(0.1f, 0.1f),
            }
        );


        rotatingCube = new GameObjectBuilder(Game)
            .Model("Sphere")
            .Material(new mat_concrete())
            .Position(-2f, 0f, 0f)
            .Build();

        GameObjects.Add(room);
        GameObjects.Add(monkey);
        GameObjects.Add(rotatingCube);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();
        
        new PointLight(this, Color4.White, 1.0f);
        PointLights[0].SetPosition(0, 0, 3);
        PointLights[0].shadowType = Light.ShadowType.Dynamic;
    }

    public override void HandleInput(KeyboardState input)
    {
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
        
        /*if (input.IsKeyPressed(Keys.H))
        {
            renderPipeline.HdrPass.HDR_Enabled = !renderPipeline.HdrPass.HDR_Enabled;
            Console.WriteLine("HDR toggled: " + renderPipeline.HdrPass.HDR_Enabled);
        }*/
    }
}