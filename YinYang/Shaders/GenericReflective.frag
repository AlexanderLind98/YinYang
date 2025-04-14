#version 460 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;

uniform samplerCube environmentCubemap;
uniform vec3 viewPos;

void main()
{
    vec3 I = normalize(FragPos - viewPos);
    vec3 R = reflect(I, normalize(Normal));
    vec3 reflected = texture(environmentCubemap, R).rgb;

    FragColor = vec4(reflected, 1.0); // Pure reflection — mix with base for realism
}
