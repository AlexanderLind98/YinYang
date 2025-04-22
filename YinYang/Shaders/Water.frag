
#version 460 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;
in vec2 texCoord;
//in mat3 TBN;

struct WaterMaterial
{
    sampler2D normTex;
    vec3 color;
    vec3 tintColor;
};

uniform samplerCube environmentCubemap;
uniform float time;
uniform vec3 viewPos;
uniform WaterMaterial waterMat;

#define MAX_LOD 5

void main()
{
    vec3 finalColor;

    //Scroll normals
    vec2 scrollUV = texCoord + vec2(time * 0.05, time * 0.03); // tweak speed and direction
    vec3 tangentNormal1 = texture(waterMat.normTex, texCoord + time * vec2(0.05, 0.03)).rgb;
    vec3 tangentNormal2 = texture(waterMat.normTex, texCoord - time * vec2(0.03, 0.05)).rgb;
    
    vec3 combinedNormal = normalize((tangentNormal1 + tangentNormal2) * 0.5 * 2.0 - 1.0);

    //Derive TBN
    vec3 Q1 = dFdx(FragPos);
    vec3 Q2 = dFdy(FragPos);
    vec2 st1 = dFdx(texCoord);
    vec2 st2 = dFdy(texCoord);

    vec3 N = normalize(Normal * -1);
    vec3 T = normalize(Q1 * st2.t - Q2 * st1.t);
    vec3 B = normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    vec3 norm = normalize(TBN * combinedNormal);
    
    //Sample reflection from cube map
    vec3 viewDir = normalize(FragPos - viewPos);
    vec3 reflectDir = reflect(viewDir, norm);
    
    vec3 reflectedColor = texture(environmentCubemap, reflectDir).rgb;

    // Fresnel factor
    float cosTheta = max(dot(-viewDir, norm), 0.0);
    vec3 F0 = vec3(0.02); // baseColor for metals, otherwise 0.04
    vec3 fresnel = F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);

    //Depth tint faked using viewing angles
    float facing = dot(norm, -viewDir);
    vec3 depthTint = mix(waterMat.color, waterMat.tintColor, pow(1.0 - facing, 2.0));

    //Combine
    float specularStrength = 1.0;
    vec3 diffuse = waterMat.color * (1.0f - fresnel);
    vec3 specular = reflectedColor * fresnel;

    //Finalize
//    finalColor = ((diffuse * depthTint) / 5) + specular * specularStrength;
    finalColor = specular * specularStrength;
    FragColor = vec4(finalColor, 1.0);
}
