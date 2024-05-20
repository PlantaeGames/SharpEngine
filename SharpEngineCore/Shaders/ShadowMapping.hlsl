float CalculateShadow(Texture2D depthTexture, SamplerState depthSampler, float4 lvpPos,
                      float bias, float k)
{
    float3 projCoords = (float3) lvpPos / lvpPos.w;
    float2 uvCoords = float2(0.5 * projCoords.x + 0.5,
                             0.5 * projCoords.y + 0.5);
    float z = 0.5 * projCoords.z + 0.5;
    float depth = depthTexture.Sample(depthSampler, uvCoords);
    
    float visibility = 1 * k;
    if(depth + bias < z)
        visibility *= 0.5;
    else
        visibility *= 1;
    
    return visibility;
}