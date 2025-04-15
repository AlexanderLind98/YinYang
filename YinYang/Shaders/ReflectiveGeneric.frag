#version 460 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;

uniform samplerCube environmentCubemap;
uniform vec3 viewPos;
uniform bool isMetallic;
uniform float roughness;

#define MAX_LOD 5

void main()
{
    vec3 finalColor;
    
    vec3 viewDir = normalize(FragPos - viewPos);
    vec3 normal = normalize(Normal);
    vec3 reflectDir = reflect(viewDir, normalize(Normal));
    
    //Quadratic filtered LOD
    float lod = roughness * roughness * MAX_LOD;
    vec3 reflectedColor = textureLod(environmentCubemap, reflectDir, lod).rgb;

    vec3 baseColor = vec3(1, 0, 0);

    // Fresnel factor
    float cosTheta = max(dot(-viewDir, normal), 0.0);
    vec3 F0 = mix(vec3(0.04), baseColor, isMetallic); // baseColor for metals, otherwise 0.04
    vec3 fresnel = F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);

    //Multiplying looks metallic, adding looks plasticy
//    if(isMetallic)
//    {
//        finalColor = baseColor * ((reflectedColor * fresnel) * roughness);
//    }
//    else
//    {
//        finalColor = baseColor + ((reflectedColor * fresnel) * roughness);
//    }

    float specularStrength = 1.0 - roughness * roughness;
    vec3 diffuse = baseColor * (1.0f - fresnel);
    vec3 specular = reflectedColor * fresnel * specularStrength;


    finalColor = mix(diffuse + specular, specular, isMetallic ? 1.0 : 0.0);

    FragColor = vec4(finalColor, 1.0);
}
