#include "../Helpers/Math.fxh"
matrix transformMatrix;
float time;
sampler2D spriteSampler : register(s1);
sampler2D noiseSampler : register(s2);

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
    VertexShaderOutput output;
    output.Position = mul(input.Position, transformMatrix);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float2 texCoords = input.TextureCoordinates.xy;
    texCoords *= float2(4.0, 1.0);
    texCoords += float2(time * -0.05, 0.0);
    texCoords = frac(texCoords);
    float4 spriteColor = tex2D(spriteSampler, texCoords);
    spriteColor *= input.Color;
    return spriteColor;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}