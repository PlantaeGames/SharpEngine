struct Vertex
{
    float4 position : SV_Position;
    float4 color : COLOR;
};

struct PixelInput
{
    float4 position : SV_Position;
    float4 color : COLOR;
};

Vertex main(Vertex vertex)
{
    PixelInput output;
    
    output.position = vertex.position;
    output.color = vertex.color;
    
    return output;
};