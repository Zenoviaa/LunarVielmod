#include "../Helpers/Math.fxh"
matrix transformMatrix;
float time;
float alpha;
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
    float glow = QuadraticBump(input.TextureCoordinates.y);
    float4 glowColor = float4(glow, glow, glow, glow) * input.Color;
    float4 finalColor = input.Color + glowColor;
    float ring = sin(input.TextureCoordinates.x * 2.56 + time);
    float ringM = lerp(1.0, ring, alpha);
    finalColor *= ringM;
    return finalColor;

}

technique Technique1
{
    pass PrimitivesPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}