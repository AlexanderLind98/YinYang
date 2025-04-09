// blending.frag
#version 460 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform float exposure;
const float gamma = 2.2;

void main()
{
    vec3 hdr = texture(scene, TexCoords).rgb;
    vec3 bloom = texture(bloomBlur, TexCoords).rgb;

    vec3 color = hdr + bloom;

    // Reinhard tone mapping
    color = vec3(1.0) - exp(-color * exposure);

    // Gamma correction
    color = pow(color, vec3(1.0 / gamma));

    FragColor = vec4(color, 1.0);
}
