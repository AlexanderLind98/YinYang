using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using YinYang.Rendering;
using YinYang.Worlds;

namespace YinYang.Managers
{
    /// <summary>
    /// Responsible for managing all GameObjects in the world.
    /// </summary>
    /// <remarks>
    /// Provides methods to update, render, and clean up all active GameObjects.
    /// </remarks>
    public class ObjectManager
    {
        /// <summary>
        /// List of all GameObjects currently present in the world.
        /// </summary>
        public List<GameObject> GameObjects { get; } = new();

        /// <summary>
        /// Updates all GameObjects with the provided frame timing.
        /// </summary>
        /// <param name="args">Frame timing arguments.</param>
        public void Update(FrameEventArgs args)
        {
            // Iterate through each GameObject and update it using the current frame data.
            foreach (var obj in GameObjects)
            {
                obj.Update(args);
            }
        }

        /// <summary>
        /// Draws all GameObjects using the specified camera, transformation matrices, and lighting information.
        /// </summary>
        /// <param name="viewProjection">Matrix that transforms world space into clip space.</param>
        /// <param name="lightSpaceMatrix">Matrix that transforms world space into the light's projection space for shadow mapping.</param>
        /// <param name="camera">The active camera instance.</param>
        /// <param name="currentWorld">The world currently being rendered.</param>
        /// <param name="debugMode">Current debug mode to pass to the shaders.</param>
        /// <remarks>
        /// The view-projection matrix combines camera view and perspective.
        /// The light-space matrix is used for projecting fragments into shadow map space.
        /// </remarks>
        public void Render(RenderContext context)
        {
            // Iterate through and draw each GameObject with appropriate matrices and lighting context.
            foreach (var obj in GameObjects)
            {
                if (obj.Renderer == null) //TODO: maybe seperate lists for renderers and non-renderers
                {
                    // If the object has no renderer, skip it.
                    //Console.WriteLine($"ObjectManager.Render: Object at {obj.Transform.Position} has no Renderer.");
                    continue;
                }
                
                obj.Draw(context);
            }
        }

        /// <summary>
        /// Executes the depth-only rendering pass for all GameObjects, used for shadow mapping.
        /// </summary>
        /// <param name="depthShader">The shader used for rendering the depth map.</param>
        public void RenderDepth(Shader depthShader)
        {
            // Render each object using the depth-only shader to populate the shadow map.
            foreach (var obj in GameObjects)
            {
                obj.RenderDepth(depthShader);
            }
        }

        /// <summary>
        /// Cleans up all GameObjects and their resources.
        /// </summary>
        /// <remarks>
        /// Disposes of all renderers, materials, meshes, and clears the object list.
        /// </remarks>
        public void Dispose()
        {
            foreach (var obj in GameObjects)
            {
                obj.Dispose();

                if (obj.Renderer != null)
                {
                    obj.Renderer.Mesh?.Dispose();

                    if (obj.Renderer.Material is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }

            GameObjects.Clear();
        }
    }
}
