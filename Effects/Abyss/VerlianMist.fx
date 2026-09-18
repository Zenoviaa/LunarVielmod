sampler maskSampler : register(s0);
sampler cloudSampler : register(s1);
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float t = time;
    t *= 0.2;
    float2 noiseCoords1 = coords + float2(t * 0.3, t * 0.05);
    float2 noiseCoords2 = coords + float2(t * -0.2, t * -0.05 + 0.2);
    
    noiseCoords1 = frac(noiseCoords1);
    noiseCoords2 = frac(noiseCoords2);
    
    float4 cloudColor = tex2D(cloudSampler, noiseCoords1 * 2.0);
    float4 cloudColor2 = tex2D(cloudSampler, noiseCoords2);
    
    float4 mixedCloudColor = cloudColor + cloudColor2;
    mixedCloudColor /= 1.8;
    mixedCloudColor = floor(mixedCloudColor * 8.0) / 8.0;
    mixedCloudColor.rgb *= float3(
    sin(t + coords.x * 8.0),
    cos(t + coords.y * 4.0),
    1.0);
    mixedCloudColor.rgb *= 1.5;
    float maskColor = tex2D(maskSampler, coords).a;
    maskColor = 1.0 - maskColor;
  //  return float4(maskColor, maskColor, maskColor, maskColor);
    return mixedCloudColor * maskColor * sampleColor * 0.6;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}