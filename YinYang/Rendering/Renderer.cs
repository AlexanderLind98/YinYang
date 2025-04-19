using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using YinYang.Materials;

namespace YinYang.Rendering
{
    /// <summary>
    /// Responsible for rendering a mesh using its associated material and the current render context.
    /// </summary>
    /// <remarks>
    /// Lighting and transformation logic moved to the material, enabling clean SOC.
    /// </remarks>
    public class Renderer
    {
        private Material _material;
        private bool DepthTest = true;

        public Material Material
        {
            get => _material;
            set
            {
                if (_material != null && _material != value)
                    (_material as IDisposable)?.Dispose();

                _material = value;
            }
        }

        public Mesh Mesh { get; set; }
        public bool RenderInDepthPass { get; set; } = true;
        public bool RenderInScenePass { get; set; } = true;

        public Renderer(Material material, Mesh mesh)
        {
            Material = material;
            Mesh = mesh;
        }

        /// <summary>
        /// Draws the mesh with the associated material using the provided rendering context and transforms.
        /// </summary>
        /// <param name="context">Shared per-frame render data (camera, lights, world, etc).</param>
        /// <param name="mvp">Combined model-view-projection matrix.</param>
        /// <param name="model">Local model transform matrix.</param>
        public void Draw(RenderContext context, Matrix4 mvp, Matrix4 model)
        {
            Material.UseShader();
            Material.UpdateUniforms();
            
            // Set transform + camera data
            Material.SetUniform("mvp", mvp);
            Material.SetUniform("model", model);
            Material.SetUniform("normalMatrix", Matrix4.Invert(model));
            Material.SetUniform("viewPos", context.Camera.Position);
            Material.SetUniform("debugMode", context.DebugMode);
            Material.SetUniform("time", context.Time);

            if (Material.UsesLighting)
                Material.PrepareLighting(context);

            if (Material.IsReflective)
                Material.SetupReflections(context);

            Mesh.Draw();
        }

        /// <summary>
        /// Draws the mesh using only depth information, used in shadow passes.
        /// </summary>
        public void RenderDepth(Shader shader, Matrix4 model)
        {
            //Ability to remove objects from appearing in depth pass, to remove shadows
            if(!RenderInDepthPass)
                return;
            
            shader.SetMatrix("model", model);
            GL.DepthMask(DepthTest);
            Mesh.Draw();
            GL.DepthMask(true);
        }
    }
}