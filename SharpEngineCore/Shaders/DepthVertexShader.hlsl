#include "VertexInputStruct.hlsl"
#include "TransformCBuffer.hlsl"
#include "LightTransformCBuffer.hlsl"
#include "Transformations.hlsl"

struct Output
{
    float4 Position : SV_Position;
    float4 Color : Color;
};

Output main(VertexInput input)
{
    Output output = (Output) 0;
    
    float4 worldCoords = TransformWorld(
                                input.position, Position, Rotation, Scale);
    float4 lightViewCoords = TransformLightView(
                                worldCoords, LightPosition, LightRotation);
    
    float4 projectionCoords = TransformPerspective(lightViewCoords,
                                    LightAspectRatio, LightFov, LightNearPlane,
                                    LightFarPlane);
    
    output.Position = projectionCoords;
    
    float z = ((projectionCoords.xyz / projectionCoords.w).z * 0.5 + 0.5) * 2.0 - 1.0;
    float linZ = (2.0 * LightNearPlane * LightFarPlane) /
                 (LightFarPlane + LightNearPlane - z * LightFarPlane);
    output.Color = float4(float3(linZ, linZ, linZ) / LightFarPlane, 1);
    
    return output;
}