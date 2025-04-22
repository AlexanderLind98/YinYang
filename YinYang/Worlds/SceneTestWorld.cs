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
    private GameObject cave;
    private GameObject cliffExit;
    private GameObject groundOutdoors;

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
        
        new SpotLight(this, Color4.White, 1f, 15.0f, 20.0f);
        SpotLights[0].ToggleLight();

        MoveCam();
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
        var cameraObj = cameraManager.Camera.GameObject;

        // set initial
        cameraObj.SetRotationInDegrees(0, 0, 0); 
        
        // move sequence
        cameraObj.AddComponent<SequentialBehavior>
        (
            new LogMotion("Starting CamMove"),
            new WaitXSeconds(0.5f),

            new LogMotion("Looking side to side"),
            new SequentialMotion(
                new TurnXYZDegrees(new Vector3(0, 45, 0), 0.5f),
                new WaitXSeconds(0.5f),
                new TurnXYZDegrees(new Vector3(0, 0, 0), 0.5f),
                new WaitXSeconds(0.5f),
                new TurnXYZDegrees(new Vector3(0, -45, 0), 0.5f),
                new WaitXSeconds(0.5f)
            ),

            new LogMotion("Turning to Step 1 direction"),
            new LookAtTargetTimed(new Vector3(4.38f, 0.61f, -1.84f), 2f),

            new LogMotion("Steps 1–3, moving towards mask"),
            //new LookAtTargetTimed(new Vector3(-0.01f, -0.36f, -0.93f), 1f),
            new ParallelMotion
            (
                new CamForwardDirectionBehavior(new Vector3(-0.01f, -0.36f, -0.93f), 10f), // Step 5's direction
                new SequentialMotion(
                    new LogMotion("Step 1"),
                    new MoveToPositionXYZ(new Vector3(4.38f, 0.61f, -1.84f), 2f),
                    new LogMotion("Step 2"),
                    new MoveToPositionXYZ(new Vector3(11.10f, 1.47f, -8.69f), 2f),
                    new LogMotion("Step 3"),
                    new MoveToPositionXYZ(new Vector3(5.65f, 1.82f, -21.28f), 2f)
                )
            ),
            new LogMotion("step 4-7, Looking at mask"),
            new ParallelMotion(
                new CamForwardDirectionBehavior(new Vector3(-2.08f, 0.436f, -29.80f), 8f), // looking at mask
                new SequentialMotion
                (
                    new LogMotion("Step 4"),
                    new MoveToPositionXYZ(new Vector3(1.21f, 2.67f, -26.63f), 2f),
                    new LogMotion("Step 5"),
                    new MoveToPositionXYZ(new Vector3(-1.82f, 2.67f, -25.35f), 2f),
                    new LogMotion("Step 6"),
                    new MoveToPositionXYZ(new Vector3(-5.17f, 2.67f, -26.16f), 2f),
                    new LogMotion("Step 7"),
                    new MoveToPositionXYZ(new Vector3(-11.22f, 2.39f, -23.77f), 2f)
                )
            ),
            new LogMotion("Step 8: turn to exit"),
            new LookAtTargetTimed(new Vector3(-18.78f, 2.04f, -21.19f), 2f),

            new LogMotion("Step 8–11: moving to exit"),
            new ParallelMotion(
                new CamForwardDirectionBehavior(new Vector3(-18.78f, 2.04f, -21.19f), 8f),
                new SequentialMotion(
                    new LogMotion("Step 8"),
                    new MoveToPositionXYZ(new Vector3(-18.78f, 2.04f, -21.19f), 2f),
                    new LogMotion("Step 9"),
                    new MoveToPositionXYZ(new Vector3(-25.65f, 3.94f, -14.91f), 2f),
                    new LogMotion("Step 10"),
                    new MoveToPositionXYZ(new Vector3(-33.11f, 8.28f, -20.36f), 2f),
                    new LogMotion("Step 11"),
                    new MoveToPositionXYZ(new Vector3(-34.93f, 8.68f, -27.26f), 2f)
                )
            ),

            new LogMotion("Pausing to look at temple"),
            new LookAtTargetTimed(new Vector3(-0.95f, 0.30f, -0.30f), 3f),
            new WaitXSeconds(1f),

            new LogMotion("Step 12–16: move while looking at origin"),
            new ParallelMotion(
                new CamForwardDirectionBehavior(new Vector3(-0.95f, 0.30f, -0.30f), 10f),
                new SequentialMotion(
                    new LogMotion("Step 12"),
                    new MoveToPositionXYZ(new Vector3(-25.73f, 8.11f, -32.39f), 2f),
                    new LogMotion("Step 13"),
                    new MoveToPositionXYZ(new Vector3(-16.98f, 4.54f, -35.47f), 2f),
                    new LogMotion("Step 14"),
                    new MoveToPositionXYZ(new Vector3(-10.14f, -2.05f, -44.77f), 2f),
                    new LogMotion("Step 15"),
                    new MoveToPositionXYZ(new Vector3(-14.36f, -3.10f, -55.40f), 2f),
                    new LogMotion("Step 16"),
                    new MoveToPositionXYZ(new Vector3(-15.95f, -4.24f, -65.03f), 2f)
                )
            ),

            new LogMotion("CamMove sequence finished")

        );
    }
}