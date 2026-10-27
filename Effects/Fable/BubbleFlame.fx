#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float2 spriteSize;
float2 noiseTexelSize;
float3 flameOuterColor;
float3 flameInnerColor;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 noiseCoords = coords * spriteSize * noiseTexelSize;
    float noiseColor = tex2D(noiseSampler, noiseCoords + float2(0.0, time));

    float2 offsetCoords = coords;
    offsetCoords.x += sin(noiseColor * 6.28) * 0.25f;

    float4 mask = tex2D(spriteSampler, offsetCoords);
    mask *= coords.y;
    mask = pow(mask, 0.25);
    mask.a = 0.0;
    mask.rgb *= lerp(flameOuterColor, flameInnerColor, coords.y);
    mask.rgb = floor(mask.rgb * 8.0) / 8.0;
    mask.rgb *= 1.5;
    mask.rgb = lerp(mask.rgb, float3(1.0, 1.0, 1.0) * mask.r, pow(coords.y, 3.0));
    return mask * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}