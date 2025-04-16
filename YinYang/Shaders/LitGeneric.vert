#version 460 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in vec3 aTangent;
layout (location = 4) in vec3 aBitangent;

out vec3 Normal;
out vec3 FragPos;
out mat3 TBN;
out vec2 texCoord;
out vec4 FragPosLightSpace;

uniform mat4 mvp;
uniform mat4 model;
uniform mat4 lightSpaceMatrix;
//uniform mat4 normalMatrix;  // To correctly transform normals to world space

void main()
{
    vec3 T = normalize(vec3(vec4(aTangent,   0.0) * model));
    vec3 B = normalize(vec3(vec4(aBitangent, 0.0) * model));
    vec3 N = normalize(vec3(vec4(aNormal,    0.0) * model));
    TBN = mat3(T, B, N);
    
    FragPos = vec3(vec4(aPos, 1.0) * model);
//    Normal = aNormal * mat3(transpose(inverse(model)));
    Normal = normalize(mat3(transpose(inverse(model))) * aNormal);
    texCoord = aTexCoord;
    FragPosLightSpace = vec4(FragPos, 1.0f) * lightSpaceMatrix;
    gl_Position = vec4(aPos, 1.0) * mvp;
}