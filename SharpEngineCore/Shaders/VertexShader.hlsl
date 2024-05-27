#include "VertexInputStruct.hlsl"
#include "PixelInputStruct.hlsl"
#include "TransformCBuffer.hlsl"
#include "CamTransformCBuffer.hlsl"
#include "Transformations.hlsl"
#include "LightsSWBuffer.hlsl"
#include "LightTypeIds.hlsl"
#include "CameraProjectionTypeIds.hlsl"

PixelInput main(VertexInput input)
{
    PixelInput output = (PixelInput) 0;
    output.color = input.color; 
    output.camPosition = CamPosition;
    output.textureCoord = input.textureCoord;
    
    float4 worldCoords = TransformWorld(input.position, Position, Rotation, Scale);
    float4 camViewCoords = TransformCameraView(worldCoords, CamPosition, CamRotation);
    
    float4 projectionCoords = 0;
    if(CamProjection.r == PERSPECTIVE_PROJECTION_ID)
    {
        projectionCoords = TransformPerspective(
                             camViewCoords, AspectRatio, Fov, NearPlane, FarPlane);
    }
    if(CamProjection.r == ORTHOGONAL_PROJECTION_ID)
    {
        projectionCoords = TransformOrthogonal(camViewCoords, CamScale);
    }
    
    output.position = projectionCoords;
    output.worldPos = worldCoords;
    
    float4 normalCoords = TransformNormal(input.normal, Rotation);
    
    output.normal = normalize(normalCoords);
    
    [unroll] for (int i = 0; i < LVP_POSITIONS_COUNT; i++)
    {
        LightData data = LightDataBuffer[i];
        
        float4 lightWorldCoords = TransformWorld(
                                input.position, Position, Rotation, Scale);
        float4 lightViewCoords = TransformLightView(
                                lightWorldCoords, data.Position, data.Rotation);
   
        float4 projectionCoords = 0;
        if(data.Type.r == DIRECTIONAL_LIGHT_ID)
        {
            projectionCoords = TransformOrthogonal(lightViewCoords, data.Scale);
        }
        if(data.Type.r == POINT_LIGHT_ID)
        {
            projectionCoords = TransformPerspective(lightViewCoords,
                                    data.AspectRatio,
                                    data.Fov,
                                    data.NearPlane,
                                    data.FarPlane);
        }
        
        output.LVPPositions[i] = projectionCoords;
    }
    
    return output;
}