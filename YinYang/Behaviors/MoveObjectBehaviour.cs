using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace YinYang.Behaviors
{
    public class MoveObjectBehaviour : Behaviour
    {
        // Movement speed in units per second
        private float movementSpeed = 25.0f;
        
        public MoveObjectBehaviour(GameObject gameObject, Game window)
            : base(gameObject, window)
        {
        }
        
        public override void Update(FrameEventArgs args)
        {
            KeyboardState input = window.KeyboardState;
            var pos = gameObject.Transform.Position; // get current position

            // Move left/right along X axis
            if (input.IsKeyDown(Keys.Left))
            {
                pos.X -= movementSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                pos.X += movementSpeed * (float)args.Time;
            }
            
            // Move forward/backward along Z axis
            if (input.IsKeyDown(Keys.Up))
            {
                pos.Z += movementSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                pos.Z -= movementSpeed * (float)args.Time;
            }
            
            // Move up/down along Y axis using Q (down) and E (up)
            if (input.IsKeyDown(Keys.Q))
            {
                pos.Y -= movementSpeed * (float)args.Time;
            }
            if (input.IsKeyDown(Keys.E))
            {
                pos.Y += movementSpeed * (float)args.Time;
            }
            
            // Reassign the modified position back to the transform.
            gameObject.Transform.Position = pos;
        }
    }
}