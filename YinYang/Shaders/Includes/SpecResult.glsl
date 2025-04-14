float SpecResult(vec3 lightDir, vec3 viewDir, vec3 normal)
{
    vec3 halfwayDir = normalize(lightDir + viewDir);
    float pi = 3.14159265;

    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    const float energyConservation = (16.0 + material.shininess) / (16.0 * pi);
    float spec = energyConservation * pow(max(dot(normal, halfwayDir), 0.0f), material.shininess);

    return spec;
}
