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
    
    public ReflectionType reflectionType { get; } = ReflectionType.Dynamic;
    
    public List<Vector3> ProbePositions { get; } = new List<Vector3>();
    
    public float UpdateFrequency { get; } = 0.025f;

    public void AddProbe(Vector3 probe)
    {
        ProbePositions.Add(probe);
    }
}