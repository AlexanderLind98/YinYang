using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Worlds;

namespace YinYang.Rendering
{
    /// <summary>
    /// Abstract base class for all modular render passes in the pipeline.
    /// </summary>
    /// <remarks>
    /// Defines a common interface for executing and cleaning up rendering stages.
    /// Provides optional toggling and naming support for diagnostics and control.
    /// </remarks>
    public abstract class RenderPass
    {
        /// <summary>
        /// Indicates whether this pass should be executed.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// A human-readable identifier for the render pass.
        /// </summary>
        public virtual string Name => GetType().Name;

        /// <summary>
        /// Executes the render pass with access to scene data and rendering context.
        /// </summary>
        /// <param name="context">Per Frame render context data.</param>
        /// <param name="objects">Scene objects to render.</param> //TODO: maybe decouple objectmanger and use list or delegate for objects to render
        /// <returns>The updated light-space matrix.</returns>
        public abstract Matrix4? Execute(RenderContext context, ObjectManager objects);


        /// <summary>
        /// Frees GPU resources used by this pass.
        /// </summary>
        public abstract void Dispose();
    }
}