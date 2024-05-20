#define LIGHTS_COUNT 8

struct LightData
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

StructuredBuffer<LightData> LightDataBuffer : register(t0);