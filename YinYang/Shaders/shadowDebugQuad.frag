#version 460 core
out vec4 FragColor;
in vec2 TexCoords;

uniform sampler2D depthMap;
uniform samplerCube depthCubeMap;
uniform int faceIndex; // Optional: which cube face to show [0 - 5]

vec3 GetCubeMapDirection(vec2 uv, int face)
{
    uv = uv * 2.0 - 1.0; // convert from [0,1] to [-1,1]

    // From OpenGL cube map face layout
    // See: https://www.khronos.org/opengl/wiki/Cubemap_Texture

    if (face == 0)       return normalize(vec3(1.0, -uv.y, -uv.x)); // +X
    else if (face == 1)  return normalize(vec3(-1.0, -uv.y, uv.x)); // -X
    else if (face == 2)  return normalize(vec3(uv.x, 1.0, uv.y));   // +Y
    else if (face == 3)  return normalize(vec3(uv.x, -1.0, -uv.y)); // -Y
    else if (face == 4)  return normalize(vec3(uv.x, -uv.y, 1.0));  // +Z
    else                 return normalize(vec3(-uv.x, -uv.y, -1.0));// -Z
}

vec3 GetCubeMapDirectionFromGrid(vec2 uv)
{
    // Map uv to 3x2 grid:
    float cellWidth = 1.0 / 3.0;
    float cellHeight = 1.0 / 2.0;

    int face = -1;
    vec2 localUV;

    // Determine face
    if (uv.y < 0.5) {
        if      (uv.x < 1.0 / 3.0) { face = 0; localUV = fract(uv * vec2(3.0, 2.0)); } // +X
        else if (uv.x < 2.0 / 3.0) { face = 1; localUV = fract(uv * vec2(3.0, 2.0)); } // -X
        else                       { face = 2; localUV = fract(uv * vec2(3.0, 2.0)); } // +Y
    } else {
        if      (uv.x < 1.0 / 3.0) { face = 3; localUV = fract(uv * vec2(3.0, 2.0)); } // -Y
        else if (uv.x < 2.0 / 3.0) { face = 4; localUV = fract(uv * vec2(3.0, 2.0)); } // +Z
        else                       { face = 5; localUV = fract(uv * vec2(3.0, 2.0)); } // -Z
    }

    localUV = localUV * 2.0 - 1.0; // [-1, 1]

    // Map to direction vector
    if (face == 0)       return normalize(vec3(1.0, -localUV.y, -localUV.x)); // +X
    else if (face == 1)  return normalize(vec3(-1.0, -localUV.y, localUV.x)); // -X
    else if (face == 2)  return normalize(vec3(localUV.x, 1.0, localUV.y));   // +Y
    else if (face == 3)  return normalize(vec3(localUV.x, -1.0, -localUV.y)); // -Y
    else if (face == 4)  return normalize(vec3(localUV.x, -localUV.y, 1.0));  // +Z
    else                 return normalize(vec3(-localUV.x, -localUV.y, -1.0));// -Z
}

void main()
{
//    float depthValue = texture(depthCubeMap, vec3(1, 0, 0)).r;
//    FragColor = vec4(vec3(depthValue), 1.0); 
//
//    vec3 dir = GetCubeMapDirection(TexCoords, 5);
//    float depthValue = texture(depthCubeMap, dir).r;

    vec3 dir = GetCubeMapDirectionFromGrid(TexCoords);
    float depthValue = texture(depthCubeMap, dir).r;

    FragColor = vec4(vec3(depthValue), 1.0);
}
