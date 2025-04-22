using System.Data.Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using YinYang.Behaviors.CollisionEvents;

namespace YinYang.Behaviors;

public class DistCollider : Behaviour
{
    private Vector3 location;
    private float radius = 8.0f;
    private bool active = true;
    private bool flipped = false;
    
    public CollisionEvent OnCollision; //TODO: change if we need more collision events
    
    public DistCollider(GameObject gameObject, Game window) : base(gameObject, window)
    {
        location = gameObject.Transform.Position;
        OnCollision = new ActivateLightEvent(window.currentWorld);
    }

    public DistCollider(CollisionEvent eventObject, GameObject gameObject, Game window) : base(gameObject, window)
    {
        OnCollision = eventObject;
    }

    public override void Update(FrameEventArgs args)
    {
        if(OnCollision.DoesUpdate)
            OnCollision.Update((float)args.Time);
        
        float distance = Vector3.Distance(window.currentWorld.MainCamera.Position, location);
        // Console.WriteLine("Dist: " + distance);
        
        if(!active)
        {
            if(distance > radius)
                active = true;
            return;
        }

        if (distance < radius)
        {
            active = false;
            flipped = !flipped;
            Trigger();
        }
    }

    protected virtual void Trigger()
    {
        Console.WriteLine("Triggered!");
        OnCollision.Trigger();
    }
}