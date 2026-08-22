sampler spriteSampler : register(s0);
sampler flameNoiseSampler : register(s1);
sampler metaballSampler : register(s2);
float time;
float3 innerColor;
float3 bloomColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float metaballAlpha = tex2D(metaballSampler, coords).a;
    
    float2 distortingNoiseCoords = coords + float2(time * -0.002, 0.003 * time);
    float n = tex2D(flameNoiseSampler, distortingNoiseCoords).r;
    
    
    float2 flameNoiseCoords = coords + float2(time * -0.05, -0.025 * time);
  
    float2 flameNoiseCoords2 = coords + float2(time * -0.035 + 0.2, 0.02 * time);
    
    
    flameNoiseCoords = frac(flameNoiseCoords * 4.0);
    flameNoiseCoords.y += sin(n * 3.14) * 0.2;
    
    flameNoiseCoords.x += metaballAlpha * 0.4;
    flameNoiseCoords.x = frac(flameNoiseCoords.x);
    flameNoiseCoords2 = frac(flameNoiseCoords2 * 1.8);
    flameNoiseCoords2.y += sin(n * 3.14) * 0.2;
    
    float4 flameNoise1 = tex2D(flameNoiseSampler, flameNoiseCoords);
    float4 flameNoise2 = tex2D(flameNoiseSampler, flameNoiseCoords2);
    flameNoise1.rgb *= lerp(bloomColor, innerColor, flameNoise1.r);
    flameNoise2.rgb *= lerp(bloomColor, innerColor, flameNoise2.r);
    float4 combinedFlameNoise = flameNoise1 + flameNoise2;

    return combinedFlameNoise * metaballAlpha * sampleColor * 2.7;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}