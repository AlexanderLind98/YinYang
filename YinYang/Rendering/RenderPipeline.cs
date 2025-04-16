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
        
        /// <summary>
        /// Temporary passthrough to access shadow depth texture.
        /// </summary>
        public Texture ShadowDepthTexture =>
            renderPasses.OfType<ShadowRenderPass>().FirstOrDefault()?.ShadowDepthTexture;

        public Texture ShadowDepthCubeTexture =>
            renderPasses.OfType<PointShadowRenderPass>().FirstOrDefault()?.ShadowDepthCubeTexture;
        
        public Texture ReflectionCubeTexture =>
            renderPasses.OfType<CubeReflectionRenderPass>().FirstOrDefault()?.ReflectionCubeTexture;
        
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
        /// 
        /// <param name="objects">Objects to render.</param> // TODO: maybe decouple objectmanger and use list or delegate for objects to render
        /// <returns>The last computed light-space matrix, if any.</returns>
        public void RenderAll(RenderContext context, ObjectManager objects)
        {
            Matrix4? lightSpaceMatrix = null;

            foreach (var pass in renderPasses)
            {
                if (!pass.Enabled) continue;
                
                lightSpaceMatrix = pass.Execute(context, objects)!;
                
                if(lightSpaceMatrix != null)
                    context.LightSpaceMatrix = (Matrix4)lightSpaceMatrix;
            }
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