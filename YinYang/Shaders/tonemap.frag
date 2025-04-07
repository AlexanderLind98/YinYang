#version 460 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D hdrBuffer;
uniform float exposure;

void main()
{
    const float gamma = 2.2;
    
    vec3 hdrColor = texture(hdrBuffer, TexCoords).rgb;

    hdrColor *= vec3(1.0, 0.5, 0.5); // red tint debug

    // Reinhard tone mapping
    vec3 mapped = vec3(1.0) - exp(-hdrColor * exposure);

    // Gamma correction 
    mapped = pow(mapped, vec3(1.0 / gamma));
    
    FragColor = vec4(hdrColor, 1.0);
} 