// lightShaft.frag
#version 460 core

out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D sceneTex;
uniform vec2 lightPos;

const int SAMPLES = 256;
const float DECAY = 0.95;
const float STEPCOUNT = 0.8;
const float EXPOSURE = 0.25;
const float WEIGHT = 0.025;

void main()
{
    vec2 fragCoord = texCoord;
    vec2 deltaTexCoord = fragCoord - lightPos;
    deltaTexCoord *= 1.0 / float(SAMPLES) * STEPCOUNT;

    vec2 texCoord = fragCoord;
    float illuminationDecay = 1.0;

    vec3 color = vec3(0.0);
    for (int i = 0; i < SAMPLES; ++i)
    {
        texCoord -= deltaTexCoord;
        vec3 texSample = texture(sceneTex, texCoord).rgb;

        texSample *= illuminationDecay * WEIGHT;
        color += texSample;

        illuminationDecay *= DECAY;
    }

    FragColor = vec4(color * EXPOSURE, 1.0);
}
