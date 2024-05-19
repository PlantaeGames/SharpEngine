#define LIGHTS_COUNT 8

struct LightData
{
	float4 Position;
	float4 Rotation;
    float4 Scale;
	float4 Color;
	float4 AmbientColor;
    float4 Intensity;
	float4 LightType;
};

StructuredBuffer<LightData> LightDataBuffer : register(t0);