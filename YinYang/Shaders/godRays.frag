// lightShaft.frag
#version 460 core

out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D sceneTex;
uniform vec2 lightPos;

const int SAMPLES = 256;
const float DECAY = 0.99;
const float STEPCOUNT = 0.8;
const float EXPOSURE = 0.5;
const float WEIGHT = 0.015;

float Rand(vec2 co)
{
    return fract(sin(dot(co, vec2(12.9898, 78.233))) * 43758.5453);
}

void main()
{
    vec2 fragCoord = texCoord;
    vec2 deltaTexCoord = fragCoord - lightPos;
    deltaTexCoord *= 1.0 / float(SAMPLES) * STEPCOUNT;

    float offset = Rand(fragCoord);
    vec2 sampleCoord = fragCoord - deltaTexCoord * offset;

    float illuminationDecay = 1.0;
    vec3 color = vec3(0.0);

    for (int i = 0; i < SAMPLES; ++i)
    {
        sampleCoord -= deltaTexCoord;
        float mask = 1.0 - texture(sceneTex, sampleCoord).r;
        vec3 texSample = vec3(mask);

        texSample *= illuminationDecay * WEIGHT;
        color += texSample;

        illuminationDecay *= DECAY;
    }

    FragColor = vec4(color * EXPOSURE, 1.0);
}