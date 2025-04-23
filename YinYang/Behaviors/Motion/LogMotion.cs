using YinYang;
using YinYang.Behaviors.Motion;

public class LogMotion : IFiniteMotion
{
    private readonly string message;
    private bool done = false;

    public LogMotion(string message)
    {
        this.message = message;
    }

    public void Apply(GameObject obj, float deltaTime)
    {
        if (!done)
        {
            Console.WriteLine(message);
            done = true;
        }
    }

    public bool IsDone => done;
}