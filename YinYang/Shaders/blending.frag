#version 460 core
out vec4 FragColor;

in vec2 TexCoords;

uniform bool bloomEnabled;
uniform sampler2D scene;
uniform sampler2D bloomBlur;
uniform float exposure;
const float gamma = 2.2;

void main()
{
    vec3 hdr = texture(scene, TexCoords).rgb;
    vec3 bloom;

    if (bloomEnabled)
    bloom = texture(bloomBlur, TexCoords).rgb;
    else
    bloom = vec3(0.0);

    vec3 color = hdr + bloom;

    // Reinhard tone mapping
    color = vec3(1.0) - exp(-color * exposure);

    // Gamma correction
    color = pow(color, vec3(1.0 / gamma));

    FragColor = vec4(color, 1.0);
}
