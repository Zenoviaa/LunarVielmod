matrix transformMatrix;
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float3 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    //We have to multiply the position by the matrix so it appears in the correct spot
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    //Lighting pos should already be converted to screen space by the cpu
    //So we can just do this
    float2 coords = input.TextureCoordinates;
    float3 lightColor = input.Color.rgb;
    float lightIntensity = input.Color.a;
    
    
    float4 diffuseLight = float4(0.0, 0.0, 0.0, 1.0);
    
    
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float attenuation = lerp(1.0, 0.0, saturate(dist));
    diffuseLight.rgb += lightColor * attenuation;
   
    return diffuseLight * attenuation;
}


technique SpriteDrawing
{
    pass PixelPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};