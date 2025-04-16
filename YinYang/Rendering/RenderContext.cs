using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Lights;
using YinYang.Worlds;

namespace YinYang.Rendering
{
    /// <summary>
    /// Encapsulates rendering context data passed to render passes and renderers.
    /// </summary>
    /// <remarks>
    /// This class acts kinda like a per-frame DTO (Data Transfer Object) that groups together
    /// camera, lighting, matrices, and other shared rendering state. For better method signatures.
    /// </remarks>
    public class RenderContext
    {
        /// <summary>The camera rendering the current frame.</summary>
        public Camera Camera { get; init; }

        /// <summary>Lighting manager containing sun, point lights, and spot lights.</summary>
        public LightingManager Lighting { get; init; }

        /// <summary>World reference, if needed for skybox, fog, etc.</summary>
        public World World { get; init; }

        /// <summary>The view-projection matrix from the camera.</summary>
        public Matrix4 ViewProjection { get; init; }

        /// <summary>The light-space matrix from the latest shadow pass.</summary>
        public Matrix4 LightSpaceMatrix { get; set; }

        /// <summary>Debug drawing mode, if enabled.</summary>
        public int DebugMode { get; init; }
        
        /// <summary>Bloom settings</summary>
        public BloomSettings BloomSettings { get; init; }
        
        public ReflectionManager Reflection { get; init; }
    }
}