cbuffer MVPCBuffer
{
    matrix MVP;
};

struct Out
{
    float4 positiion : SV_Position;
    float2 uv : TEXCOORD;
    float4 color : COLOR;
};

Out main(float2 position : POSITION, float2 uv : TEXCOORD, float4 color : COLOR)
{
    Out output;
    output.positiion = mul(MVP, float4(position.xy, 0, 1));
    output.uv = uv;
    output.color = color;
    
    return output;
}