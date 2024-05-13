struct Input
{
    float4 position : SV_Position;
    float4 normal : NORMAL;
    float4 color : COLOR;
    float4 textureCoord : TEXCOORD;
    float4 camPosition : CAMPOSITION;
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
    float4 Lm = normalize(LightPosition - input.position);
    float4 Kd = 1;
    float4 diffuse = saturate(mul(dot(Lm, normalize(input.normal)), Kd));
    
    // ambient
    float Ia = 1;
    float4 ambient = mul(LightAmbient, Ia);
    
    // specular
    float4 r = normalize(reflect(Lm, normalize(input.normal)));
    float4 v = normalize(input.camPosition - input.position);
    float Ks = .2;
    float specular = mul(pow(saturate(dot(r, v)), 100), Ks);
     
    // final illumination
    float4 illumination = ambient + diffuse;
    
    color = float4(1, 1, 1, 1);
    color *= illumination;
    
    return color;
}