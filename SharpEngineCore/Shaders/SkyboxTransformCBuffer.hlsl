cbuffer SkyboxTransform : register(b0)
{
    float4 Rotation;
    float AspectRatio;
    float Fov;
    float NearPlane;
    float FarPlane;
}