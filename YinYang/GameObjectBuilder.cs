using OpenTK.Mathematics;
using YinYang.Behaviors;
using YinYang.Components;
using YinYang.Materials;
using YinYang.Rendering;

namespace YinYang;

/// <summary>
/// Fluent builder for constructing and configuring GameObjects in a declarative manner.
/// Supports loading OBJ models, assigning materials, transforms, and behaviors.
/// </summary>
public class GameObjectBuilder
{
    private readonly Game _game;
    private GameObject _gameObject;
    private Mesh _mesh;
    private Material _material;

    /// <summary>
    /// Initializes a new builder instance for constructing GameObjects.
    /// </summary>
    /// <param name="game">The main game instance.</param>
    public GameObjectBuilder(Game game)
    {
        _game = game;
    }

    /// <summary>
    /// Loads a model from an OBJ file and assigns the resulting GameObject and Mesh.
    /// </summary>
    /// <param name="modelName">The name of the model to load (without extension).</param>
    /// <returns>The builder instance.</returns>
    public GameObjectBuilder Model(string modelName)
    {
        (_gameObject, _mesh) = GameObjectFactory.CreateObjModel(_game, modelName);
        return this;
    }

    /// <summary>
    /// Assigns a material to be used with the GameObject's mesh.
    /// </summary>
    /// <param name="material">The material to use.</param>
    /// <returns>The builder instance.</returns>
    public GameObjectBuilder Material(Material material)
    {
        _material = material;
        return this;
    }

    /// <summary>
    /// Sets the initial position of the GameObject.
    /// </summary>
    public GameObjectBuilder Position(float x, float y, float z)
    {
        _gameObject.Transform.Position = new Vector3(x, y, z);
        return this;
    }

    /// <summary>
    /// Sets the initial scale of the GameObject.
    /// </summary>
    public GameObjectBuilder Scale(float x, float y, float z)
    {
        _gameObject.Transform.Scale = new Vector3(x, y, z);
        return this;
    }

    /// <summary>
    /// Adds a behavior component to the GameObject.
    /// The behavior must have a constructor with (GameObject, Game) as the first two parameters.
    /// </summary>
    public GameObjectBuilder Behavior<T>(params object[] additionalArgs) where T : Behaviour
    {
        _gameObject.AddComponent<T>(additionalArgs);
        return this;
    }

    /// <summary>
    /// Finalizes and returns the fully constructed GameObject.
    /// </summary>
    public GameObject Build()
    {
        if (_material != null && _mesh != null)
        {
            _gameObject.Renderer = new Renderer(_material, _mesh);
            _material = null;
        }
        else
        {
            (_material as IDisposable)?.Dispose();
        }

        return _gameObject;
    }
}
