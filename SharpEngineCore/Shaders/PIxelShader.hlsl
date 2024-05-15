struct Input
{
    float4 position : SV_Position;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
    float4 camPosition : CAMPOSITION;
    float4 worldPos : WORLDPOS;
};

cbuffer Light : register(b0)
{
    float4 LightPosition;
    float4 LightRotation;
    float4 LightColor;
    float4 LightAmbient;
}

float4 main(Input input) : SV_Target
{
    float4 color = input.color;
    
    // diffuse 
    float3 l = (float3) LightPosition;
    float3 p = { input.worldPos.x, input.worldPos.y, input.worldPos.z };
    float3 Lm = normalize(l - p);
    float Kd = 1;
    float diffuse = saturate(mul(dot(Lm, normalize((float3)input.normal)), Kd));
    
     // specular
    float3 r = normalize(reflect(Lm, normalize((float3)input.normal)));
    float3 v = normalize((float3) input.camPosition - p);
    float Ks = 0.6f;
    float specular = mul(pow(saturate(dot(r, v)), 32), Ks);
    
    color = 1;
    color *= specular + LightAmbient + diffuse;
    
    return color;
}