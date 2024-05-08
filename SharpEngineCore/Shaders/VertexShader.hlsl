struct InputVertex
{
    float4 position : POSITION;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
};

struct OutputVertex
{
    float4 position : SV_Position;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
};

OutputVertex main(InputVertex vertex)
{
    OutputVertex output = (OutputVertex) 0;
    
    output.position = vertex.position;
    output.color = vertex.color;
    
    return output;
};