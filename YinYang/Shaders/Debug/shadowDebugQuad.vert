// shadowDebugQuad.vert
#version 460 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

out vec2 TexCoords;

void main()
{
    TexCoords = aTexCoord;
    gl_Position = vec4(aPos.xy, 0.0, 1.0);
}
