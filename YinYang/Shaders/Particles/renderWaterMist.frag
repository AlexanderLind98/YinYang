// renderWaterMist.frag

#version 460 core

in float lifetime;
in float distance;

out vec4 FragColor;

// Constants for mist rendering
float fadeDistance = 25.0;
float maxLifetime = 1.5;
float minAlpha = 0.1;
vec3 mistColor = vec3(0.7, 0.85, 0.95);

void main()
{
    if (lifetime <= 0.0)
        discard;

    float alpha = clamp(lifetime / maxLifetime, minAlpha, 1.0);

    float fadeStart = fadeDistance * 0.7;
    float fadeEnd = fadeDistance;

    float distFade = 1.0 - smoothstep(fadeStart, fadeEnd, distance);
    alpha *= distFade;
    
    // Rounding the water
    // [-1,1] range
    vec2 uv = gl_PointCoord * 2.0 - 1.0;
    // distance² from center
    float r2 = dot(uv, uv);
    // if outside circle discard              
    if (r2 > 1.0)
        discard;

    // soft edge falloff
    alpha *= smoothstep(1.0, 0.9, r2);

    FragColor = vec4(mistColor, alpha);
}
