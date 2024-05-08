struct Input
{
    float4 position : SV_Position;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
};

float4 main(Input input) : SV_Target
{
    float4 color;
    
    color = input.color;
    
    return color;
}