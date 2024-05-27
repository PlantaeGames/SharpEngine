cbuffer LightTransform : register(b0)
{
    float4 LightPosition;
    float4 LightRotation;
    float4 LightScale;
    float4 LightType;
    
    float LightAspectRatio;
    float LightFov;
    float LightNearPlane;
    float LightFarPlane;
};