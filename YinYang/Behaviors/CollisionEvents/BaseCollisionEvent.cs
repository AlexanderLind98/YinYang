namespace YinYang.Behaviors.CollisionEvents;

public abstract class CollisionEvent
{
    public bool DoesUpdate{get;set;} = false;
    
    protected CollisionEvent()
    {
        
    }
    
    public abstract void Trigger();

    public abstract void Update(float deltaTime);
}