#version 460 core
out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform float theta;

void main()
{
    // Offset texture coordinates to center (0.5, 0.5)
    vec2 centered = texCoord - vec2(0.5);

    // Apply rotation
    float cosT = cos(theta);
    float sinT = sin(theta);
    vec2 rotated = vec2(
    centered.x * cosT - centered.y * sinT,
    centered.x * sinT + centered.y * cosT
    );

    // Re-offset back to original range
    vec2 rotatedTexCoord = rotated + vec2(0.5);

    // Sample from the single texture using rotated coordinates
    outputColor = texture(texture0, rotatedTexCoord);
}


//#version 330 core
//in vec2 texCoord;
//out vec4 FragColor;
//
//void main()
//{
//    FragColor = vec4(texCoord, 0.0, 1.0);
//}
