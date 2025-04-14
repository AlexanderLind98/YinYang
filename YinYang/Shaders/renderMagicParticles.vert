// magic_render.vert
#version 460 core

// not used
layout(location = 0) in vec3 dummy;

struct Particle 
{
    vec4 position;
    vec4 velocity;
};

layout(std430, binding = 0) buffer Particles 
{
    Particle particles[];
};

uniform mat4 viewProj;

out float lifetime;

void main()
{
    uint id = gl_InstanceID;
    vec4 pos = particles[id].position;

    // EO; if dead on start, set to far away
    if (pos.w <= 0.0)
    {
        gl_Position = vec4(-1000.0, -1000.0, -1000.0, 1.0);
        gl_PointSize = 0.0;
        return;
    }

    lifetime = pos.w;
    gl_Position = viewProj * vec4(pos.xyz, 1.0);
    gl_PointSize = 6.0;
}
