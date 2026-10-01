#include "../Helpers/Math.fxh"
sampler spriteSampler : register(s0);
sampler noiseSampler : register(s1);
float time;
float distortionStrength;
float2 spriteSize;
float2 noiseTexelSize;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
  
    float2 offsetCoords = coords + float2(time * -0.5, 0.0);
    offsetCoords *= 2.0;
    offsetCoords.x *= 0.15;
    offsetCoords = frac(offsetCoords);

    float2 noiseSampleCoords = offsetCoords * spriteSize * noiseTexelSize;
    float4 noiseColor = tex2D(noiseSampler, noiseSampleCoords);
    
    float2 distortedCoords = coords + float2(noiseColor.r * distortionStrength, 0.0);
    float4 spriteColor = tex2D(spriteSampler, distortedCoords);

    spriteColor *= 1.6;
    

    return spriteColor * tintColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}