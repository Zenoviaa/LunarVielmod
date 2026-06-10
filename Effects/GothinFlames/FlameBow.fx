sampler glowMaskSampler : register(s0);
sampler flameSampler : register(s1);
float3 flameInsideColor;
float3 flameBloomColor;
float time;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 bloomColor : COLOR0) : COLOR0
{
    //Applying the same technique again, we scroll a 
    float2 trailCoords = coords + float2(time * -0.05, 0.0);
    trailCoords = frac(trailCoords);
    float flameNoise = tex2D(flameSampler, trailCoords).r;
    float d = flameNoise < coords.x;
    flameNoise *= d;
    flameNoise *= bloomColor.a;
 
    float4 glowMask = tex2D(glowMaskSampler, coords);
    float3 flameColor = lerp(flameInsideColor, flameBloomColor, saturate(flameNoise / 0.5));
    glowMask.rgb += flameColor * flameNoise;
    glowMask *= bloomColor;
    return glowMask;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}