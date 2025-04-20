// renderMagicParticles.frag
#version 460 core

in float lifetime;
in float distance;

uniform float fadeDistance; 

out vec4 FragColor;

void main()
{
    if (lifetime <= 0.0)
        discard;

    float alpha = clamp(lifetime / 5.0, 0.0, 1.0);

    float fadeStart = fadeDistance * 0.8;
    float fadeEnd = fadeDistance;

    float distFade = 1.0 - smoothstep(fadeStart, fadeEnd, distance);
    alpha *= distFade;

    vec3 baseColor = vec3(0.85, 0.65, 0.15);
    FragColor = vec4(baseColor, alpha);
}
