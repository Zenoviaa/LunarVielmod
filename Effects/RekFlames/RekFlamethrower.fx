sampler spriteSampler : register(s0);
sampler distortionSampler : register(s1);

float time;
float strength;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float distortionStrength = strength * lerp(0.5, 1.0, (1.0 - sampleColor.a));
    float2 distortingNoiseCoords = coords + float2(0.0, time * -0.1);
    distortingNoiseCoords = frac(distortingNoiseCoords * 3.0);
    float distortionNoise = tex2D(distortionSampler, distortingNoiseCoords);
    
    float2 spriteCoords = coords + float2(sin(distortionNoise), 0.0) * distortionStrength;
    float intensity = (sampleColor.r) * 8.0;
    float4 spriteColor = tex2D(spriteSampler, spriteCoords) * sampleColor * intensity;
    
    
    float2 flameNoiseCoords = coords + float2(time * -0.15, 0.3);
    flameNoiseCoords = frac(flameNoiseCoords * 3.0);
    float flameNoise = tex2D(distortionSampler, flameNoiseCoords);
    spriteColor -= flameNoise * 0.1;

    return spriteColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}