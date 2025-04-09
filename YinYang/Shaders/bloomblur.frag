// bloomblur.frag
#version 460 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D image;

uniform bool horizontal;
const float weight[5] = float[] (0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);

// Gaussian blur
void main()
{
    // Calculate the texture coordinates offset
    vec2 tex_offset = 1.0 / textureSize(image, 0); 
    
    // The size of one texel in texture coordinates
    vec3 result = texture(image, TexCoords).rgb * weight[0];

    // Apply the Gaussian blur
    for(int i = 1; i < 5; ++i)
    {
        // Offset for the current sample, based on the direction (horizontal or vertical)
        vec2 offset = horizontal
        ? vec2(tex_offset.x * i, 0.0)
        : vec2(0.0, tex_offset.y * i);

        // Sample the texture at the offset positions and accumulate the results
        result += texture(image, TexCoords + offset).rgb * weight[i];
        result += texture(image, TexCoords - offset).rgb * weight[i];
    }

    // Set the final color
    FragColor = vec4(result, 1.0);
}
