float CalculateShadow(Texture2D<float> depthTexture, SamplerState depthSampler, float4 lvpPos,
                      float bias, float k, float kHalf)
{
    float3 projCoords = (float3) lvpPos / lvpPos.w;
    float2 uvCoords = float2(0.5 * projCoords.x + 0.5,
                             0.5 * projCoords.y + 0.5);
    float z = 0.5 * projCoords.z + 0.5;
    float depth = depthTexture.Sample(depthSampler, uvCoords);
    
    float visibility = k;
    if(depth + bias < z)
        visibility *= kHalf;
    else
        visibility *= 1;
    
    return visibility;
}