// blending.frag
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
    vec3 hdrScene = texture(scene, texCoord).rgb;
    vec3 hdrBloom = bloomEnabled
        ? texture(bloomBlur, texCoord).rgb * bloomStrength
        : vec3(0.0);

    vec3 fogTex   = texture(volumetric, texCoord).rgb;
    float maskVal = fogTex.r;                
    const float maskThreshold    = 0.001;    
    maskVal = max(0.0, maskVal - maskThreshold);

    const float volumetricWeight = 0.005;    
    vec3 hdrFog = maskVal * volumetricWeight * shaftColor;

    vec3 hdrCombined = hdrScene + hdrBloom + hdrFog;

    vec3 toneMapped = vec3(1.0) - exp(-hdrCombined * exposure);

    FragColor = vec4(toneMapped, 1.0);
}
