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
    texCoords += float2(time * -0.25, 0.0);
    texCoords = frac(texCoords);
    float4 spriteColor = tex2D(spriteSampler, frac(texCoords * float2(8.0, 1.0)));
    
    float2 noiseOffsetCoords = texCoords;
    noiseOffsetCoords += float2(time * 0.05, 0.0);
    noiseOffsetCoords = frac(noiseOffsetCoords);
    float noiseColor = tex2D(noiseSampler, noiseOffsetCoords).r;
    float osc = sin(noiseColor * 3.14) * 0.5 + 0.5;
    float osc2 = lerp(osc, 0.8, 1.0);
    spriteColor *= osc2;
    spriteColor += pow(QuadraticBump(texCoords.y), 3.0)  * input.Color * 1.2;
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