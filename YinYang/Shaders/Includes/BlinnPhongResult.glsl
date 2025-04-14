vec3 BlinnPhongResult(vec3 ambient, vec3 diffuse, vec3 specular)
{
    vec3 result;
    if (debugMode == 1)
    result = ambient;
    else if (debugMode == 2)
    result = diffuse;
    else if (debugMode == 3)
    result = specular;
    else
    result = ambient + diffuse + specular;

    return result;
}
