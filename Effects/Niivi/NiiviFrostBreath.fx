#include "../Helpers/Math.fxh"
matrix transformMatrix;

sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
sampler noiseSampler2 : register(s2);
float2 spriteSize;
float2 noiseTexelSize;
float2 noise2TexelSize;
float time;
float distortionStrength;

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
    //So frost breath that zemmie wants seems to be defined by a little laser in the middle similar to rek
    //With a faint smoky outside that's heavily distorting, more the further out it goes
    //and the nseveral little smoke particles, icey bits, and snowflakes on top
    //It should also curve a little bit and spark near the edges
    //whre it impacts the ground it should create erupting icicles
    
    //first lets get the laser rendering then cook this shader
    return input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}