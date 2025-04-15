// magic_render.frag
#version 460 core

in float lifetime;

out vec4 FragColor;

void main()
{
    // EO; if lifetime is negative, discard the particle
    if (lifetime <= 0.0) 
    {
        discard; 
    }

    // fade out over time
    float alpha = clamp(lifetime / 5.0, 0.0, 1.0);
    // magical golden glow
    vec3 baseColor = vec3(0.85, 0.65, 0.15);

    FragColor = vec4(baseColor, alpha);
}

//void main()
//{
//    // blå = lav lifetime, rød = høj
//    float t = clamp(lifetime / 5.0, 0.0, 1.0);
//    FragColor = vec4(t, 0.0, 1.0 - t, 1.0);
//}

