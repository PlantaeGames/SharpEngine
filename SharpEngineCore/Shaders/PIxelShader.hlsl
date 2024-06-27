#include "PixelInputStruct.hlsl"
#include "LightDataCBuffer.hlsl"
#include "DepthTextures.hlsl"
#include "PhongLighting.hlsl"
#include "ShadowMapping.hlsl"
#include "PixelShaderPassSwitchData.hlsl"

float4 main(PixelInput input) : SV_Target
{
    float4 color = input.color;
    
    float4 ambient = 0;
    float4 diffuse = 0;
    float4 specular = 0;
    
    LightDataStruct data = LightData;
               
    float4 a = CalculateAmbient(data.Position, input.worldPos, data.AmbientColor,
                                    data.Type.r, data.Intensity.r, 1);
    float d = CalculateDiffuse(
                    input.normal, data.Position, input.worldPos,
                    data.Rotation, data.Type.r, data.Intensity.r, 1);
    float s = CalculateSpecular(
                    input.normal, data.Position, input.worldPos,
                    data.Rotation, data.Type.r,
                    input.camPosition, data.Intensity.g, data.Intensity.r, 1);
        
    float bias = max(0.05 * (1.0 - d), 0.005);
    float visiblity = CalculateShadow(DepthTexture, DepthSampler,
                            input.LVPPosition, data.Type.r,
                            data.NearPlane, data.FarPlane,
                            bias, data.Intensity.b, data.Intensity.a);
    
    
    ambient = a * data.Color;
    diffuse = d * data.Color * visiblity;
    if (visiblity < 1)
    {
        s = 0;
    }
    specular = s * data.Color;
    
    if(visiblity == 1 &&
       DoLighting == false)
        discard;
    
    if(visiblity < 1 &&
       DoLighting == false)
        color *= -1;      
    
    color *= ambient + diffuse + specular;

    return color;
}