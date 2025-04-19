using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using YinYang.Managers;
using YinYang.Worlds;

namespace YinYang.Behaviors.CollisionEvents;

public class ActivateLightEvent : CollisionEvent
{
    private bool lerping;
    private bool backwards;
    private float lerpTime = 1.0f; // duration in seconds
    private float lerpProgress = 0f;

    //Hardcoded colors
    private Color4 ambientOffColor = Color4.Black;
    private Color4 ambientOnColor = Color4.CornflowerBlue;
    private Vector3 sunOffColor = new Vector3(0, 0, 0);
    private Vector3 sunOnColor = new Vector3(5, 5, 2.5f);
    
    World world;
    
    public ActivateLightEvent(World world)
    {
        DoesUpdate = true;
        lerping = false;
        backwards = true;
        
        this.world = world;
    }
    
    public override void Trigger()
    {
        lerping = true;
        lerpProgress = 0f;
        backwards = !backwards;
    }

    public override void Update(float deltaTime)
    {
        if(!lerping)
            return;
        
        //TODO: Lerp lighting and ambiance
        Console.WriteLine("Lerping");
        
        lerpProgress += deltaTime / lerpTime; // Replace Time.DeltaTime with your delta time

        float t = Math.Clamp(lerpProgress, 0f, 1f);
        if (backwards)
            t = 1f - t;

        // Lerp between ambient light colors
        Color4 ambient = LerpColor(ambientOffColor, ambientOnColor, t);
        Vector3 sunlight = Vector3.Lerp(sunOffColor, sunOnColor, t);

        // Apply lighting here
        world.DirectionalLight.LightColor = sunlight;
        world.SkyColor = ambient;
        GL.ClearColor(ambient);

        if (lerpProgress >= 1f)
        {
            lerping = false;
        }
    }
    
    private Color4 LerpColor(Color4 a, Color4 b, float t)
    {
        return new Color4(
            MathHelper.Lerp(a.R, b.R, t),
            MathHelper.Lerp(a.G, b.G, t),
            MathHelper.Lerp(a.B, b.B, t),
            MathHelper.Lerp(a.A, b.A, t)
        );
    }

}