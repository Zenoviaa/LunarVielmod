#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler starSampler : register(s1);
float2 spriteSize;
float2 noiseTexelSize;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{ 
    float mask = tex2D(spriteSampler, coords);
    float2 starCoords = coords * spriteSize * noiseTexelSize;
    starCoords += float2(time * 0.05, time * -0.02);
    starCoords.x += sin(time * 0.4 + coords.y * 8.0) * 0.02;
    float4 starColor = tex2D(starSampler, starCoords);
    return starColor * mask * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}