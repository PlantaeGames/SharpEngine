#define DEPTH_TEXTURES_COUNT 8

Texture2DArray<float> DepthTextures[DEPTH_TEXTURES_COUNT] : register(t1);
SamplerState DepthSamplers[DEPTH_TEXTURES_COUNT] : register(s1);