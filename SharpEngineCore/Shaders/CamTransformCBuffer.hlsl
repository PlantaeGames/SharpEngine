cbuffer CamTransform : register(b2)
{
	float4 CamPosition;
	float4 CamRotation;
	float4 CamScale;
    	float4 CamProjection;
	float AspectRatio;
	float Fov;
	float NearPlane;
	float FarPlane;
};