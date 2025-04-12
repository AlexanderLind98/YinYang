using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace YinYang.Behaviors;

public class EditorBehavior : Behaviour
{
    public EditorBehavior(GameObject gameObject, Game window)
        : base(gameObject, window)
    {
        camera = window.currentWorld.MainCamera;
    }
    
    private float speed = 0.1f;
    private Vector3 scaleFactor = new Vector3(0.1f);
    
    private Camera camera;
    
    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 camUp;
    
    public override void Update(FrameEventArgs args)
    {
        camForward = new Vector3(camera.Front.X, 0, camera.Front.Z);
        
        // Avoid zero-length vectors
        if (camForward.LengthSquared < 0.0001f)
        {
            camForward = -Vector3.UnitZ; // Default fallback (e.g., facing "forward" in world space)
        }
        else
        {
            camForward = camForward.Normalized();
        }
        camUp = camera.Up.Normalized();
        camRight = Vector3.Cross(camForward, camUp).Normalized();
        if (camRight.LengthSquared < 0.0001f)
        {
            camRight = -Vector3.UnitX; // Default fallback (e.g., facing "forward" in world space)
        }
        else
        {
            camRight = camRight.Normalized();
        }
        
        KeyboardState keyState = window.KeyboardState;

        speed = keyState.IsKeyDown(Keys.RightShift) ? 0.5f : 0.1f; //Double speed
        
        if(!keyState.IsKeyDown(Keys.RightShift))
            speed = keyState.IsKeyDown(Keys.RightControl) ? 0.01f : 0.1f; //Half speed

        if (keyState.IsKeyDown(Keys.Up)) //Forwards
        {
            gameObject.Transform.Position += (camForward * speed);
        }
        
        if(keyState.IsKeyDown(Keys.Down)) //Backwards
        {
            gameObject.Transform.Position -= (camForward * speed);
        }

        if (keyState.IsKeyDown(Keys.Left)) //Left
        {
            gameObject.Transform.Position -= (camRight * speed);
        }

        if (keyState.IsKeyDown(Keys.Right)) //Right
        {
            gameObject.Transform.Position += (camRight * speed);
        }

        if (keyState.IsKeyDown(Keys.PageUp)) //Up
        {
            gameObject.Transform.Position += (camUp * speed);
        }

        if (keyState.IsKeyDown(Keys.PageDown)) //Down
        {
            gameObject.Transform.Position -= (camUp * speed);
        }
        
        //Rotation
        if (keyState.IsKeyDown(Keys.KeyPad8)) //Add pitch
        {
            gameObject.Transform.Rotation += (Vector3.UnitX * speed);
        }
        
        if (keyState.IsKeyDown(Keys.KeyPad2)) //Subtract pitch
        {
            gameObject.Transform.Rotation -= (Vector3.UnitX * speed);
        }

        if (keyState.IsKeyDown(Keys.KeyPad4)) //Add yaw
        {
            gameObject.Transform.Rotation -= (Vector3.UnitY * speed);
        }

        if (keyState.IsKeyDown(Keys.KeyPad6)) //Subtract yaw
        {
            gameObject.Transform.Rotation += (Vector3.UnitY * speed);
        }

        if (keyState.IsKeyDown(Keys.KeyPad7)) //Add roll
        {
            gameObject.Transform.Rotation += (Vector3.UnitZ * speed);
        }

        if (keyState.IsKeyDown(Keys.KeyPad9)) //Subtract roll
        {
            gameObject.Transform.Rotation -= (Vector3.UnitZ * speed);
        }

        if (keyState.IsKeyDown(Keys.KeyPad5)) //Reset rotation
        {
            gameObject.Transform.Rotation = Vector3.Zero;
        }
        
        //Scale
        if (keyState.IsKeyDown(Keys.KeyPadAdd)) //Smaller
        {
            gameObject.Transform.Scale += (scaleFactor * speed);
        }
        
        if (keyState.IsKeyDown(Keys.KeyPadSubtract)) //Bigger
        {
            gameObject.Transform.Scale -= (scaleFactor * speed);
        }
    }
}