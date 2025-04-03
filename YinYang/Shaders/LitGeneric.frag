#version 460 core

// debug mode input 
uniform int debugMode; // 0 = full, 1 = ambient, 2 = diffuse, 3 = specular

//Structs
struct Material
{
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
    sampler2D diffTex;
    sampler2D specTex;
};

struct DirLight
{
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

struct PointLight
{
    vec3 position;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

struct SpotLight
{
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};

//Inputs
in vec3 Normal;
in vec3 FragPos;
in vec2 texCoord;
in vec4 FragPosLightSpace;

//Outputs
out vec4 FragColor;

//Defines
#define MAX_POINTLIGHTS 16
#define MAX_SPOTLIGHTS 16

//Uniforms
uniform vec3 viewPos;
uniform Material material;
uniform DirLight dirLight;
uniform int numPointLights;
uniform int numSpotLights;
uniform PointLight pointLights[MAX_POINTLIGHTS];
uniform SpotLight spotLights[MAX_SPOTLIGHTS];
uniform sampler2D shadowMap;

//Prototypes / definitions
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 BlinnPhongResult(vec3 ambient, vec3 diffuse, vec3 specular);
float SpecResult(vec3 lightDir, vec3 viewDir, vec3 normal);
float DirShadowCalculation(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir);

//Methods
float DirShadowCalculation(vec4 fragPosLightSpace, vec3 normal, vec3 lightDir)
{
    // Shadow mapping
//    vec4 fragPosLightSpace = lightSpaceMatrix * vec4(FragPos, 1.0);
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;

    // Check if in shadow
    float closestDepth = texture(shadowMap, projCoords.xy).r;
    float currentDepth = projCoords.z;
    //float bias = 0.005;
    float bias = max(0.05 * (1.0 - dot(normal, lightDir)), 0.005);

//    float shadow = currentDepth > closestDepth + bias ? 1.0 : 0.0;
//

    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
        }
    }
    shadow /= 9.0;

    if(projCoords.z > 1.0)
    shadow = 0.0;
    
    return shadow;
}

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

vec3 BlinnPhongResult(vec3 ambient, vec3 diffuse, vec3 specular)
{
    // Final lighting based on debug mode
    vec3 result;
    if (debugMode == 1)
    result = ambient;
    else if (debugMode == 2)
    result = diffuse;
    else if (debugMode == 3)
    result = specular;
    else
    result = (ambient + diffuse + specular);

    return result;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    
    vec3 texColor = texture(material.diffTex, texCoord).rgb;

    // combine results
    vec3 ambient  = light.ambient  * material.ambient * texColor;
    vec3 diffuse = light.diffuse * diff * material.diffuse * texColor;
    vec3 specular = light.specular * SpecResult(lightDir, viewDir, normal) * vec3(texture(material.specTex, texCoord)) * texColor;

    float shadow = DirShadowCalculation(FragPosLightSpace, normal, lightDir);

    if (debugMode == 1)
        return ambient + (1.0 - shadow);
    else
        return ambient + (1.0 - shadow) * (diffuse + specular);

    // Final lighting based on debug mode
    
//    ambient += (1.0 - shadow);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    
    // attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    
    // combine results
    vec3 ambient = light.ambient  * material.diffuse;
    vec3 diffuse = light.diffuse * (diff * material.diffuse);
    vec3 specular = light.specular * SpecResult(lightDir, viewDir, normal) * vec3(texture(material.specTex, texCoord));
    
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;

    vec3 texColor = vec3(1.0f);

    if(texture(material.diffTex, texCoord).a > 0.0f)
    {
        texColor = texture(material.diffTex, texCoord).rgb;

        ambient *= texColor;
        diffuse *= texColor;
        specular *= texColor;
    }

    return BlinnPhongResult(ambient, diffuse, specular);
}

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    // Normalize input
    vec3 lightDir = normalize(-light.direction);

    // Diffuse
    float diff = max(dot(normal, lightDir), 0.0);
    
    // Ambient
    vec3 ambient = light.ambient * material.ambient;
    vec3 diffuse = light.diffuse * (diff * material.diffuse);
    vec3 specular = light.specular * SpecResult(lightDir, viewDir, normal) * vec3(texture(material.specTex, texCoord));

    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    vec3 LightToPixel = normalize(light.position - FragPos  );

    float theta = dot(LightToPixel, normalize(-light.direction));
    float epsilon = 0.01;  // Soft transition range
    float intensity = smoothstep(light.outerCutOff, light.cutOff, theta);

    ambient *= attenuation;
    diffuse *= intensity;
    specular *= intensity;

    vec3 texColor = vec3(1.0f);

    if(texture(material.diffTex, texCoord).a > 0.0f)
    {
        texColor = texture(material.diffTex, texCoord).rgb;

        ambient *= texColor;
        diffuse *= texColor;
        specular *= texColor;
    }

    // Final lighting based on debug mode
    vec3 result = BlinnPhongResult(ambient, diffuse, specular);

    // Apply intensity to lighting result
    vec3 finalColor = result * intensity + ambient;
    
    return finalColor;
}

void main()
{
    // Normalize input
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    
    vec3 result;
    
    result = CalcDirLight(dirLight, norm, viewDir);
    
    if(numPointLights != 0) //Only calc lights if lights exist!
    {
         for (int i = 0; i < numPointLights; i++)
         {
             result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
         }
    }

    if(numSpotLights != 0) //Only calc lights if lights exist!
    {
        for(int i = 0; i < numSpotLights; i++)
        {
            result += CalcSpotLight(spotLights[i], norm, FragPos, viewDir);
        }
    }
    
    FragColor = vec4(result, 1.0f);
}
