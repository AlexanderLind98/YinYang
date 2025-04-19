using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Worlds;

// For TriangleMesh and CubeMesh

namespace YinYang
{
    public class Game : GameWindow
    {
        public readonly World currentWorld;
        public int DebugMode { get; set; } = 0;
        public bool ShowSceneTexture = false;
        public bool ShowBloomTexture = false;
        public bool ShowVolumetricTexture = false;

        

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            CenterWindow();
            GL.ClearColor(Color4.Black);
            
            currentWorld = new GodRayTestWorld(this);
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            
            currentWorld.LoadWorld();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            KeyboardState input = KeyboardState;
            currentWorld.HandleInput(input); 

            currentWorld.UpdateWorld(args);

            /*if (input.IsKeyPressed(Keys.F1)) SwitchWorld(1);
            if (input.IsKeyPressed(Keys.F2)) SwitchWorld(2);
            if (input.IsKeyPressed(Keys.F3)) SwitchWorld(3);
            if (input.IsKeyPressed(Keys.F4)) SwitchWorld(4);
            if (input.IsKeyPressed(Keys.F5)) SwitchWorld(5);
            if (input.IsKeyPressed(Keys.F6)) SwitchWorld(6);*/


            // Shader Debug mode switch
            if (input.IsKeyPressed(Keys.D1)) DebugMode = 1;
            if (input.IsKeyPressed(Keys.D2)) DebugMode = 2;
            if (input.IsKeyPressed(Keys.D3)) DebugMode = 3;
            if (input.IsKeyPressed(Keys.D4)) DebugMode = 0;

            if (input.IsKeyPressed(Keys.Escape))
            {
                if(CursorState == CursorState.Grabbed)
                {
                    CursorState = CursorState.Normal;
                    return;
                }
                
                if(CursorState == CursorState.Normal)
                {
                    if(currentWorld.Editor is { IsEditingObject: true })
                        currentWorld.Editor.CommitObject(true);
                    
                    Close();
                }
            }

            if (MouseState.IsButtonDown(MouseButton.Left))
            {
                CursorState = CursorState.Grabbed;
            }

            if (input.IsKeyPressed(Keys.D5))
            {
                currentWorld.ToggleDebugOverlay();
            }
            
            if (input.IsKeyPressed(Keys.D6))
            {
                ShowSceneTexture = !ShowSceneTexture;
                Console.WriteLine("SceneColorTexture debug: " + ShowSceneTexture);
            }

            if (input.IsKeyPressed(Keys.D7))
            {
                ShowBloomTexture = !ShowBloomTexture;
                Console.WriteLine("BrightColorTexture debug: " + ShowBloomTexture);
            }
            
            if (input.IsKeyPressed(Keys.D8))
            {
                ShowVolumetricTexture = !ShowVolumetricTexture;
                Console.WriteLine("VolumetricTexture debug: " + ShowVolumetricTexture);
            }

            Title = $"{currentWorld.WorldName} | {currentWorld.DebugLabel}";
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            
            currentWorld.DrawWorld(args, DebugMode);
            
            
            SwapBuffers();
        }

        protected override void OnUnload()
        {
            currentWorld.UnloadWorld();
            
            base.OnUnload();
        }
        
        private void SwitchWorld(int index)
        {
            currentWorld.UnloadWorld();

            /*currentWorld = index switch
            {
                1 => new SimpleShapesWorld(this),
                2 => new LightTestWorld(this),
                3 => new PointLightWorld(this),
                4 => new SpotLightWorld(this),
                5 => new MultiLightTestWorld(this),
                _ => new SimpleShapesWorld(this)
            };*/

            currentWorld.LoadWorld();
        }
    }
}