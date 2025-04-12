// upsample.frag

// This shader performs upsampling on a texture, as taken from Call Of Duty method, presented at ACM Siggraph 2014.

#version 460 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D srcTexture;
uniform float filterRadius; 

void main()
{
    // === Texture-space radius ===
    // This controls how wide the blur spreads in UV space.
    // Since we're upsampling, using texture-space ensures the radius *grows* as resolution increases.
    float x = filterRadius;
    float y = filterRadius;

    // === 3x3 tent kernel ===
    // This is a weighted blur centered on the current pixel.
    // The weights are arranged like so:
    //
    //   1 2 1
    //   2 4 2
    //   1 2 1   ← total sum = 16
    //
    // This gives a nice Gaussian-like falloff and smooth blending.

    vec3 a = texture(srcTexture, vec2(texCoord.x - x, texCoord.y + y)).rgb;
    vec3 b = texture(srcTexture, vec2(texCoord.x,     texCoord.y + y)).rgb;
    vec3 c = texture(srcTexture, vec2(texCoord.x + x, texCoord.y + y)).rgb;

    vec3 d = texture(srcTexture, vec2(texCoord.x - x, texCoord.y)).rgb;
    vec3 e = texture(srcTexture, vec2(texCoord.x,     texCoord.y)).rgb; // center pixel
    vec3 f = texture(srcTexture, vec2(texCoord.x + x, texCoord.y)).rgb;

    vec3 g = texture(srcTexture, vec2(texCoord.x - x, texCoord.y - y)).rgb;
    vec3 h = texture(srcTexture, vec2(texCoord.x,     texCoord.y - y)).rgb;
    vec3 i = texture(srcTexture, vec2(texCoord.x + x, texCoord.y - y)).rgb;

    // === Apply weights ===
    // Sum the samples with the tent filter weights (center = 4, edges = 2, corners = 1)
    vec3 color = e * 4.0;
    color += (b + d + f + h) * 2.0;
    color += (a + c + g + i);
    color *= 1.0 / 16.0;

    // Output the final blurred result
    FragColor = vec4(color, 1.0);
}
