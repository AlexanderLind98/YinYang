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
    private Vector3 currentRotationDegrees = gameObject.Transform.GetRotationInDegrees();
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

        if (window.CursorState == CursorState.Grabbed && (deltaX != 0 || deltaY != 0))
        {
            // Update local rotation cache
            currentRotationDegrees.Y += deltaX;
            currentRotationDegrees.X -= deltaY;
            currentRotationDegrees.X = Math.Clamp(currentRotationDegrees.X, -89f, 89f);

            // Apply to transform
            gameObject.SetRotationInDegrees(currentRotationDegrees.X, currentRotationDegrees.Y, currentRotationDegrees.Z);

            // Recalculate front vector based on current degrees
            float pitchRad = MathHelper.DegreesToRadians(currentRotationDegrees.X);
            float yawRad = MathHelper.DegreesToRadians(currentRotationDegrees.Y);

            front.X = MathF.Cos(pitchRad) * MathF.Cos(yawRad);
            front.Y = MathF.Sin(pitchRad);
            front.Z = MathF.Cos(pitchRad) * MathF.Sin(yawRad);
            front = Vector3.Normalize(front);

            Vector3 right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }

        cameraComponent.Front = front;
        cameraComponent.Up = up;

        // Movement
        float step = speed * (float)args.Time;
        if (input.IsKeyDown(Keys.W)) gameObject.Transform.Position += front * step;
        if (input.IsKeyDown(Keys.S)) gameObject.Transform.Position -= front * step;
        if (input.IsKeyDown(Keys.A)) gameObject.Transform.Position -= Vector3.Normalize(Vector3.Cross(front, up)) * step;
        if (input.IsKeyDown(Keys.D)) gameObject.Transform.Position += Vector3.Normalize(Vector3.Cross(front, up)) * step;
        if (input.IsKeyDown(Keys.Space)) gameObject.Transform.Position += up * step;
        if (input.IsKeyDown(Keys.LeftShift)) gameObject.Transform.Position -= up * step;

        if (input.IsKeyPressed(Keys.F))
        {
            window.currentWorld.SpotLights[0].ToggleLight();
        }
    }
}