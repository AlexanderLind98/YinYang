using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Lights;
using YinYang.Materials;

namespace YinYang.Worlds;

public class SceneTestWorld : World
{
    private GameObject cave;
    private GameObject cliffExit;
    private GameObject groundOutdoors;

    public SceneTestWorld(Game game) : base(game)
    {
        WorldName = game.Title + " Scene Test World";

        SkyColor = Color4.CornflowerBlue;
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
            .Model("Scene/Cave")
            .Material(new  mat_concrete())
            .Position(0f, 0, 0f)
            .Build();

        cliffExit = new GameObjectBuilder(Game)
            .Model("Scene/CliffExit")
            .Material(new mat_concrete())
            .Position(0, 0f, 0f)
            .Build();

        groundOutdoors = new GameObjectBuilder(Game)
            .Model("Scene/GroundOutdoors")
            .Material(new mat_concrete())
            .Position(0f, 0f, 0f)
            .Build();

        GameObjects.Add(cave);
        GameObjects.Add(cliffExit);
        GameObjects.Add(groundOutdoors);
        
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