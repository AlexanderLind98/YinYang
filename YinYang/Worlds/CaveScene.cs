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
            .Model("Cave/MainCave")
            // .Model("SmoothCube")
            .Material(new  mat_cliffStone())
            .Position(0f, 0, 0f)
            .Build();

        CavePillar = new GameObjectBuilder(Game)
            .Model("Cave/CavePillar")
            .Material(new mat_cliffStone())
            .Position(0, 0f, 0f)
            .Build();
        
        lightStone = new GameObjectBuilder(Game)
            .Model("Cave/LightStone")
            .Material(new mat_lightStone())
            .Position(5.75f, 0.7f, 2.98f)
            .RotationDegrees(40.0f, 45.8f, 11.45f)
            .Scale(1.37f, 1.37f, 1.37f)
            .Behavior<EditorBehavior>()
            .Build();

        GameObjects.Add(cave);
        GameObjects.Add(CavePillar);
        GameObjects.Add(lightStone);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();

        new PointLight(this, Color4.Goldenrod);
        PointLights[0].SetPosition(0, 0, 0);
        
        new PointLight(this, Color4.Goldenrod);
        PointLights[1].SetPosition(0, 2, -25);
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

        if (input.IsKeyPressed(Keys.T))
        {
            lightingManager.Sun.ToggleLight();
        }
    }
}