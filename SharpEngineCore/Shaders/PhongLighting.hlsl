#include "Transformations.hlsl"
#include "LightTypeIds.hlsl"


float4 CalculateAmbient(float4 lightPos, float4 fragWorldPos, 
                        float4 ambientColor, float type, float k, float kE)
{
    float3 l = (float3) lightPos;
    float3 p = (float3) fragWorldPos;
   
    float lightDistance = sqrt(pow((l - p).x, 2) + pow((l - p).y, 2) + pow((l - p).z, 2));
    
    float persistance = CalculateInverseLaw(lightDistance, kE);
    if (type == DIRECTIONAL_LIGHT_ID)
    {
        persistance = 1;
    }
	return saturate(mul(ambientColor, k)) * persistance;
}

float CalculateDiffuse(float4 normal, float4 lightPos, float4 fragWorldPos,
                       float4 lightAngles, float type, float k, float kE)
{
    float3 n = normalize((float3) normal);
    float3 l = (float3) lightPos;
    float3 p = (float3) fragWorldPos;
    
    float3 lightDir = normalize(l - p);
    float lightDistance = sqrt(pow((l - p).x, 2) + pow((l - p).y, 2) + pow((l - p).z, 2));
    
    float persistance = CalculateInverseLaw(lightDistance, kE);
    if(type == DIRECTIONAL_LIGHT_ID)
    {
        lightDir = CalculateForwardDir(lightAngles);
        persistance = 1;
    }
    
    float diffuse = saturate(mul(dot(lightDir, n), k)) * persistance;
    
    return diffuse;
}

float CalculateSpecular(
        float4 normal, float4 lightPos, float4 fragWorldPos, float4 lightAngles,
        float type, float4 eyePos, float power, float k, float kE)
{
    float3 n = normalize((float3) normal);
    float3 l = (float3) lightPos;
    float3 p = (float3) fragWorldPos;
    float3 eP = (float3) eyePos;
    
    float3 lightDir = normalize(l - p);
    float lightDistance = sqrt(pow((l - p).x, 2) + pow((l - p).y, 2) + pow((l - p).z, 2));
    
    float persistance = CalculateInverseLaw(lightDistance, kE);
    if (type == DIRECTIONAL_LIGHT_ID)
    {
        lightDir = CalculateForwardDir(lightAngles);
        persistance = 1;
    }
    
    float3 r = normalize(reflect(-lightDir, n));
    float3 v = normalize(eP - p);
    
    float specular = saturate(mul(pow(saturate(dot(r, v)), power), k)) * persistance;
    return specular;
}