using YinYang;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using StbImageSharp;

static WindowIcon CreateIcon()
{
    Stream stream = File.OpenRead("YinYangEngine.png");
    ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
    var windowIcon = new WindowIcon(new Image(image.Width, image.Height, image.Data));

    return windowIcon;
}

GameWindowSettings settings = new GameWindowSettings()
{
    UpdateFrequency = 60.0
};

NativeWindowSettings nativeWindowSettings = new NativeWindowSettings()
{
    ClientSize = new Vector2i(1920, 1080),
    Title = "Yin Yang Engine | ",
    NumberOfSamples = 4,
    Icon = CreateIcon()
};

/*
[System.Runtime.InteropServices.DllImport("kernel32.dll")]
static extern bool SetEnvironmentVariable(string lpName, string lpValue);

// Force NVIDIA GPU
SetEnvironmentVariable("SHIM_MCCOMPAT", "0x800000001");
SetEnvironmentVariable("NV_OPTIMUS_ENABLEMENT", "0x00000001");

SetEnvironmentVariable("AMD_POWERXPRESS_REQUEST_HIGH_PERFORMANCE", "1");
*/

// GLFW.WindowHint(WindowHintInt.Samples, 4);
using Game game = new Game(settings, nativeWindowSettings);
game.Run();