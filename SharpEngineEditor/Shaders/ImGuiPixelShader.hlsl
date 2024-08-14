Texture2D textTexture;
SamplerState textSampler;

struct In
{
    float4 positiion : SV_Position;
    float2 uv : TEXCOORD;
    float4 color : COLOR;
};

float4 main(In input) : SV_Target
{
    float4 color = input.color;
    float4 font = textTexture.Sample(textSampler, input.uv);
    
    color *= font;
    
    return color;
}