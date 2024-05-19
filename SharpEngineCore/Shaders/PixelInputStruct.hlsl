#define LVP_POSITIONS_COUNT 8

struct PixelInput
{
    float4 position : SV_Position;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
    float4 camPosition : CAMPOSITION;
    float4 worldPos : WORLDPOS;
    float4 LVPPositions[LVP_POSITIONS_COUNT] : LVPPositions;
};