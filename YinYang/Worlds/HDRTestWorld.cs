using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Materials;
using YinYang.Lights;

namespace YinYang.Worlds;

public class HDRTestWorld : World
{
    private GameObject room;
    private GameObject staticCube;
    private GameObject Cube;
    private GameObject rotatingCube;
    
    public override string DebugLabel => "HDR Visual Test";
    public HDRTestWorld(Game game) : base(game)
    {
        WorldName = "HDR Test World";
        SkyColor = Color4.Black;
        
       
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
            .Position(-3f, 0f, 0f)
            .Behavior<RotateObjectBehavior>(Vector3.UnitX, 10f)
            .Build();
        
        Cube = new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new mat_concrete())
            .Position(0f, 0f, 0f)
            .Behavior<RotateObjectBehavior>(Vector3.UnitY, 10f)
            .Build();

        rotatingCube = new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new mat_chrome())
            .Position(3f, 0f, 0f)
            .Behavior<RotateObjectBehavior>(Vector3.UnitZ, 10f)
            .Build();

        GameObjects.Add(room);
        GameObjects.Add(staticCube);
        GameObjects.Add(Cube);
        GameObjects.Add(rotatingCube);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();
    }

    public override void HandleInput(KeyboardState input)
    {
        if (input.IsKeyPressed(Keys.H))
        {
            renderPipeline.HdrPass.HDR_Enabled = !renderPipeline.HdrPass.HDR_Enabled;
            Console.WriteLine("HDR toggled: " + renderPipeline.HdrPass.HDR_Enabled);
        }
    }
}