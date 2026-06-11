sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float threshold;
float3 flameInsideColor;
float3 flameBloomColor;
float time;


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 tintColor : COLOR0) : COLOR0
{
    float2 diff = (coords - float2(0.5, 0.5));
    float2 pushOffset = diff * 0.5 * time;
    
    coords *= 4.0;
    float2 sampleCoords = coords + pushOffset;

  
    //Applying a distortion will make it feel more natural
    float2 noiseCoords = sampleCoords + float2(time * 0.4, time * 0.4);
    noiseCoords = frac(noiseCoords);
    float n = tex2D(uImage1, noiseCoords).r;
    float radians = n * 6.28;
    float2 dOffset = float2(cos(radians), sin(radians)) * 0.05;
    
    sampleCoords += dOffset;
    sampleCoords = frac(sampleCoords);
    
    float4 noiseColor = tex2D(uImage0, sampleCoords);
    float len = length(diff);
    float interpolant = saturate(len / 0.5);
    
    //First check makes it dissipate over time and the second one gives it the circular look
    //We don't actually need polar coordinates heree, just push the points out from the middle
    float aboveThreshold = noiseColor.r > time && noiseColor.r > interpolant;
    float3 flameColor = lerp(flameBloomColor, flameInsideColor, noiseColor.r) * noiseColor.r;

    float4 finalColor = float4(flameColor, 1.0) * tintColor;
    finalColor.rgb += lerp(float3(1.0, 1.0, 1.0), float3(0.0, 0.0, 0.0), time + time);
    finalColor *= aboveThreshold;
    
    float4 bloomColor = float4(flameBloomColor, 1.0) * tintColor;
    bloomColor *= 0.5;
    
    float aboveThreshold2 = noiseColor.r > (time - 0.6) && noiseColor.r > interpolant;
    bloomColor *= (aboveThreshold2);
    bloomColor *= lerp(1.0, 0.0, time);
    finalColor.rgb += bloomColor.rgb;
    return finalColor * lerp(2.0, 1.0, time);
}


technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}