// renderMagicParticles.vert
#version 460 core

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
uniform vec3 cameraPosition;
uniform float fadeDistance; 

out float lifetime;
out float distance;

void main()
{
    uint id = gl_InstanceID;
    vec4 pos = particles[id].position;

    if (pos.w <= 0.0)
    {
        gl_Position = vec4(-1000.0, -1000.0, -1000.0, 1.0);
        gl_PointSize = 0.0;
        return;
    }

    float dist = length(pos.xyz - cameraPosition);
    distance = dist;

    // bigger points for water
    float size = mix(32.0, 8.0, clamp(dist / fadeDistance, 0.0, 1.0));
    gl_PointSize = size;

    lifetime = pos.w;
    gl_Position = vec4(pos.xyz, 1.0) * viewProj;
}


