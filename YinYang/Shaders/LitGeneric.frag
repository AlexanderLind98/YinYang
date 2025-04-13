// LitGeneric.frag
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
layout(location = 0) out vec4 FragColor;
layout(location = 1) out vec4 BrightColor;

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
uniform samplerCube cubeMap;
uniform float far_plane;
uniform float bloomThresholdMin;
uniform float bloomThresholdMax;

// INCLUDES (skal stå efter de ting de skal bruge)
#include "BlinnPhongResult.glsl"
#include "SpecResult.glsl"
#include "DirShadowCalculation.glsl"
#include "CalcDirLight.glsl"


//Prototypes / definitions
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);


//Methods

// array of offset direction for sampling
vec3 gridSamplingDisk[20] = vec3[]
(
vec3(1, 1,  1), vec3( 1, -1,  1), vec3(-1, -1,  1), vec3(-1, 1,  1),
vec3(1, 1, -1), vec3( 1, -1, -1), vec3(-1, -1, -1), vec3(-1, 1, -1),
vec3(1, 1,  0), vec3( 1, -1,  0), vec3(-1, -1,  0), vec3(-1, 1,  0),
vec3(1, 0,  1), vec3(-1,  0,  1), vec3( 1,  0, -1), vec3(-1, 0, -1),
vec3(0, 1,  1), vec3( 0, -1,  1), vec3( 0, -1, -1), vec3( 0, 1, -1)
);

float PointShadowCalculation(vec3 fragPos, vec3 lightPos)
{
    // get vector between fragment position and light position
    vec3 fragToLight = fragPos - lightPos;
    // now get current linear depth as the length between the fragment and light position
    float currentDepth = length(fragToLight);
    // test for shadows
    float shadow = 0.0;
    float bias = 0.15;
    int samples = 20;
    float closestDepth = 0.0f;
    float viewDistance = length(viewPos - fragPos);
    float diskRadius = (1.0 + (viewDistance / far_plane)) / 25.0;
//    float diskRadius = 25.0;
    for(int i = 0; i < samples; ++i)
    {
        closestDepth = texture(cubeMap, fragToLight + gridSamplingDisk[i] * diskRadius).r * far_plane;   // undo mapping [0;1]
        if(currentDepth - bias > closestDepth)
            shadow += 1.0;
//        shadow = currentDepth - bias > closestDepth ? 1.0 : 0.0;
//        shadow += currentDepth - bias > closestDepth ? 1.0 : 0.0;
    }
    shadow /= float(samples);

    // display closestDepth as debug (to visualize depth cubemap)
//     FragColor = vec4(vec3(closestDepth / far_plane), 1.0);
//    FragColor = vec4(vec3(currentDepth / far_plane), 1.0); // see what frag thinks its distance is

    return shadow;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    
    // attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    vec3 texColor = texture(material.diffTex, texCoord).rgb;
    
    // combine results
    vec3 ambient = light.ambient * texColor;
    vec3 diffuse = light.diffuse * diff * texColor;
    vec3 specular = light.specular * SpecResult(lightDir, viewDir, normal) * texColor;
    
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    
    float shadow = PointShadowCalculation(fragPos, light.position);
    
    return (ambient + (1.0 - shadow) * (diffuse + specular));

//    return BlinnPhongResult(ambient, diffuse, specular);
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
    
    vec3 result = vec3(0);

    result += CalcDirLight(dirLight, norm, viewDir);

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

    //    FragColor = vec4(result, 1.0f);
    //
    //    // Bright pass: extract highlights
    //    float brightness = dot(result, vec3(0.2126, 0.7152, 0.0722)); // luminance
    //    if (brightness > 1.0)
    //    BrightColor = vec4(result, 1.0);
    //    else
    //    BrightColor = vec4(0.0);

    FragColor = vec4(result, 1.0f);

    // Soft bloom extraction based on luminance 
    vec3 weights = vec3(0.2126, 0.7152, 0.0722); //(magic nuumbers are perceptual luminance weights, based on human eye)
    //vec3 weights = vec3(0.299, 0.587, 0.114); // luminance weights based on old TV standards
    //vec3 weights = vec3(1.0 / 3.0); // greyscale luminance weights 
    float brightness = dot(result, weights);
   
    // use smoothsteep to create a soft threshold between 1.0 and 2.5
    float bloomFactor = smoothstep(bloomThresholdMin, bloomThresholdMax, brightness);
    
    // apply bloom factor to the result
    BrightColor = vec4(result * bloomFactor, 1.0);

    // debugs
    //    FragColor = vec4(0.5, 0.0, 0.0, 1.0);
    //    BrightColor = vec4(0.0);
}
