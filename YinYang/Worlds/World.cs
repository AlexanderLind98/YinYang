using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Lights;
using YinYang.Managers;
using YinYang.Particles;
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
        
        // public bool ShowSceneTexture => Game.showSceneTexture;
        // public bool ShowBloomTexture => Game.showBloomTexture;
        // public bool ShowVolumetricTexture => Game.showVolumetricTexture;


        private SceneRenderPass scenePass;
        private VolumetricLightPass volumetricLightPass;
        private BloomBlurPass blurPass;
        private BloomMipChain _bloomMipChain;
        private BloomDownsamplePass _bloomDownsamplePass;
        private BloomUpsamplePass _bloomUpsamplePass;
        private CompositePass compositePass;
        private BloomSettings bloomSettings = new();
        private CubeReflectionRenderPass cubeReflectionRenderPass;

        private bool bloomLinked = false;
        
        private bool bloomEnabled = true;
        private bool volumetricEnabled = true;


        
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
        //public Vector3 SunDirection = new Vector3(45f, 135f, 0f);
        public Vector3 SunDirection = new Vector3(0, 0, -1);

        /// <summary>1
        /// Default sun light color (also represents intensity).
        /// </summary>
        public Vector3 SunColor = new Vector3(1f, 1f, 1f);

        // Core manager systems for modular responsibilities.
        protected CameraManager cameraManager = new();
        protected ObjectManager objectManager = new();
        protected LightingManager lightingManager = new();
        protected RenderPipeline renderPipeline = new();
        protected ReflectionManager reflectionManager = new();
        
        public EditorTool? Editor;
        
        // Temporary pass-throughs for lighting TODO: Refactor to acces lightingManager directly or other way
        public DirectionalLight DirectionalLight => lightingManager.Sun;
        public List<PointLight> PointLights => lightingManager.PointLights;
        public List<SpotLight> SpotLights => lightingManager.SpotLights;

        public Camera MainCamera => cameraManager.Camera;

        // Temporary access to shadow map TODO: refcator to acces through renderpipeline
        public Texture depthMap => renderPipeline.ShadowDepthTexture;
        public Texture depthCubeMap => renderPipeline.ShadowDepthCubeTexture;
        public Texture reflectionCubeMap => renderPipeline.ReflectionCubeTexture;

        
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
            //lightingManager.InitializeDirectionalLightInDegrees(this, 0, 0, 0, SunColor, Light.ShadowType.Dynamic);

            lightingManager.InitializeDirectionalLight(this, SunDirection, SunColor);

            // Initialize modular render passes
            renderPipeline.AddPass(new ShadowRenderPass(lightingManager));
            renderPipeline.AddPass(new PointShadowRenderPass());
            //renderPipeline.AddPass(new SceneRenderPass());
            scenePass = new SceneRenderPass();
            renderPipeline.AddPass(scenePass);
            
            // Volumetric light pass (compute shader)
            volumetricLightPass = new VolumetricLightPass(Game.Size.X, Game.Size.Y);
            renderPipeline.AddPass(volumetricLightPass);
            
            // Initialize bloom mip chain
            _bloomMipChain = new BloomMipChain();
            _bloomMipChain.Init(Game.Size.X, Game.Size.Y, bloomSettings.MipLevels);
            
            // Create bloom passes
            _bloomDownsamplePass = new BloomDownsamplePass(_bloomMipChain);
            _bloomUpsamplePass = new BloomUpsamplePass(_bloomMipChain);

            compositePass = new CompositePass();

            // Add to pipeline
            renderPipeline.AddPass(_bloomDownsamplePass);
            renderPipeline.AddPass(_bloomUpsamplePass);
            renderPipeline.AddPass(compositePass);
            
            cubeReflectionRenderPass = new CubeReflectionRenderPass();
            renderPipeline.AddPass(cubeReflectionRenderPass);
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
            if (input.IsKeyPressed(Keys.B))
            {
                SetBloomEnabled(!bloomEnabled);
            }
            
            if (input.IsKeyPressed(Keys.V))
            {
                SetVolumetricEnabled(!volumetricEnabled);
            }

            
            // bloom strength
            if (input.IsKeyPressed(Keys.O))
                bloomSettings.BloomStrength = Math.Max(0.0f, bloomSettings.BloomStrength - 0.01f);

            if (input.IsKeyPressed(Keys.P))
                bloomSettings.BloomStrength = Math.Min(2.0f, bloomSettings.BloomStrength + 0.01f);

            // bloom threshold
            if (input.IsKeyPressed(Keys.K))
                bloomSettings.BloomThresholdMin = Math.Max(0.0f, bloomSettings.BloomThresholdMin - 0.05f);

            if (input.IsKeyPressed(Keys.L))
                bloomSettings.BloomThresholdMin = Math.Min(10.0f, bloomSettings.BloomThresholdMin + 0.05f);

            if (input.IsKeyPressed(Keys.N))
                bloomSettings.BloomThresholdMax = Math.Max(0.0f, bloomSettings.BloomThresholdMax - 0.05f);

            if (input.IsKeyPressed(Keys.M))
                bloomSettings.BloomThresholdMax = Math.Min(10.0f, bloomSettings.BloomThresholdMax + 0.05f);
            
            bloomSettings.ClampThresholds();
            
            // bloom filter radius
            if (input.IsKeyPressed(Keys.U))
                bloomSettings.FilterRadius = Math.Max(0.0f, bloomSettings.FilterRadius - 0.001f);

            if (input.IsKeyPressed(Keys.I))
                bloomSettings.FilterRadius = Math.Min(0.05f, bloomSettings.FilterRadius + 0.001f);
            
            // bloom exposure
            if (input.IsKeyPressed(Keys.T))
                bloomSettings.Exposure = Math.Max(0.01f, bloomSettings.Exposure - 0.01f);

            if (input.IsKeyPressed(Keys.Y))
                bloomSettings.Exposure = Math.Min(5.0f, bloomSettings.Exposure + 0.01f);
            
            // print bloom settings
            if (input.IsKeyPressed(Keys.D0)) 
            {
                Console.WriteLine(
                    "[BloomSettings]\n" +
                    $"Strength      = {bloomSettings.BloomStrength:0.00}\n" +
                    $"ThresholdMin  = {bloomSettings.BloomThresholdMin:0.00} - " +
                    $"ThresholdMax  = {bloomSettings.BloomThresholdMax:0.00}\n" +
                    $"FilterRadius  = {bloomSettings.FilterRadius:0.000}\n" +
                    $"Exposure      = {bloomSettings.Exposure:0.00}"
                );
            }
            
            cameraManager.HandleInput(input); 
        }
        
        /// <summary>
        /// Enables or disables the full bloom system (all passes + composite).
        /// </summary>
        private void SetBloomEnabled(bool enabled)
        {
            bloomEnabled = enabled;

            _bloomDownsamplePass.Enabled = enabled;
            _bloomUpsamplePass.Enabled = enabled;
            compositePass.SetBloomEnabled(enabled);

            Console.WriteLine(enabled ? "Bloom ENABLED" : "Bloom DISABLED");
        }

        private void SetVolumetricEnabled(bool enabled)
        {
            volumetricEnabled = enabled;
            volumetricLightPass.Enabled = enabled;
            compositePass.SetVolumetricEnabled(enabled); // We'll add this next
            Console.WriteLine(enabled ? "Volumetric Light ENABLED" : "Volumetric Light DISABLED");
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
            GL.Enable(EnableCap.FramebufferSrgb); // TODO: check if this is needed
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
                BloomSettings = bloomSettings,
                Reflection = reflectionManager
            };
            
            renderPipeline.RenderAll(context, objectManager);
            
            // render particles
            foreach (var go in GameObjects)
            {
                var ps = go.GetComponent<MagicParticleSystem>();
                ps?.DrawParticles(context);
            }


            // After scene pass has run, link bloom targets (if not yet done)
            if (!bloomLinked && scenePass.SceneColorTexture != 0 && scenePass.BrightColorTexture != 0)
            {
                _bloomDownsamplePass.InputTexture = scenePass.BrightColorTexture;

                compositePass.SceneTexture = scenePass.SceneColorTexture;
                compositePass.BloomTexture = _bloomMipChain.Mips[0].Texture;

                bloomLinked = true;

                // Console.WriteLine("[World] Linked bloom/composite inputs after FBO init.");
                // Console.WriteLine("[ScenePass] SceneTex: " + scenePass.SceneColorTexture + ", BrightTex: " + scenePass.BrightColorTexture);
                // Console.WriteLine("[BlurPass] InputBrightTexture: " + blurPass.InputBrightTexture);
                // Console.WriteLine("[Composite] Scene: " + compositePass.SceneTexture + ", Bloom: " + compositePass.BloomTexture);
            }
            
            if (debugOverlayEnabled)
            {
                _debugOverlay.Draw(reflectionCubeMap, new Vector2i(Game.Size.X, Game.Size.Y));
                // DrawDebugTexture(depthMap.Handle, Game.Size);
            }
            
            // Draw MRT debug textures 
            float scale = 0.25f;

            if (Game.showSceneTexture && scenePass.SceneColorTexture != 0)
            {
                // Nederst højre hjørne
                DrawDebugTexture(scenePass.SceneColorTexture, new Vector2(1.0f - scale, 0.0f), scale);
            }

            if (Game.showBloomTexture && scenePass.BrightColorTexture != 0)
            {
                // Lige ovenover scene texture 
                //DrawDebugTexture(scenePass.BrightColorTexture, new Vector2(1.0f - scale, scale), scale);
                //DrawDebugTexture(_bloomUpsamplePass.TargetTexture, new Vector2(1.0f - scale, scale), scale);
                
                // Show first (full-res) and last (softest) mip levels
                DrawDebugTexture(_bloomMipChain.Mips[0].Texture, new Vector2(1.0f - scale, scale * 1), scale);
                DrawDebugTexture(_bloomMipChain.Mips[^1].Texture, new Vector2(1.0f - scale, scale * 2), scale);
            }
            
            if (Game.showVolumetricTexture && volumetricLightPass != null && volumetricLightPass.VolumetricTexture != 0)
            {
                DrawDebugTexture(volumetricLightPass.VolumetricTexture, new Vector2(0.0f, scale * 1), scale);
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
            Console.WriteLine("debugOverlayEnabled debug: " + debugOverlayEnabled);
        }
    }
}