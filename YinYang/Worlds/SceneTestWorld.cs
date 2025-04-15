using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Behaviors.Motion;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Materials;

namespace YinYang.Worlds;

public class SceneTestWorld : World
{
 
    private GameObject movingCube1;

    public SceneTestWorld(Game game) : base(game)
    {
        WorldName = game.Title + " Scene Test World";

        SkyColor = Color4.CornflowerBlue;
        //lightingManager.Sun.ToggleLight();
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
        
        GameObjects.Add(new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new  mat_concrete())
            .Position(2f, 0f, 0f)
            .Scale(1, 1, 1)
            .Build());

        movingCube1 = new GameObjectBuilder(Game)
            .Model("Cube")
            .Material(new mat_concrete())
            .Position(2f, 3f, 0f)
            .Build();
        GameObjects.Add(movingCube1);

        // movingCube1.AddComponent<SequentialBehavior>(
        //     new LoopMotion(
        //         new SequentialMotion(
        //             new MoveToPositionXYZ(new Vector3(2, 3, 0), 2f),
        //             new MoveToPositionXYZ(new Vector3(-2, 3, 0), 2f)
        //         ), 999
        //     )
        // );
            
        
        // var red = new PointLight(this, Color4.Red, 30f);
        // PointLights[0].shadowType = Light.ShadowType.Dynamic;
        // red.Transform.Position = new Vector3(3f, 3f, 1f);
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