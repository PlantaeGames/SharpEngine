#include "SkyboxVertexInputStruct.hlsl"
#include "SkyboxPixelInputStruct.hlsl"
#include "SkyboxTransformCBuffer.hlsl"
#include "Transformations.hlsl"

SkyboxPixelInput main(SkyboxVertexInput input)
{
    SkyboxPixelInput output = (SkyboxPixelInput) 0;
    output.TexCoords = input.Position;
     
    float4 position = TransformNormal(input.Position, -Rotation);  
    //position += float4(0, 0, 2, 0);
    position = TransformPerspective(position, AspectRatio, Fov, NearPlane, FarPlane);
    
    output.Position = position;
    output.Position.z = output.Position.w;
    
    return output;
}
