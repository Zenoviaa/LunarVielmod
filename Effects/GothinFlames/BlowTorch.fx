sampler glowMaskSampler : register(s0);
sampler noiseSampler : register(s1);
float3 flameStartColor;
float3 flameBloomColor;
float time;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    //Apply distortion to the blowtorch so it doesn't just look like one straight circle lol
    float2 noiseCoords = coords + float2(time + 2.4, 0.0);
    noiseCoords = frac(noiseCoords);
    float n = tex2D(noiseSampler, noiseCoords).r;
    float2 distortionOffset = float2(0.0, sin(n * 3.14 * 8.0)) * 0.01;
    float2 sampleCoords = coords + distortionOffset;
    
    //Applying the same technique again, we scroll a 
    //This time we're going to actually apply a gradient to the glow so it isn't the asme glow from the start to the end
    //This will genuinely just look better
    //I think we go from yellow to red
    float4 glowMask = tex2D(glowMaskSampler, sampleCoords) * tintColor;
    float3 flameColor = lerp(flameStartColor, flameBloomColor, coords.x);
    glowMask.rgb *= flameColor;
    glowMask.rgb += lerp(1.5, 0.0, saturate(coords.x + time)) * glowMask.r * 8.0;
    return glowMask;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}