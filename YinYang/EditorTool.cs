using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;
using YinYang.Materials;
using YinYang.Worlds;

namespace YinYang;

public class EditorTool
{
    private static GameObject? currentGameObject;
    private KeyboardState keyState;
    private World world;

    private Material concreteMaterial = new mat_concrete();
    private Material chromeMaterial = new mat_chrome();
    private Material cliffStoneMaterial = new mat_cliffStone();
    private Material lightStoneMaterial = new mat_lightStone();

    private int currentModel = 0;
    private List<String> modelNames = new List<String>()
    {
        "SmoothCube", "Sphere", "Monkey", "Cave/LightStone", "Cave/BigRock", "Cave/GroundRock001",
        "Cave/GroundRock002", "Cave/GroundRock003", "Cave/GroundRock004", "Cave/GroundRock005", "Cave/GroundRock006"
    };
    private int currentMaterial = 0;
    private List<Material> materials;
    
    public bool IsEditingObject = currentGameObject is not null;
    
    public EditorTool(World world)
    {
        keyState = world.Game.KeyboardState;
        this.world = world;
        
        materials = new List<Material>(){concreteMaterial, chromeMaterial, cliffStoneMaterial, lightStoneMaterial};
    }

    public void Update()
    {
        IsEditingObject = currentGameObject is not null;
        
        //Create object
        if (keyState.IsKeyPressed(Keys.Insert))
        {
            if(currentGameObject != null)
                CommitObject();
            
            Vector3 spawnPosition = new Vector3();
            spawnPosition = world.MainCamera.Position + (world.MainCamera.Front * 5); //Spawn in front of camera
            
            currentGameObject = new GameObjectBuilder(world.Game)
                .Model("SmoothCube")
                .Material(materials[currentMaterial])
                .Position(spawnPosition.X, spawnPosition.Y, spawnPosition.Z)
                .Behavior<EditorBehavior>()
                .Build();
            world.GameObjects.Add(currentGameObject);
            
            currentGameObject.Renderer.Mesh = GameObjectFactory.CreateModel(modelNames[currentModel]);
        }

        //Destroy object
        if (keyState.IsKeyPressed(Keys.Delete))
        {
            if (currentGameObject == null)
            {
                EditorMessage("No object to delete!");
                return;
            }

            world.GameObjects.Remove(currentGameObject);
        }

        //Change model
        if (keyState.IsKeyPressed(Keys.Period))
        {
            NextModel();
        }

        if (keyState.IsKeyPressed(Keys.Comma))
        {
            PreviousModel();
        }
        
        //BUG: Doesn't work after a certain point... Let's just set them manually
        /*//Change material 
        if (keyState.IsKeyPressed(Keys.M))
        {
            NextMaterial();
        }

        if (keyState.IsKeyPressed(Keys.N))
        {
            PreviousMaterial();
        }*/
        
        //Commit object
        if (keyState.IsKeyPressed(Keys.Enter) && currentGameObject != null)
        {
            CommitObject();
        }
    }

    private void NextModel()
    {
        if (currentGameObject == null)
            return;
        
        currentModel++;
        if(currentModel >= modelNames.Count)
            currentModel = 0;
        
        currentGameObject.Renderer.Mesh = GameObjectFactory.CreateModel(modelNames[currentModel]);
    }

    private void PreviousModel()
    {
        if (currentGameObject == null)
            return;
        
        currentModel--;
        if(currentModel < 0)
            currentModel = modelNames.Count - 1;
        
        currentGameObject.Renderer.Mesh = GameObjectFactory.CreateModel(modelNames[currentModel]);
    }

    /*private void NextMaterial()
    {
        if(currentGameObject == null)
            return;
        
        currentMaterial++;
        if(currentMaterial >= materials.Count)
            currentMaterial = 0;
        
        EditorMessage(currentMaterial.ToString());
        
        currentGameObject.Renderer.Material = materials[currentMaterial];
    }

    private void PreviousMaterial()
    {
        if(currentGameObject == null)
            return;
        
        currentMaterial--;
        if(currentMaterial < 0)
            currentMaterial = materials.Count - 1;
        
        EditorMessage(currentMaterial.ToString());
        
        currentGameObject.Renderer.Material = materials[currentMaterial];
    }*/

    public void CommitObject(bool force = false)
    {
        if(force)
            EditorMessage("- FORCE COMMITTED OBJECT - \n   - Evaluate if you want this object or not!", ConsoleColor.Yellow);
        
        //TODO: Model and material name
        EditorMessage("Committed object - " + modelNames[currentModel] + ":" +
                    "\nPosition is: " + currentGameObject.Transform.Position + 
                    "\nRotation is: " + currentGameObject.Transform.GetRotationInDegrees() +
                    "\nScale is: " + currentGameObject.Transform.Scale, force ? ConsoleColor.Yellow : ConsoleColor.Green);
            
        currentGameObject.RemoveComponent<EditorBehavior>();
        currentGameObject = null;
    }

    private void EditorMessage(string message, ConsoleColor color = ConsoleColor.Gray)
    {
        Console.ForegroundColor = color;
        Console.WriteLine($"\nEditorTool: {message}");
    }
}