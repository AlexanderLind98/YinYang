// blending.frag
#version 460 core
out vec4 FragColor;

in vec2 texCoord;

const float gamma = 2.2;

uniform bool bloomEnabled;
uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform float exposure;
uniform float bloomStrength;

// Volumetric lighting texture
uniform int volumetricEnabled;
uniform sampler2D volumetric;
uniform vec3 shaftColor;

void main()
{
    // Sample scene color (HDR)
    vec3 hdr = texture(scene, texCoord).rgb;

    // Tone-map HDR color
    vec3 toneMapped = vec3(1.0) - exp(-hdr * exposure);

    // Sample bloom texture (already LDR)
    vec3 bloom = texture(bloomBlur, texCoord).rgb;

    // Add bloom after tone mapping
    vec3 color = toneMapped + (bloomEnabled ? bloom * bloomStrength : vec3(0.0));

    // Add Volumetric light after bloom
    vec3 fog = texture(volumetric, texCoord).rgb;
    
    fog *= shaftColor;
    if (volumetricEnabled == 1)
        color += fog;


    // Final color output
    FragColor = vec4(color, 1.0);
}
