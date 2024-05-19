#include "VertexInputStruct.hlsl"
#include "TransformCBuffer.hlsl"
#include "LightTransformCBuffer.hlsl"
#include "Transformations.hlsl"

float4 main(VertexInput input) : SV_Position
{
    float4 worldCoords = TransformWorld(
                                input.position, Position, Rotation, Scale);
    float4 lightViewCoords = TransformLightView(
                                worldCoords, LightPosition, LightRotation);
    
    float4 projectionCoords = TransformOrthogonal(lightViewCoords, LightScale);
    
    return projectionCoords;
}