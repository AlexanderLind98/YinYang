#version 460

in vec4 FragPos;

uniform vec3 probePos;
uniform float far_plane;
    
void main()
{
    //Distance between frag and light source
    float lightDistance = length(FragPos.xyz - probePos);
    
    //"Normalize" to 0-1
    lightDistance = lightDistance / far_plane;
    
    //Assign depth value
    gl_FragDepth = lightDistance;
}