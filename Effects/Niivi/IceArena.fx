#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s2);
sampler gradientSampler : register(s3);
float2 spriteSize;
float2 noiseTexelSize;
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float t = time;
    t *= 0.2;
    float2 crystalCoords = coords;
    crystalCoords.x += sin(coords.y * 4.0 + t) * 0.05;
    float4 spriteColor = tex2D(spriteSampler, crystalCoords);
    float brighter = spriteColor.r > 0.95;
    float2 scrollingCoords = coords * spriteSize * noiseTexelSize;
    scrollingCoords *= 0.25;
    scrollingCoords += float2(t * 0.2, t * 0.1);
    float4 noiseColor = tex2D(noiseSampler, scrollingCoords);
    float4 finalColor = spriteColor * tintColor - noiseColor * 0.2;
    finalColor.rgb = ApplyHue(finalColor.rgb, t * 4.0 + coords.x * 4.0);
    finalColor *= pow(coords.y, 4.0) * 0.8;
    finalColor += (brighter * pow(coords.y, 4.0) * 0.4);
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}