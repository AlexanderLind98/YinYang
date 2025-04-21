using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Behaviors.Motion;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Materials;

namespace YinYang.Worlds;

public class GodRayTestWorld : World
{
 
    private GameObject movingCube1;
    private GameObject sunpointer;
    private GameObject center;

    public GodRayTestWorld(Game game) : base(game)
    {
        WorldName = game.Title + " Scene Test World";

        SkyColor = Color4.Black;
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
        
        // GameObjects.Add(new GameObjectBuilder(Game)
        //     .Model("Ground")
        //     .Material(new  mat_concrete())
        //     .Position(0f, -3f, 0f)
        //     .Scale(2, 2, 2)
        //     .Build());
        
        // cubes i 3D grid
        int countX = 6;
        int countY = 2;
        int countZ = 6;
        float spacing = 3.0f;
        float verticalOffset = 10.0f;

        for (int x = -countX / 2; x < countX / 2; x++)
        {
            for (int y = 0; y < countY; y++)
            {
                for (int z = -countZ / 2; z < countZ / 2; z++)
                {
                    GameObjects.Add(new GameObjectBuilder(Game)
                        .Model("Cube")
                        .Material(new mat_concrete())
                        .Position(x * spacing, y * spacing + verticalOffset, z * spacing)
                        .Build()
                    );
                }
            }
        }
        
        movingCube1 = new GameObjectBuilder(Game)
            .Model("Cube")
            .Material(new mat_concrete())
            .Position(2f, 3f, 0f)
            .Build();
        GameObjects.Add(movingCube1);

        sunpointer = new GameObjectBuilder(Game)
            .Model("Cave/LightStone")
            .Material(new mat_chrome())
            .Position(0,0,-1f)
            .Build();
        GameObjects.Add(sunpointer);
        
        center = new GameObjectBuilder(Game)
            .Model("Sphere")
            .Material(new mat_chrome())
            .Position(0,0,0f)
            .Scale(0.5f, 0.5f, 0.5f)
            .Build();
        GameObjects.Add(center);

        movingCube1.AddComponent<SequentialBehavior>(
            new LoopMotion(
                new SequentialMotion(
                    new MoveToPositionXYZ(new Vector3(2, 3, 0), 2f),
                    new MoveToPositionXYZ(new Vector3(-2, 3, 0), 2f)
                ), 999
            )
        );
            
        
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