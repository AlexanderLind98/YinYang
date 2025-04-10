using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace YinYang.Behaviors;

public class EditorBehavior(GameObject gameObject, Game window) : Behaviour(gameObject, window)
{
    private float speed = 0.1f;
    private Vector3 scaleFactor = new Vector3(0.1f);
    
    public override void Update(FrameEventArgs args)
    {
        KeyboardState keyState = window.KeyboardState;

        speed = keyState.IsKeyDown(Keys.RightShift) ? 0.5f : 0.1f; //Double speed
        
        if(!keyState.IsKeyDown(Keys.RightShift))
            speed = keyState.IsKeyDown(Keys.RightControl) ? 0.01f : 0.1f; //Half speed

        if (keyState.IsKeyDown(Keys.Up)) //Forwards
        {
            gameObject.Transform.Position -= (Vector3.UnitZ * speed);
        }
        
        if(keyState.IsKeyDown(Keys.Down)) //Backwards
        {
            gameObject.Transform.Position += (Vector3.UnitZ * speed);
        }

        if (keyState.IsKeyDown(Keys.Left)) //Left
        {
            gameObject.Transform.Position -= (Vector3.UnitX * speed);
        }

        if (keyState.IsKeyDown(Keys.Right)) //Right
        {
            gameObject.Transform.Position += (Vector3.UnitX * speed);
        }

        if (keyState.IsKeyDown(Keys.PageUp)) //Up
        {
            gameObject.Transform.Position += (Vector3.UnitY * speed);
        }

        if (keyState.IsKeyDown(Keys.PageDown)) //Down
        {
            gameObject.Transform.Position -= (Vector3.UnitY * speed);
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
            gameObject.Transform.Rotation += (Vector3.UnitY * speed);
        }

        if (keyState.IsKeyDown(Keys.KeyPad6)) //Subtract yaw
        {
            gameObject.Transform.Rotation -= (Vector3.UnitY * speed);
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

        if (keyState.IsKeyPressed(Keys.Enter))
        {
            Console.WriteLine("EditorBehavior - Position is: " + gameObject.Transform.Position);
            Console.WriteLine("EditorBehavior - Scale is: " + gameObject.Transform.Scale);
            Console.WriteLine("EditorBehavior - Rotation is: " + gameObject.Transform.GetRotationInDegrees());
        }
    }
}