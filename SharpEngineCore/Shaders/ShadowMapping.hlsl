float ToLinearDepth(float depth, float nearPlane, float farPlane)
{
    float z = depth;
    float linZ = (2.0 * nearPlane * farPlane) /
                 (farPlane + nearPlane - z * farPlane);
    linZ = linZ / farPlane;
    
    return linZ;
}

float CalculateShadow(Texture2D<float> depthTexture, SamplerState depthSampler, float4 lvpPos,
                      float nearPlane, float farPlane,
                      float bias, float k, float kHalf)
{
    float3 projCoords = (float3) lvpPos / lvpPos.w;
    float2 uvCoords = float2(0.5 * projCoords.x + 0.5,
                             0.5 * projCoords.y + 0.5);
    float z = 0.5 * projCoords.z + 0.5;
    z = z * 2.0 - 1.0;
    float linZ = ToLinearDepth(z, nearPlane, farPlane);
    
    float depth = depthTexture.Sample(depthSampler, uvCoords);
    float linDepth = ToLinearDepth(depth, nearPlane, farPlane);
    
    float visibility = k;
    
    if(z > 1.0f)
        return visibility;
    
    if(linDepth + bias < linZ)
        visibility *= kHalf;
    else
        visibility *= 1;
    
    return visibility;
}