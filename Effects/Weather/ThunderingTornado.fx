
matrix transformMatrix;
sampler spriteSampler : register(s0);
float time;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float3 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0;

};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float3 TextureCoordinates : TEXCOORD0;
    float4 Color : COLOR0;
  
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Normal = input.Normal;
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 coords = input.TextureCoordinates.xy;
  
    //coords.y += sin(coords.x * 3.14) * 1.6;
    float4 spriteColor = tex2D(spriteSampler, frac(coords + float2(time * 0.5, time * -2.5)));
    float4 finalColor = input.Color * spriteColor * coords.y / 8.0;
    finalColor *= 2.0;
    finalColor *= input.Normal.r;
    //finalColor *= input.Normal.r;
    return finalColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
