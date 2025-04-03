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

// GLFW.WindowHint(WindowHintInt.Samples, 4);
using Game game = new Game(settings, nativeWindowSettings);
game.Run();