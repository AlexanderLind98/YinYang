#version 460 core
out vec4 FragColor;

uniform vec3 color;
uniform vec4 glow; //rgb = color info, 4 = intensity
uniform vec3 viewPos;

in vec3 Normal;
in vec3 FragPos;

//TODO: Have a base color
//TODO: Glow color
//TODO: Fresnel to glow more

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

void main()
{
    // View direction
    vec3 viewDir = normalize(viewPos - FragPos);

    // Surface normal
    vec3 norm = normalize(Normal);

    // Angle between view direction and normal
    float theta = clamp(dot(viewDir, norm), 0.0, 1.0);

    // Fresnel glow
    vec3 F0 = vec3(0.15); // diamond-ish
    vec3 fresnel = fresnelSchlick(theta, F0);

//    vec3 glowColor = glow.rgb * fresnel;
    vec3 glowColor = glow.rgb * (fresnel * glow.w);

    // Combine with base color
    vec3 finalColor = (color + glowColor) * fresnel;

    FragColor = vec4(finalColor, 1.0);
}
