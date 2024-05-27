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
               
        float4 a = CalculateAmbient(data.Position, input.worldPos, data.AmbientColor,
                                    data.Type.r, data.Intensity.r, 1);
        float d = CalculateDiffuse(
                    input.normal, data.Position, input.worldPos, 
                    data.Rotation, data.Type.r, data.Intensity.r, 1);
        float s = CalculateSpecular(
                    input.normal, data.Position, input.worldPos, 
                    data.Rotation, data.Type.r,
                    input.camPosition, data.Intensity.g, data.Intensity.r, 1);
        
        Texture2DArray<float> depthTexture = DepthTextures[i];
        SamplerState depthSampler = DepthSamplers[i];
        
        float bias = max(0.05 * (1.0 - d), 0.005);    
        float visiblity = CalculateShadow(depthTexture, depthSampler,
                            input.LVPPositions[i], data.Type.r, 0,
                            data.NearPlane, data.FarPlane,
                            bias, data.Intensity.b, data.Intensity.a);
        
       ambient += a * data.Color;
       diffuse += d * data.Color * visiblity;
       specular += s * data.Color;
    }
    
    color *= ambient + diffuse + specular;
    
    return color;
}