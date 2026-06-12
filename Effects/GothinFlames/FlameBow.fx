sampler glowMaskSampler : register(s0);
sampler flameSampler : register(s1);
float3 flameInsideColor;
float3 flameBloomColor;
float time;
float dissipateThreshold;
float distortionStrength;
float QuadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 bloomColor : COLOR0) : COLOR0
{
    //Applying the same technique again, we scroll a 
    float2 trailCoords = coords + float2(time * -0.05, 0.0);

    trailCoords.y *= 24.0 * distortionStrength;
    trailCoords = frac(trailCoords);
    float flameNoise = tex2D(flameSampler, trailCoords).r;
    float d = flameNoise > QuadraticBump(coords.x * coords.x + 0.05);
    float d2 = flameNoise > dissipateThreshold;
    flameNoise *= bloomColor.a;
    
    coords.y += sin(time * 0.5 + coords.x * 16.0) * 0.001 ;
    coords.x += sin(0.3 * coords.y * 32.0 + time * 0.05 ) * 0.03 ;
    float4 glowMask = tex2D(glowMaskSampler, coords);
    float3 flameColor = lerp(flameInsideColor, flameBloomColor, saturate(flameNoise / 0.5)) * d;

    float3 bowColorTwo = lerp(flameBloomColor, flameInsideColor, coords.x - 0.15);

    glowMask.rgb *= bowColorTwo;
    glowMask.rgb *= 0.9;
    glowMask.rgb -= flameColor * glowMask.r;
    glowMask *= bloomColor;
    glowMask *= 1.8;
    glowMask *= d2;
    return glowMask;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}