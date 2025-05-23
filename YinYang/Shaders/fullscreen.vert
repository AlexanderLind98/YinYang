#version 460 core
layout(location = 0) in vec2 aPos;
layout(location = 1) in vec2 aTexCoords;

out vec2 texCoord;

void main()
{
    texCoord = aTexCoords;
    gl_Position = vec4(aPos.xy, 0.0, 1.0);
}
