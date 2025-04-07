using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;

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
        public override Matrix4 Execute(RenderContext context, ObjectManager objects)
        {
            // Set OpenGL viewport dimensions to match window size
            GL.Viewport(0, 0, context.Camera.RenderWidth, context.Camera.RenderHeight);

            // Clear the color and depth buffers before drawing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render all objects using lighting and transformation data
            objects.Render(context);
            
            var err = GL.GetError();
            if (err != ErrorCode.NoError)
                Console.WriteLine($"[GL ERROR] after {nameof(SceneRenderPass)}: {err}");

            // Return the same light-space matrix to pass along to any subsequent render passes
            return context.LightSpaceMatrix;
        }

        /// <summary>
        /// Releases any resources used by this render pass. Not used in this case.
        /// </summary>
        public override void Dispose() { }
    }
}