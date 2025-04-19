using OpenTK.Mathematics;

namespace YinYang.Managers;

public class ReflectionManager
{
    public enum ReflectionType
    {
        None,
        Static,
        Dynamic
    }
    
    public ReflectionType reflectionType { get; } = ReflectionType.Static;
    
    public List<Vector3> probePositions { get; } = new List<Vector3>();

    public void AddProbe(Vector3 probe)
    {
        probePositions.Add(probe);
    }
}