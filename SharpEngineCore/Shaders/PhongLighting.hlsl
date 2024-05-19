float4 CalculateAmbient(float4 ambientColor, float k)
{
	return saturate(mul(ambientColor, k));
}

float CalculateDiffuse(float4 normal, float4 lightPos, float4 fragWorldPos, float k)
{
    float3 n = (float3) normal;
    float3 l = (float3) lightPos;
    float3 p = (float3) fragWorldPos;
    
    float3 lightDir = normalize(l - p);
    
    float diffuse = saturate(mul(dot(lightDir, n), k));
    
    return diffuse;
}

float CalculateSpecular(
        float4 normal, float4 lightPos, float4 fragWorldPos, 
        float4 eyePos, float power, float k)
{
    float3 n = (float3) normal;
    float3 l = (float3) lightPos;
    float3 p = (float3) fragWorldPos;
    float3 eP = (float3) eyePos;
    
    float3 lightDir = normalize(l - p);
    
    float3 r = normalize(reflect(-lightDir, n));
    float3 v = normalize(eP - p);
    
    float specular = mul(pow(saturate(dot(r, v)), power), k);
    return specular;
}