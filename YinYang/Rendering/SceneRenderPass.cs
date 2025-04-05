using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Worlds;

namespace YinYang.Rendering
{
    /// <summary>
    /// Render pass responsible for rendering the visible scene from the camera's perspective.
    /// </summary>
    /// <remarks>
    /// This pass renders all scene objects into the default framebuffer, using lighting and shadow data.
    /// </remarks>
    public class SceneRenderPass : RenderPass
    {
        /// <summary>
        /// Executes the scene render pass from the camera's viewpoint.
        /// </summary>
        /// <param name="camera">The active camera for view/projection transforms.</param>
        /// <param name="lighting">Lighting manager providing light data.</param>
        /// <param name="objects">Scene objects to render.</param>
        /// <param name="lightSpaceMatrix">Matrix used to transform world space into light space for shadows.</param>
        /// <param name="currentWorld">The current world instance.</param>
        /// <returns>Returns the input light-space matrix unmodified.</returns>
        public override Matrix4 Execute(Camera camera, LightingManager lighting, ObjectManager objects, Matrix4 lightSpaceMatrix, World currentWorld)
        {
            // Set OpenGL viewport dimensions to match window size
            GL.Viewport(0, 0, camera.RenderWidth, camera.RenderHeight);

            // Clear the color and depth buffers before drawing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Compute the view-projection matrix for this frame
            // This transforms world space into clip space from the camera's point of view
            Matrix4 viewProjection = camera.GetViewProjection();

            // Render all objects using lighting and transformation data
            objects.Render(viewProjection, lightSpaceMatrix, camera, currentWorld, 0); // TODO: replace currentWorld and 0 with render context and debug mode as needed

            // Return the same light-space matrix to pass along to any subsequent render passes
            return lightSpaceMatrix;
        }

        /// <summary>
        /// Releases any resources used by this render pass. Not used in this case.
        /// </summary>
        public override void Dispose() { }
    }
}
