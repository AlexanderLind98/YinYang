using OpenTK.Mathematics;
using YinYang.Lights;
using YinYang.Worlds;

namespace YinYang.Managers
{
    /// <summary>
    /// Manages all lighting components in the world, including directional, point, and spot lights.
    /// </summary>
    /// <remarks>
    /// Handles initialization and storage of sun (directional) light and other dynamic lights.
    /// </remarks>
    public class LightingManager
    {
        /// <summary>
        /// The main directional light (sun) used for casting shadows and ambient lighting.
        /// </summary>
        public DirectionalLight Sun { get; private set; }

        /// <summary>
        /// List of all active point lights in the scene.
        /// </summary>
        public List<PointLight> PointLights { get; } = new();

        /// <summary>
        /// List of all active spotlights in the scene.
        /// </summary>
        public List<SpotLight> SpotLights { get; } = new();

        /// <summary>
        /// Initializes the lighting system by creating a sun light and placing it in the world.
        /// </summary>
        /// <param name="world">The parent world in which the light will be placed.</param>
        /// <param name="initialDirection">The initial rotation (direction) of the sun.</param>
        /// <param name="initialColor">The initial color and intensity of the sun.</param>
        public void InitializeDirectionalLight(World world, Vector3 initialDirection, Vector3 initialColor)
        {
            // Create a directional light representing the sun, with the given color and default intensity.
            Sun = new DirectionalLight(world, new Color4(initialColor.X, initialColor.Y, initialColor.Z, 1.0f), 1.0f);
            Sun.shadowType = Light.ShadowType.Static;

            // Set the sun's direction using Euler angles (pitch, yaw, roll).
            // This determines the direction from which the sunlight is cast.
            Sun.Transform.Rotation = initialDirection;

            // Place the sun at an elevated position in the world.
            Sun.Transform.Position = new Vector3(0, 0, 1);

            // Optionally attach a visual representation of the light.
            Sun.UpdateVisualizer(world);
        }
        
        /// <summary>
        /// Initializes the directional light using Euler angles in degrees instead of a direction vector.
        /// </summary>
        /// <param name="world">The world this light belongs to.</param>
        /// <param name="pitchDegrees">X-axis (tilt). Positive = upward.</param>
        /// <param name="yawDegrees">Y-axis (turn). Positive = left.</param>
        /// <param name="rollDegrees">Z-axis (roll). Positive = clockwise (front view).</param>
        /// <param name="initialColor">The color and intensity of the light.</param>
        public void InitializeDirectionalLightInDegrees(World world, float pitchDegrees, float yawDegrees, float rollDegrees, Vector3 initialColor, Light.ShadowType shadowType)
        {
            Sun = new DirectionalLight(world, new Color4(initialColor.X, initialColor.Y, initialColor.Z, 1.0f), 1.0f);

            Sun.Transform.SetRotationInDegrees(pitchDegrees, yawDegrees, rollDegrees);
            Sun.Transform.Position = new Vector3(0, 0, 1);
            Sun.UpdateVisualizer(world);
        }
    }
}
