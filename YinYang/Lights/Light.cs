using OpenTK.Mathematics;
using YinYang.Components;

namespace YinYang.Lights;

public class Light
{
    public Vector3 LightColor { get; set; } = Vector3.One;
    protected Vector3 DefaultColor = Vector3.One;
    protected float LightIntensity { get; set; } = 1.0f;
    
    public float Constant = 1.0f;
    public float Linear = 0.09f;
    public float Quadratic = 0.032f;

    public GameObject? Visualizer;

    public void ToggleLight()
    {
        LightColor = LightColor == DefaultColor ? Vector3.Zero : DefaultColor;
    }
    
    protected Vector3 ConvertEulerToDirection(Vector3 rotation)
    {
        // Convert Euler angles to radians if necessary (assuming they are already in radians)
        float pitch = rotation.X; // Pitch (rotation around X-axis)
        float yaw = rotation.Y;   // Yaw (rotation around Y-axis)

        // Calculate the direction based on the Euler angles (pitch, yaw)
        float x = MathF.Cos(pitch) * MathF.Cos(yaw);
        float y = MathF.Sin(pitch);
        float z = MathF.Cos(pitch) * MathF.Sin(yaw);

        // Return the resulting direction vector, inverted to match the shader's inversion
        return new Vector3(-x, -y, -z);  // Inverted to match the shader's calculation
    }
    
    protected Vector3 ConvertDirection(Vector3 direction)
    {
        // Normalize the direction to ensure it's a unit vector.
        direction = Vector3.Normalize(direction);
    
        // Compute the pitch: angle from horizontal.
        float pitch = (float)Math.Atan2(direction.Y, Math.Sqrt(direction.X * direction.X + direction.Z * direction.Z));
    
        // Compute the yaw. For a mesh whose forward vector is along Z,
        // a yaw of atan2(direction.X, direction.Z) will yield π/2 for a light direction of (0.5, 0.5, 0).
        float yaw = (float)Math.Atan2(direction.X, direction.Z);
    
        float roll = 0f;

        // Return the Euler angles in radians.
        return new Vector3(pitch, yaw, roll);
    }

    public virtual Vector3 SetPosition(float x, float y, float z)
    {
        return new Vector3(x, y, z);
    }

    
    /*protected Vector3 ConvertDirection(Vector3 direction)
    {
        // Normalize the direction to ensure it's a unit vector
        direction = Vector3.Normalize(direction);
        
        float pitch = (float)Math.Atan2(direction.Y, Math.Sqrt(direction.X * direction.X + direction.Z * direction.Z));
        
        float yaw = (float)Math.Atan2(direction.X, direction.Z) + (float)Math.PI / 2;
        
        float roll = 0f;

        // Return the Euler angles in radians
        return new Vector3(pitch, yaw, roll);
    }*/
}