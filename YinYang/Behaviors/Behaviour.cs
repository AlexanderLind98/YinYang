using OpenTK.Windowing.Common;

namespace YinYang.Behaviors
{
    public abstract class Behaviour
    {
        protected GameObject gameObject;
        protected Game window; 
        public Behaviour(GameObject gameObject, Game window)
        {
            this.gameObject = gameObject;
            this.window = window;
        }

        public abstract void Update(FrameEventArgs args);
    }
}