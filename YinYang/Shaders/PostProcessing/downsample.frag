// downsample.frag

// This shader performs downsampling on a texture, as taken from Call Of Duty method, presented at ACM Siggraph 2014.
// This particular method was customly designed to eliminate "pulsating artifacts and temporal stability issues".
#version 460 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D srcTexture;
uniform vec2 srcResolution;

void main()
{
    // Compute the size of a single texel in UV space. (Used to offset sample coordinates based on pixel distance)
    vec2 texelSize = 1.0 / srcResolution;
    float x = texelSize.x;
    float y = texelSize.y;

    // === Sampling pattern ===
    // We take 13 samples arranged in a cross + diagonal pattern around the current texel.
    //
    // Grid layout (e = center texel):
    //
    // a - b - c
    // - j - k -
    // d - e - f
    // - l - m -
    // g - h - i
    //
    // This sampling pattern is from Sledgehammer Games (Call of Duty, SIGGRAPH 2014).
    // It's designed to avoid flickering and temporal instability, while staying energy-conservative.
    // Most of the samples are bilinear filtered by the GPU, so the actual lookup count is ~36 due to filtering.

    vec3 a = texture(srcTexture, vec2(texCoord.x - 2*x, texCoord.y + 2*y)).rgb;
    vec3 b = texture(srcTexture, vec2(texCoord.x,       texCoord.y + 2*y)).rgb;
    vec3 c = texture(srcTexture, vec2(texCoord.x + 2*x, texCoord.y + 2*y)).rgb;

    vec3 d = texture(srcTexture, vec2(texCoord.x - 2*x, texCoord.y)).rgb;
    vec3 e = texture(srcTexture, vec2(texCoord.x,       texCoord.y)).rgb; // current pixel
    vec3 f = texture(srcTexture, vec2(texCoord.x + 2*x, texCoord.y)).rgb;

    vec3 g = texture(srcTexture, vec2(texCoord.x - 2*x, texCoord.y - 2*y)).rgb;
    vec3 h = texture(srcTexture, vec2(texCoord.x,       texCoord.y - 2*y)).rgb;
    vec3 i = texture(srcTexture, vec2(texCoord.x + 2*x, texCoord.y - 2*y)).rgb;

    vec3 j = texture(srcTexture, vec2(texCoord.x - x, texCoord.y + y)).rgb;
    vec3 k = texture(srcTexture, vec2(texCoord.x + x, texCoord.y + y)).rgb;
    vec3 l = texture(srcTexture, vec2(texCoord.x - x, texCoord.y - y)).rgb;
    vec3 m = texture(srcTexture, vec2(texCoord.x + x, texCoord.y - y)).rgb;

    // === Weight distribution ===
    // All weights add up to exactly 1.0 to preserve light energy during downsampling.
    //
    // Grouped weight regions:
    // - Center pixel:         e       * 0.125
    // - Corners:              a,c,g,i * 0.03125
    // - Cross (N/E/S/W):      b,d,f,h * 0.0625
    // - Diagonals (x4):       j,k,l,m * 0.125
    //
    // Total sum = 0.125 + (4x 0.03125) + (4x 0.0625) + (4x 0.125) = 1.0

    vec3 color = e * 0.125;
    color += (a + c + g + i) * 0.03125;
    color += (b + d + f + h) * 0.0625;
    color += (j + k + l + m) * 0.125;

    FragColor = vec4(color, 1.0);
}
