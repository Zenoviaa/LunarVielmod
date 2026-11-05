#include "../Helpers/Math.fxh"
matrix transformMatrix;

sampler spriteSampler;
sampler noiseSampler;
float time;

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
    //first calculate some coords and oise
    float2 coords = input.TextureCoordinates.xy;
    float2 scrollingCoords = coords + float2(time * -2.0, 0.0);
    scrollingCoords = frac(scrollingCoords);
    float4 smokeColor = tex2D(noiseSampler, scrollingCoords);
    float4 trailColor = tex2D(spriteSampler, scrollingCoords);
    
    float4 finalColor = smokeColor * trailColor;
    finalColor.r += QuadraticBump(coords.y) * 0.2;
    finalColor.rgb = ApplyHue(finalColor.rgb, time + coords.x * 4.0) * 2.0;

    finalColor *= lerp(1.0, 1.5, sin(time * 16.0) * 0.5 + 0.5);
 
    finalColor *= input.Color;

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