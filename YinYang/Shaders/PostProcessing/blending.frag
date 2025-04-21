#version 460 core

out vec4 FragColor;
in  vec2 texCoord;

// Scene + Bloom
uniform bool  bloomEnabled;
uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform float exposure;
uniform float bloomStrength;

// Volumetric lighting
uniform int   volumetricEnabled;
uniform sampler2D volumetric;
uniform vec3  shaftColor;

void main()
{
    // Fetch base HDR scene color
    vec3 hdrScene = texture(scene, texCoord).rgb;

    // Optional bloom glow
    vec3 hdrBloom = bloomEnabled
        ? texture(bloomBlur, texCoord).rgb * bloomStrength
        : vec3(0.0);

    // Optional volumetric shaft contribution
    vec3 shaft = vec3(0.0);
    if (volumetricEnabled != 0)
    {
        // Use R channel from god ray texture as mask
        float shaftIntensity = texture(volumetric, texCoord).r;

        // Optional threshold or curve can go here
        const float maskThreshold = 0.000;
        shaftIntensity = max(0.0, shaftIntensity - maskThreshold);

        const float volumetricWeight = 0.9; // final blending weight
        shaft = shaftIntensity * volumetricWeight * shaftColor;
    }

    // Combine all HDR components
    vec3 hdrCombined = hdrScene + hdrBloom + shaft;

    // Tone mapping (exposure-based)
    vec3 toneMapped = vec3(1.0) - exp(-hdrCombined * exposure);

    FragColor = vec4(toneMapped, 1.0);
}
