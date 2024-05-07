struct Input
{
    float4 position : SV_Position;
    float4 color : COLOR;
};

float4 main(Input input) : SV_Target
{
    float4 color;
    
    color = input.color;
    
    return color;
}