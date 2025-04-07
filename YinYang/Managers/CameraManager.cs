using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;

namespace YinYang.Managers
{
    /// <summary>
    /// Responsible for creating, storing, and providing access to the main camera.
    /// </summary>
    /// <remarks>
    /// Initializes the camera as a GameObject with movement behavior, and provides 
    /// utility methods for accessing view-projection matrices and camera position.
    /// </remarks>
    public class CameraManager
    {
        /// <summary>
        /// The active camera instance in the world.
        /// </summary>
        public Camera? Camera { get; private set; }

        /// <summary>
        /// Sets up the camera by creating a GameObject with camera and movement behavior.
        /// </summary>
        /// <param name="game">The main game instance.</param>
        /// <param name="gameObjects">The list to which the camera object will be added.</param>
        public void Setup(GameWindow game, List<GameObject> gameObjects)
        {
            // Create a new GameObject to represent the camera in the scene.
            GameObject cameraObject = new GameObject(game);

            // Add the Camera behavior with specified field of view, aspect ratio, and clipping planes.
            // This determines how the 3D world is projected onto a 2D screen.
            cameraObject.AddComponent<Camera>(60.0f, game.Size.X, game.Size.Y, 0.3f, 1000.0f);

            // Add a behavior that allows camera movement using input.
            cameraObject.AddComponent<CamMoveBehavior>();

            // Retrieve the camera component so we can reference it directly.
            Camera = cameraObject.GetComponent<Camera>();

            // Add the camera object to the world so it gets updated and rendered.
            gameObjects.Add(cameraObject);

            // Lock and focus the cursor inside the game window.
            // game.CursorState = CursorState.Grabbed;
        }
        
        /// <summary>
        /// Updates the camera's internal state and any attached behaviors.
        /// </summary>
        /// <param name="args">Frame timing information.</param>
        public void Update(FrameEventArgs args)
        {
            Camera?.Update(args);
        }
        
        /// <summary>
        /// Handles keyboard input for the camera.
        /// </summary>
        /// <param name="input">Current keyboard state.</param>
        public void HandleInput(KeyboardState input)
        {
            Camera?.HandleInput(input);
        }


        /// <summary>
        /// Returns the view-projection matrix from the active camera.
        /// </summary>
        /// <returns>A matrix that transforms world-space coordinates into clip-space.</returns>
        /// <remarks>
        /// This matrix is composed by multiplying the view matrix (camera orientation and position)
        /// with the projection matrix (field of view, aspect ratio, near/far planes).
        /// </remarks>
        public Matrix4 GetViewProjection()
        {
            return Camera.GetViewProjection();
        }

        /// <summary>
        /// Returns the world-space position of the active camera.
        /// </summary>
        /// <returns>The position of the camera.</returns>
        public Vector3 GetPosition()
        {
            return Camera.Position;
        }
    }
}
