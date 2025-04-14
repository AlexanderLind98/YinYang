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

    // Gamma correction to convert to display color space
    color = pow(color, vec3(1.0 / gamma));

    FragColor = vec4(color, 1.0);
}
