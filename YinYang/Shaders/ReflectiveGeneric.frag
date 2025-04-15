#version 460 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;

uniform samplerCube environmentCubemap;
uniform vec3 viewPos;
uniform bool isMetallic;

void main()
{
    vec3 finalColor;
    
    vec3 viewDir = normalize(FragPos - viewPos);
    vec3 normal = normalize(Normal);
    vec3 reflectDir = reflect(viewDir, normalize(Normal));
    
    vec3 reflectedColor = texture(environmentCubemap, reflectDir).rgb;

    vec3 baseColor = vec3(0.0, 0.0, 0.7);

    // Fresnel factor
    float cosTheta = max(dot(-viewDir, normal), 0.0);
    vec3 F0 = mix(vec3(0.04), baseColor, isMetallic); // baseColor for metals, otherwise 0.04
    vec3 fresnel = F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);

    //Multiplying looks metallic, adding looks plasticy
    if(isMetallic)
    {
        finalColor = baseColor * reflectedColor * (fresnel * 5);
    }
    else
    {
        finalColor = baseColor + reflectedColor * (fresnel * 5);
    }

    FragColor = vec4(finalColor, 1.0);
}
