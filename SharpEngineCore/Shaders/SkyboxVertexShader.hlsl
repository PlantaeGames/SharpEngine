#include "SkyboxVertexInputStruct.hlsl"
#include "SkyboxPixelInputStruct.hlsl"
#include "SkyboxTransformCBuffer.hlsl"
#include "Transformations.hlsl"

SkyboxPixelInput main(SkyboxVertexInput input)
{
    SkyboxPixelInput output = (SkyboxPixelInput) 0;
    output.TexCoords = input.Position;
     
    float4 position = TransformNormal(input.Position, Rotation);    
    //position = TransformPerspective(position, 0.75, 70, 0.03, 1000);
    
    output.Position = position;
    output.Position.z = output.Position.w;
    
    return output;
}
