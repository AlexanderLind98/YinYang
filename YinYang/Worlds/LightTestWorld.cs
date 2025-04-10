using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Lights;
using YinYang.Materials;

namespace YinYang.Worlds;

public class LightTestWorld : World
{
    private GameObject room;
    private GameObject staticCube;
    private GameObject rotatingCube;

    public LightTestWorld(Game game) : base(game)
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
        
        room = new GameObjectBuilder(Game)
            .Model("Ground")
            .Material(new  mat_concrete())
            .Position(0f, -2f, 0f)
            .Scale(2, 2 ,2)
            .Build();

        staticCube = new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new mat_concrete())
            .Position(-1.5f, 0f, 0f)
            .Build();

        rotatingCube = new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new mat_glow())
            .Position(1.5f, 0f, 0f)
            .Behavior<RotateObjectBehavior>(Vector3.UnitZ, 10f)
            .Build();

        GameObjects.Add(room);
        GameObjects.Add(staticCube);
        GameObjects.Add(rotatingCube);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();
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
            /*staticCube.Renderer.Material = new mat_gold_simple();
            staticCube.Renderer.Material.UpdateUniforms();

            rotatingCube.Renderer.Material = new mat_gold_simple();
            rotatingCube.Renderer.Material.UpdateUniforms();*/

            Game.DebugMode = 0; // Full lighting
        }
    }
}