using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Rendering;

namespace YinYang.Worlds
{
    /// <summary>
    /// Represents a high-level simulation world that coordinates game object updates and rendering.
    /// </summary>
    /// <remarks>
    /// Delegates rendering and system logic to manager classes to ensure modular design.
    /// </remarks>
    public abstract class World
    {
        /// <summary>
        /// Reference to the core game instance.
        /// </summary>
        public readonly Game Game;

        /// <summary>
        /// A label shown in debug UI.
        /// </summary>
        public virtual string DebugLabel => "Combined";

        /// <summary>
        /// Identifier name for the world (used when switching scenes).
        /// </summary>
        public string WorldName { get; set; }

        /// <summary>
        /// The sky color used to clear the screen and influence ambient lighting.
        /// </summary>
        public Color4 SkyColor = Color4.CornflowerBlue;

        /// <summary>
        /// Default sun direction vector (used at startup).
        /// </summary>
        public Vector3 SunDirection = new Vector3(-0.2f, -1.0f, -0.3f);

        /// <summary>
        /// Default sun light color (also represents intensity).
        /// </summary>
        public Vector3 SunColor = new Vector3(2f, 2f, 1.8f);

        // Core manager systems for modular responsibilities.
        protected CameraManager cameraManager = new();
        protected ObjectManager objectManager = new();
        protected LightingManager lightingManager = new();
        protected RenderPipeline renderPipeline = new();
        
        // Temporary pass-throughs for lighting TODO: Refactor to acces lightingManager directly or other way
        public DirectionalLight DirectionalLight => lightingManager.Sun;
        public List<PointLight> PointLights => lightingManager.PointLights;
        public List<SpotLight> SpotLights => lightingManager.SpotLights;

        // Temporary access to shadow map TODO: refcator to acces through renderpipeline
        public Texture depthMap => renderPipeline.ShadowDepthTexture;

        
        // Temporary access to game objects TODO: refactor to access through objectManager
        public List<GameObject> GameObjects => objectManager.GameObjects;


        /// <summary>
        /// Constructs a new world, initializing cameras, lights, and the shadow pipeline.
        /// </summary>
        /// <param name="game">The game instance owning this world.</param>
        protected World(Game game)
        {
            Game = game;

            // Initialize the camera as a game object and grab the cursor.
            cameraManager.Setup(Game, objectManager.GameObjects);

            // Set up lighting system including directional sunlight.
            lightingManager.InitializeDirectionalLight(this, SunDirection, SunColor);

            // Initialize modular render passes
            renderPipeline.AddPass(new ShadowRenderPass());
            renderPipeline.AddPass(new SceneRenderPass());
            
            // Add a post-processing pass for HDR rendering.
            renderPipeline.HdrPass = new HDRRenderPass();
            renderPipeline.AddPass(renderPipeline.HdrPass);
            //renderPipeline.AddPass(new HDRRenderPass());
        }

        /// <summary>
        /// Constructs the contents of the world (models, lights, etc).
        /// </summary>
        protected virtual void ConstructWorld() { }

        /// <summary>
        /// Optional input handling hook (called every frame).
        /// </summary>
        /// <param name="input">Keyboard state snapshot.</param>
        public virtual void HandleInput(KeyboardState input)
        {
            cameraManager.HandleInput(input); 
        }

        /// <summary>
        /// Returns the sky color vector (used for ambient light calculation).
        /// </summary>
        public Vector3 GetSkyColor() => new(SkyColor.R, SkyColor.G, SkyColor.B);

        /// <summary>
        /// Returns the sky color with an added intensity (for ambient variation).
        /// </summary>
        public Vector3 GetSkyColor(float intensity)
        {
            return new Vector3(SkyColor.R + intensity, SkyColor.G + intensity, SkyColor.B + intensity);
        }
       
        /// <summary>
        /// Called once to prepare all OpenGL state and build world contents.
        /// </summary>
        public void LoadWorld()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.FramebufferSrgb);
            GL.ClearColor(SkyColor);

            ConstructWorld();
        }

        /// <summary>
        /// Called every frame to update object behavior.
        /// </summary>
        /// <param name="args">Frame timing arguments.</param>
        public void UpdateWorld(FrameEventArgs args)
        {
            objectManager.Update(args);
            cameraManager.Update(args);
        }

        /// <summary>
        /// Called every frame to render the world, including shadows and scene objects.
        /// </summary>
        /// <param name="args">Frame timing arguments.</param>
        /// <param name="debugMode">Active debug mode for shader configuration.</param>
        public void DrawWorld(FrameEventArgs args, int debugMode)
        {
            // Execute all render passes configured in the render pipeline using the current world instance.
            renderPipeline.RenderAll(cameraManager.Camera, lightingManager, objectManager, this, debugMode);
        }

    

        /// <summary>
        /// Called when the world is being disposed or switched.
        /// </summary>
        public void UnloadWorld()
        {
            objectManager.Dispose();
            renderPipeline.Dispose();
        }
    }
}