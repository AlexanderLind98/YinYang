vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);

    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);

    vec3 texColor = texture(material.diffTex, texCoord).rgb;

    // combine results
    vec3 ambient  = light.ambient  * material.ambient * texColor;
    vec3 diffuse  = light.diffuse * diff * material.diffuse * texColor;
    vec3 specular = light.specular * SpecResult(lightDir, viewDir, normal)
    * vec3(texture(material.specTex, texCoord)) * texColor;

    float shadow = DirShadowCalculation(FragPosLightSpace, normal, lightDir);

    if (debugMode == 1)
    return ambient + (1.0 - shadow);
    else
    return ambient + (1.0 - shadow) * (diffuse + specular);
}
