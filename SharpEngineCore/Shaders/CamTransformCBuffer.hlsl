cbuffer CamTransform : register(b0)
{
	float4 CamPosition;
	float4 CamRotation;
	float4 CamScale;
	float AspectRatio;
	float Fov;
	float NearPlane;
	float FarPlane;
};