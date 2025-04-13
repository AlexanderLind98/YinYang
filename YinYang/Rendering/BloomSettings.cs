namespace YinYang.Rendering;

/// <summary>
/// Stores and manages all runtime-tunable bloom settings, such as thresholding, strength,
/// exposure, blur radius, mip level configuration, and per-mip weighting.
/// </summary>
public class BloomSettings
{
    /// <summary>HDR exposure for tone mapping (affects brightness of the final image).</summary>
    public float Exposure = 0.1f;

    /// <summary>How much bloom to apply when compositing (0 = none, 1 = full).</summary>
    public float BloomStrength = 0.1f;

    /// <summary>Minimum luminance for a pixel to start contributing to bloom.</summary>
    public float BloomThresholdMin = 1.0f;

    /// <summary>Maximum luminance for full bloom contribution (used in smoothstep).</summary>
    public float BloomThresholdMax = 2.5f;

    /// <summary>Filter radius used during upsampling (controls blur spread at each level).</summary>
    public float FilterRadius = 0.005f;
    

    // MIPCHAIN

    /// <summary>Number of mip levels used for downsample/upsample bloom (higher = softer, wider).</summary>
    public int MipLevels = 5;

    /// <summary>Relative contribution from each mip level during upsampling blend.</summary>
    public float[] MipWeights = BloomWeights_SmoothFade; 

    // WEIGHT PRESETS (manual toggles)

    /// <summary>Flat: equal contribution from all levels (classic additive bloom).</summary>
    public static readonly float[] BloomWeights_Flat = { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };

    /// <summary>Default: smoothly fades contribution across mips.</summary>
    public static readonly float[] BloomWeights_SmoothFade = { 1.0f, 0.6f, 0.3f, 0.15f, 0.05f };

    /// <summary>Subtle: very limited halo spread, mostly tight bloom.</summary>
    public static readonly float[] BloomWeights_Subtle = { 1.0f, 0.3f, 0.1f, 0.0f, 0.0f };

    /// <summary>Cinematic: gentle, long-tail glow falloff.</summary>
    public static readonly float[] BloomWeights_Cinematic = { 0.9f, 0.5f, 0.25f, 0.125f, 0.05f };

    /// <summary>Overbloom: exaggerated, dreamy-style bleeding effect.</summary>
    public static readonly float[] BloomWeights_Overbloom = {1.0f, 1.5f, 2.0f, 2.5f };
    
    /// <summary>
    /// Ensures valid luminance range. Called after user input.
    /// Prevents flip effects by keeping max > min.
    /// </summary>
    public void ClampThresholds()
    {
        const float margin = 0.01f;
        if (BloomThresholdMin >= BloomThresholdMax)
            BloomThresholdMax = BloomThresholdMin + margin;
    }
    
    /// <summary>Returns weight for a given mip level index, or 0 if out of range.</summary>
    public float GetMipWeight(int level)
    {
        if (level < MipWeights.Length)
            return MipWeights[level];
        return 0.0f;
    }
}
