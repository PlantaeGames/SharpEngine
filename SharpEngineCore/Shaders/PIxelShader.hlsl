#include "PixelInputStruct.hlsl"
#include "LightsSWBuffer.hlsl"
#include "DepthTextures.hlsl"
#include "PhongLighting.hlsl"
#include "ShadowMapping.hlsl"

float4 main(PixelInput input) : SV_Target
{
    float4 color = input.color;
    //return color;
    
    float4 ambient = 0;
    float4 diffuse = 0;
    float4 specular = 0;
    [unroll] for (int i = 0; i < LIGHTS_COUNT; i++)
    {   
        LightData data = LightDataBuffer[i];
        
        Texture2D<float> depthTexture = DepthTextures[i];
        SamplerState depthSampler = DepthSamplers[i];
        float visiblity = CalculateShadow(depthTexture, depthSampler,
                            input.LVPPositions[i], 0.0025, data.Intensity.b);
        
        
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