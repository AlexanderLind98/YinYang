using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using YinYang.Behaviors;

namespace YinYang
{
    public class Camera : Behaviour
    {
        public Vector3 Front = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
        private float FOV;
        private float aspectX;
        private float aspectY;
        private float near;
        private float far;
        
        public Vector3 Position => gameObject.Transform.Position;
        public GameObject GameObject => gameObject;
        
        private readonly GameWindow window;

        public int RenderWidth => window.Size.X;
        public int RenderHeight => window.Size.Y;

        public Camera(GameObject gameObject, Game window, float FOV, float aspectX, float aspectY,float near,float far) : base(gameObject, window)
        {
            this.window = window;
            
            gameObject.Transform.Position = new Vector3(0.0f, 0.0f, 5.0f);
            gameObject.Transform.Rotation = new Vector3(0.0f, -90.0f, 0.0f);
            this.FOV = FOV;
            this.aspectX = aspectX;
            this.aspectY = aspectY;
            this.near = near;
            this.far = far;
        }

        public override void Update(FrameEventArgs args)
        { 
            //Movement moved -> CamMoveBehavior.cs
        }
        
        public virtual void HandleInput(KeyboardState input) { }

        public Matrix4 GetViewProjection()
        {
            Matrix4 view = Matrix4.LookAt(gameObject.Transform.Position, gameObject.Transform.Position + Front, Up);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), aspectX / aspectY, near, far);

            return view * projection;
        }

        public Matrix4 GetView()
        {
            return Matrix4.LookAt(gameObject.Transform.Position, gameObject.Transform.Position + Front, Up);
        }
        
        public Matrix4 GetProjection()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), aspectX / aspectY, near, far);
        }
    }
}