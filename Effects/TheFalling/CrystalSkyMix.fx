sampler pixelCloudSampler : register(s0);
sampler maskSampler : register(s1);
sampler cloudSampler : register(s2);
sampler noiseSampler : register(s3);
float time;
float distortionStrength;

float2 SampleDistortionOffset(float2 coords)
{
    coords += float2(time * 0.02, time * -0.02);
    coords *= 0.5;
    coords = frac(coords);
    float n = tex2D(noiseSampler, coords).r;
    float radians = n * 6.28;
    float2 offset = float2(cos(radians), sin(radians));
    float2 distortionOffset = offset * distortionStrength;
    return distortionOffset;
}

float4 SampleClouds(float2 coords)
{
    float2 cloudCoords = coords + SampleDistortionOffset(coords);
    cloudCoords += float2(time * 0.05, time * -0.05);
    cloudCoords = frac(cloudCoords);
    float4 cloudColor = tex2D(pixelCloudSampler, cloudCoords);
    
    float2 cloudCoords2 = coords + SampleDistortionOffset(coords);
    cloudCoords2 += float2(time * -0.05 + 0.3, time * 0.05);
    cloudCoords2 = frac(cloudCoords2);
    cloudCoords2.x = 1.0 - cloudCoords2.x;
    float4 cloudColor2 = tex2D(pixelCloudSampler, cloudCoords2);
    float4 mixedClouds = cloudColor + cloudColor2;
    mixedClouds *= 0.6;
    return mixedClouds;
}

float4 SampleClouds2(float2 coords)
{
    coords += float2(time * -0.025, time * 0.025);
    coords = frac(coords);
    float4 cloudColor = tex2D(cloudSampler, coords);
    return cloudColor;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float maskColor = tex2D(maskSampler, coords).r;
    coords *= 6.0;
    float4 baseClouds = SampleClouds(coords);
    float4 painterlyClouds = SampleClouds2(coords);
    float4 mixedClouds = baseClouds;
    mixedClouds.rgb -= (painterlyClouds.rgb * 0.3);
    float4 finalClouds = mixedClouds;
    finalClouds *= maskColor * sampleColor;
   // finalClouds = round(finalClouds * 6.0) / 6.0;

    return finalClouds;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}