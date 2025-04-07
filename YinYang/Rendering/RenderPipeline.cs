using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Worlds;

namespace YinYang.Rendering
{
    /// <summary>
    /// Coordinates rendering by managing a sequence of modular render passes.
    /// </summary>
    /// <remarks>
    /// This version of the RenderPipeline supports modular render passes that can be chained and executed in sequence.
    /// </remarks>
    public class RenderPipeline
    {
        private readonly List<RenderPass> renderPasses = new();
        
        public HDRRenderPass HdrPass { get; set; }
        public BloomRenderPass BloomPass { get; set; }

        
        /// <summary>
        /// Temporary passthrough to access shadow depth texture.
        /// </summary>
        public Texture ShadowDepthTexture =>
            renderPasses.OfType<ShadowRenderPass>().FirstOrDefault()?.ShadowDepthTexture;

        /// <summary>
        /// Adds a render pass to the pipeline.
        /// </summary>
        /// <param name="pass">The render pass to add.</param>
        public void AddPass(RenderPass pass)
        {
            renderPasses.Add(pass);
        }

        /// <summary>
        /// Executes all render passes in the order they were added.
        /// </summary>
        /// <param name="camera">The camera providing view/projection data.</param>
        /// <param name="lighting">Scene lighting information.</param>
        /// <param name="objects">Objects to render.</param> // TODO: maybe decouple objectmanger and use list or delegate for objects to render
        /// <param name="currentWorld">The current world instance.</param>
        /// <param name="debugMode">The current debug mode.</param>
        /// <returns>The last computed light-space matrix, if any.</returns>
        public Matrix4 RenderAll(RenderContext context, ObjectManager objects)
        {
            Matrix4 lightSpaceMatrix = Matrix4.Identity;

            foreach (var pass in renderPasses)
            {
                if (!pass.Enabled) continue;
                
                lightSpaceMatrix = pass.Execute(context, objects);
                context.LightSpaceMatrix = lightSpaceMatrix;

            }

            return lightSpaceMatrix;
        }


        /// <summary>
        /// Disposes all render passes in the pipeline.
        /// </summary>
        public void Dispose()
        {
            foreach (var pass in renderPasses)
            {
                pass.Dispose();
            }
        }
    }
}