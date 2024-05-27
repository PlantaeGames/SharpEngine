#include "LightTypeIds.hlsl"

float ToLinearDepth(float depth, float nearPlane, float farPlane)
{
    float z = depth;
    z = z * 2.0 - 1.0;
    
    float linZ = (2.0 * nearPlane * farPlane) /
                 (farPlane + nearPlane - z * farPlane);
    linZ = linZ / farPlane;
    
    return linZ;
}

float CalculateShadow(Texture2DArray<float> depthTexture, SamplerState depthSampler,
                      float4 lvpPos, float lightType, float index,
                      float nearPlane, float farPlane,
                      float bias, float k, float kHalf)
{
    float3 projCoords = (float3) lvpPos / lvpPos.w;
    float2 uvCoords = float2(0.5 * projCoords.x + 0.5,
                             0.5 * projCoords.y + 0.5);
    
    float z = 0.5 * projCoords.z + 0.5;
    
    if(lightType == POINT_LIGHT_ID)
    {
        z = ToLinearDepth(z, nearPlane, farPlane);
    }
    
    float depth = depthTexture.Sample(depthSampler, float3(uvCoords.x, uvCoords.y, index));
    float linDepth = ToLinearDepth(depth, nearPlane, farPlane);
    
    float visibility = k;
    
    if(z > 1.0f)
        return visibility;
    
    if(linDepth + bias < z)
        visibility *= kHalf;
    else
        visibility *= 1;
    
    return visibility;
}