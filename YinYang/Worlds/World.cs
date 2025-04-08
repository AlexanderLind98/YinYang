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
        private bool debugOverlayEnabled = false;
        private DebugOverlay _debugOverlay;
        
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
            _debugOverlay = new DebugOverlay();

            // Initialize the camera as a game object and grab the cursor.
            cameraManager.Setup(Game, objectManager.GameObjects);

            // Set up lighting system including directional sunlight.
            lightingManager.InitializeDirectionalLight(this, SunDirection, SunColor);

            // Initialize modular render passes
            renderPipeline.AddPass(new ShadowRenderPass());
            renderPipeline.AddPass(new SceneRenderPass());
            renderPipeline.HdrPass = new HDRRenderPass();
            renderPipeline.AddPass(renderPipeline.HdrPass);
            
            // post-processing.
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
            var context = new RenderContext
            {
                Camera = cameraManager.Camera,
                Lighting = lightingManager,
                World = this,
                ViewProjection = cameraManager.GetViewProjection(),
                LightSpaceMatrix = Matrix4.Identity, // placeholder to start
                DebugMode = debugMode,
            };
            
            renderPipeline.RenderAll(context, objectManager);
            
            if (debugOverlayEnabled)
            {
                _debugOverlay.Draw(depthMap, new Vector2i(Game.Size.X, Game.Size.Y));
            }
        }

        /// <summary>
        /// Called when the world is being disposed or switched.
        /// </summary>
        public void UnloadWorld()
        {
            objectManager.Dispose();
            renderPipeline.Dispose();
            _debugOverlay.Dispose();
        }
        
        /// <summary>
        /// Renders a 2D texture (such as color, brightness, or intermediate buffers) to the screen
        /// using a screen-space debug overlay. This is intended for visual debugging of render targets
        /// within any modular render pass.
        /// </summary>
        /// <param name="textureHandle">
        /// Name of the texture to be displayed. 
        /// </param>
        /// <param name="viewportPos">
        /// The normalized screen-space coordinates (X, Y) where the texture should appear.
        /// Values are in the range [0, 1], where (0,0) is bottom-left and (1,1) is top-right of the screen.
        /// </param>
        /// <param name="scale">
        /// The relative size of the displayed texture, as a fraction of the screen resolution.
        /// </param>
        /// <remarks>
        /// This method should typically be called from within a render pass, gated by a condition like:
        /// <code>
        /// if (context.DebugMode == 3)
        ///     context.World.DrawDebugTexture(myTextureID, new Vector2(0.75f, 0.0f));
        /// </code>
        /// Only color textures are supported by this method. For depth maps, use <c>Draw(Texture depthMap, Vector2i screenSize)</c>.
        /// </remarks>
        public void DrawDebugTexture(int textureHandle, Vector2 viewportPos, float scale = 0.25f)
        {
            _debugOverlay.DrawTexture(textureHandle, new Vector2i(Game.Size.X, Game.Size.Y), viewportPos, scale);
        }
        
        public void ToggleDebugOverlay()
        {
            debugOverlayEnabled = !debugOverlayEnabled;
        }
    }
}