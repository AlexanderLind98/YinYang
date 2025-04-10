using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors.Motion;
using YinYang.Lights;
using YinYang.Materials;

namespace YinYang.Worlds;

public class MotionTestWorld : World
{
    private GameObject SmoothCube;
    private GameObject Monkey;
    private GameObject Sphere;

    public MotionTestWorld(Game game) : base(game)
    {
        WorldName = game.Title + " motion Test World";

        SkyColor = Color4.CornflowerBlue;
        // SunColor = Vector3.Zero;
        // DirectionalLight.LightColor = SunColor;
    }
    
    protected override void ConstructWorld()
    {
        base.ConstructWorld();
        
        Ground.Add(new GameObjectBuilder(Game)
            .Model("Ground")
            .Material(new  mat_concrete())
            .Position(0f, -3f, 0f)
            .Scale(2, 2, 2)
            .Build());
        
        SmoothCube = new GameObjectBuilder(Game)
            .Model("SmoothCube")
            .Material(new  mat_concrete())
            .Position(0f, -2f, 0f)
            .Scale(1, 1, 1)
            .Build();

        Monkey = new GameObjectBuilder(Game)
            .Model("Monkey")
            .Material(new mat_concrete())
            .Position(2f, -0.5f, 2f) 
            .RotationDegrees(0, -90, 0) 
            .Build();
        
        Sphere = new GameObjectBuilder(Game)
            .Model("Sphere")
            .Material(new mat_concrete())
            .Position(0f, 0f, 0f)
            .Build();
        
        Sphere.AddComponent<ParallelBehavior>
        (
            new IAutoMotion[] 
            {
                new UpDown(0.1f, 1f),
                new SidetoSide(0.2f, 0.5f),
                new FowardBackward(0.1f, 0.1f),
            }
        );
        
        Monkey.AddComponent<SequentialBehavior>(
            new WaitXSeconds(1),
            new TurnXYZDegrees(new Vector3(0,0,0), 0.5f),

            new SequentialMotion(

                // Hjørne 1: gå frem og drej
                new MoveToPositionXYZ(new Vector3(2, 0, 2), 2f),
                new TurnXYZDegrees(new Vector3(0, 90, 0), 0.8f),
                new PulseScaleXTimes(0.1f, 0.5f, 1),
                new WaitXSeconds(0.5f),

                // Hjørne 2: går og kigger sig omkring
                new ParallelMotion(
                    new MoveToPositionXYZ(new Vector3(-2, 0, 2), 2f),
                    new LoopMotion(
                        new SequentialMotion(
                            new TurnXYZDegrees(new Vector3(0, 135, 0), 0.4f),
                            new TurnXYZDegrees(new Vector3(0, 225, 0), 0.4f)
                        ),
                        2 // kigger sig omkring 2 gange
                    )
                ),
                new TurnXYZDegrees(new Vector3(0, 180, 0), 0.6f),
                new WaitXSeconds(0.4f),

                // Hjørne 3: gå hen og scaler
                new MoveToPositionXYZ(new Vector3(-4, 0, -4), 2f),
                new TurnXYZDegrees(new Vector3(0, 270, 0), 0.8f),
                new LoopMotion(
                    new SequentialMotion(
                        new ScaleTo(new Vector3(1.5f), 0.3f),
                        new ScaleTo(new Vector3(1f), 0.3f)
                    ),
                    3
                ),

                // Hjørne 4: går mens han roterer sidelæns igen
                new ParallelMotion(
                    new MoveToPositionXYZ(new Vector3(4, 0, -4), 2f),
                    new LoopMotion(
                        new SequentialMotion(
                            new TurnXYZDegrees(new Vector3(0, 315, 0), 0.4f),
                            new TurnXYZDegrees(new Vector3(0, 45, 0), 0.4f)
                        ),
                        2
                    )
                ),
                new TurnXYZDegrees(new Vector3(0, 0, 0), 0.6f),
                new MoveToPositionXYZ(new Vector3(2, 0, 2), 2f),
                new WaitXSeconds(0.5f)
            ),
            
            // Returnér til centerposition og aktiver lookout-loop
            new MoveToPositionXYZ(new Vector3(0, 1.5f, 0.5f), 2f),

            new ParallelMotion( // pulser lidt mens monkey kigger 
                new LoopMotion( // overload gentag 999 gange
                    new SequentialMotion(
                        new TurnXYZDegrees(new Vector3(0, 90, 0), 0.5f),
                        new TurnXYZDegrees(new Vector3(0, 0, 0), 0.5f),
                        new TurnXYZDegrees(new Vector3(0, -90, 0), 0.5f),
                        new TurnXYZDegrees(new Vector3(0, 0, 0), 0.5f)
                    ), 999
                ),
                new PulseScaleXTimes(0.03f, 1.5f, 999)
            )
        );



        Ground.Add(SmoothCube);
        Ground.Add(Monkey);
        Ground.Add(Sphere);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();
        
        new PointLight(this);
        PointLights[0].SetPosition(0, 0, 3);
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