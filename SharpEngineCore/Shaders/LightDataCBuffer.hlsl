struct LightDataStruct
{
	float4 Position;
	float4 Rotation;
    float4 Scale;
	float4 Color;
	float4 AmbientColor;
    float4 Intensity;
	float4 Type;
	
    float AspectRatio;
    float Fov;
    float NearPlane;
    float FarPlane;
};

cbuffer LightDataCBuffer : register (b0)
{
    LightDataStruct LightData;
};