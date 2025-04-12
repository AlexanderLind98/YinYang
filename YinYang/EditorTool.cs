using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Materials;
using YinYang.Worlds;

namespace YinYang;

public class EditorTool
{
    public EditorTool(World world)
    {
        keyState = world.Game.KeyboardState;
        this.world = world;
    }
    
    private GameObject currentGameObject;
    private KeyboardState keyState;
    private World world;

    public void Update()
    {
        //TODO: Create object, move object, commit object, repeat
        
        //Create object
        if (keyState.IsKeyPressed(Keys.Insert))
        {
            currentGameObject = new GameObjectBuilder(world.Game)
                .Model("SmoothCube")
                .Material(new mat_concrete())
                .Position(0f, 0f, 0f)
                .Behavior<EditorBehavior>()
                .Build();
            world.GameObjects.Add(currentGameObject);
        }
    }
}