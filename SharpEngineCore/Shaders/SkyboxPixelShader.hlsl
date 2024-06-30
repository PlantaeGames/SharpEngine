#include "SkyboxPixelInputStruct.hlsl"

TextureCube CubeMap : register(t0);
SamplerState CubeMapSampelr : register(s0);

float4 main(SkyboxPixelInput input) : SV_Target
{
    float4 color = CubeMap.Sample(CubeMapSampelr, (float3) input.TexCoords);
    return color;
}