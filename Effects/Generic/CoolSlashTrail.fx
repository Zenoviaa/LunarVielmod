#include "../Helpers/Math.fxh"
matrix transformMatrix;
float time;
sampler2D spriteSampler : register(s1);
sampler2D noiseSampler : register(s2);
float4 topColor;
float4 bottomColor;
float2 spriteSize;
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
   //sample bottom layer
    float2 coords = input.TextureCoordinates.xy;
    float2 framedCoords = coords * 0.33;
 
    float2 bottomCoords = framedCoords + float2(0.0, 2.0 / 3.0);
    float2 middleCoords = framedCoords + float2(0.0, 1.0 / 3.0);
    float2 topCoords = framedCoords;
    
    float speed = -0.5;
    float4 bottomSlashColor = tex2D(spriteSampler, frac(bottomCoords + float2(time * speed, 0.0)));
    float4 middleSlashColor = tex2D(spriteSampler, frac(middleCoords + float2(time * speed, 0.0)));
    float4 topSlashColor = tex2D(spriteSampler, frac(topCoords + float2(time * speed * 1.5, 0.0)));
    
    topSlashColor *= lerp(topColor, bottomColor, pow(coords.y, 3.0));
    middleSlashColor *= lerp(topColor, bottomColor, coords.y);
    bottomSlashColor *= topColor * 0.5;
    
    float4 combinedColor = (bottomSlashColor + middleSlashColor + topSlashColor);
    combinedColor = floor(combinedColor * 16.0) / 16.0;
    combinedColor *= 2.0;
    combinedColor *= 1.0 - coords.y;
    return combinedColor * input.Color;
}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}