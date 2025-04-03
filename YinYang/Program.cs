using YinYang;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


GameWindowSettings settings = new GameWindowSettings()
{
    UpdateFrequency = 60.0
};

NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
{
    ClientSize = new Vector2i(1920, 1080),
    Title = "OBJ Viewer",
    NumberOfSamples = 4
};

// GLFW.WindowHint(WindowHintInt.Samples, 4);
using Game game = new Game(settings, nativeWindowSettings);
game.Run();