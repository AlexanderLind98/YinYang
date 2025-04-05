using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace YinYang.Behaviors;

/// <summary>
/// Behavior class adding movement and mouse look to a camera game object.
/// </summary>
public class CamMoveBehavior(GameObject gameObject, Game window) : Behaviour(gameObject, window)
{
    private Camera cameraComponent;
    
    private Vector3 front = new Vector3(0.0f, 0.0f, -1.0f);
    private Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
    private float speed = 10.0f;
    private float sensitivity = 10.0f;

    public override void Update(FrameEventArgs args)
    {
        if (cameraComponent == null)
            cameraComponent = gameObject.GetComponent<Camera>();

        if (cameraComponent == null)
        {
            Console.WriteLine("CamMoveBehavior: Camera component not found.");
            return;
        }
        
        KeyboardState input = window.KeyboardState;
        MouseState mouse = window.MouseState;

        front = cameraComponent.Front;
        up = cameraComponent.Up;

        float deltaX = mouse.Delta.X * sensitivity * (float)args.Time;
        float deltaY = mouse.Delta.Y * sensitivity * (float)args.Time;
        
        if (deltaX != 0 || deltaY != 0)
        {
            Vector3 rotation = gameObject.Transform.Rotation;
            
            rotation.Y += deltaX;  //Rotate left/right
            rotation.X -= deltaY;  //Rotate up/down (invert Y-axis)
            
            rotation.X = Math.Clamp(rotation.X, -89f, 89f);

            //Apply rotation
            gameObject.Transform.Rotation = rotation;

            //Recalc vectors used in movement
            front.X = MathF.Cos(MathHelper.DegreesToRadians(rotation.Y)) * MathF.Cos(MathHelper.DegreesToRadians(rotation.X));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(rotation.X));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(rotation.Y)) * MathF.Cos(MathHelper.DegreesToRadians(rotation.X));
            front = Vector3.Normalize(front);
            
            
            Vector3 right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }
        
        cameraComponent.Front = front;
        //Console.WriteLine(front.ToString());
        cameraComponent.Up = up;
        
        if (input.IsKeyDown(Keys.W))
        {
            gameObject.Transform.Position += front * speed* (float)args.Time; //Forward 
        }
        if (input.IsKeyDown(Keys.S))
        {
            gameObject.Transform.Position -= front * speed* (float)args.Time; //Backwards
        }
        if (input.IsKeyDown(Keys.A))
        {
            gameObject.Transform.Position -= Vector3.Normalize(Vector3.Cross(front, up)) * speed* (float)args.Time; //Left
        }
        if (input.IsKeyDown(Keys.D))
        {
            gameObject.Transform.Position += Vector3.Normalize(Vector3.Cross(front, up)) * speed * (float)args.Time; //Right
        }
        if (input.IsKeyDown(Keys.Space))
        {
            gameObject.Transform.Position += up * speed * (float)args.Time; //Up 
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            gameObject.Transform.Position -= up * speed * (float)args.Time; //Down
        }

        if (input.IsKeyPressed(Keys.F))
        {
            window.currentWorld.SpotLights[0].ToggleLight();
        }
    }
}