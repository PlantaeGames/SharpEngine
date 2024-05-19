#include "PixelInputStruct.hlsl"
#include "LightsSWBuffer.hlsl"
#include "DepthTextures.hlsl"
#include "PhongLighting.hlsl"
#include "ShadowMapping.hlsl"

float4 main(PixelInput input) : SV_Target
{
    float4 color = input.color;
    
    float4 ambient = 0;
    float diffuse = 0;
    float specular = 0;
    for (int i = 0; i < LIGHTS_COUNT; i++)
    {
        float visiblity = 1;
        float2 lvpPos = float2(input.LVPPositions[i].x, input.LVPPositions[i].y);
        if (DepthTextures[i].Sample(DepthSamplers[i], lvpPos) < input.LVPPositions[i].z)
        {
            visiblity *= 0.5;
        }
        
        LightData data = LightDataBuffer[i];
        
        float4 a = CalculateAmbient(data.AmbientColor, data.Intensity.r);
        float d = CalculateDiffuse(
                    input.normal, data.Position, input.worldPos, data.Intensity.r);
        float s = CalculateSpecular(
                    input.normal, data.Position, input.worldPos, 
                    input.camPosition, data.Intensity.g, data.Intensity.r);
        
        ambient += a * data.Color * visiblity;
        diffuse += d * data.Color * visiblity;
        specular += s * data.Color * visiblity;
    }
    
    color *= ambient + diffuse + specular;
    
    return color;
}