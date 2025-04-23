using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Behaviors.Motion;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Materials;
using YinYang.Particles;

namespace YinYang.Worlds;

public class SceneTestWorld : World
{
    private GameObject cave;
    private GameObject cliffExit;
    private GameObject groundOutdoors;
    private GameObject mask;

    public SceneTestWorld(Game game) : base(game)
    {
        WorldName = game.Title + " Scene Test World";

        SkyColor = Color4.CornflowerBlue;
        lightingManager.Sun.ToggleLight();
    }

    protected override void ConstructWorld()
    {
        base.ConstructWorld();
        
        cave = new GameObjectBuilder(Game)
            .ModelTBN("Cave/CaveNew")
            .Material(new  mat_cliffStone())
            .Position(0f, 0, 0f)
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

        GameObjects.Add(cave);
        
        mask = new GameObjectBuilder(Game)
            .ModelTBN("mask")
            .Material(new mat_mirror())
            .Position(-2.08f, 0.436f, -29.80f)
            .RotationDegrees(28.64f, 0f, 0f)
            .Scale(2,2,2)
            .Build();

        GameObjects.Add(mask);
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();
        
        // particles
        PlaceParticles();

        // Cam Route
        //MoveCam();
    }

    private void PlaceParticles()
    {
        var magicParticlesRight = new GameObject(Game);
        magicParticlesRight.Transform.Position = new Vector3(5.75f, 0.7f, 3.0f);
        magicParticlesRight.AddComponent<MagicParticleSystem>(1000); 
        GameObjects.Add(magicParticlesRight);
        
        var magicParticlesFront = new GameObject(Game);
        magicParticlesFront.Transform.Position = new Vector3(0.4f, 0.7f, -6.5f);
        magicParticlesFront.AddComponent<MagicParticleSystem>(1000); 
        GameObjects.Add(magicParticlesFront);
        
        var WaterfallParticles = new GameObject(Game);
        WaterfallParticles.Transform.Position = new Vector3(0,0,0); 
        WaterfallParticles.AddComponent<WaterfallMistParticleSystem>(1000);
        GameObjects.Add(WaterfallParticles);
    }

    private void MoveCamTest()
    {
        var cameraObj = cameraManager.Camera.GameObject;

        cameraObj.AddComponent<SequentialBehavior>(
            new LogMotion("MoveCamTest begins"),
            new MoveToPositionXYZ(new Vector3(4.38f, 0.61f, -1.84f), 3f),new LogMotion("step 1"),
            new MoveToPositionXYZ(new Vector3(11.10f, 1.47f, -8.69f), 3f),new LogMotion("step 2"),
            new MoveToPositionXYZ(new Vector3(5.65f, 1.82f, -21.28f), 3f),new LogMotion("step 3"),
            new MoveToPositionXYZ(new Vector3(1.21f, 2.67f, -26.63f), 3f),new LogMotion("step 4"),
            new MoveToPositionXYZ(new Vector3(-1.82f, 2.67f, -25.35f), 3f),new LogMotion("step 5"),
            new MoveToPositionXYZ(new Vector3(-5.17f, 2.67f, -26.16f), 3f),new LogMotion("step 6"),
            new MoveToPositionXYZ(new Vector3(-11.22f, 2.39f, -23.77f), 3f),new LogMotion("step 7"),
            new MoveToPositionXYZ(new Vector3(-18.78f, 2.04f, -21.19f), 3f),new LogMotion("step 8"),
            new MoveToPositionXYZ(new Vector3(-25.65f, 3.94f, -14.91f), 3f),new LogMotion("step 9"),
            new MoveToPositionXYZ(new Vector3(-33.11f, 8.28f, -20.36f), 3f),new LogMotion("step 10"),
            new MoveToPositionXYZ(new Vector3(-34.93f, 8.68f, -27.26f), 3f),new LogMotion("step 11"),
            new MoveToPositionXYZ(new Vector3(-25.73f, 8.11f, -32.39f), 3f),new LogMotion("step 12"),
            new MoveToPositionXYZ(new Vector3(-16.98f, 4.54f, -35.47f), 3f),new LogMotion("step 13"),
            new MoveToPositionXYZ(new Vector3(-10.14f, -2.05f, -44.77f), 3f),new LogMotion("step 14"),
            new MoveToPositionXYZ(new Vector3(-14.36f, -3.10f, -55.40f), 3f),new LogMotion("step 15"),
            new MoveToPositionXYZ(new Vector3(-15.95f, -4.24f, -65.03f), 3f),new LogMotion("step 16"),
            new LogMotion("MoveCamTest finished")
        );
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
            new TurnXYZDegrees((0,200,0),2f),

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
                    new MoveToPositionXYZ(new Vector3(-34.93f, 8.68f, -27.26f), 2f)
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