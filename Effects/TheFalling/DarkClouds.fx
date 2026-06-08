sampler pixelCloudSampler : register(s0);
sampler noiseSampler : register(s1);

float3 goldColor;
float2 parallax;
float2 texelSize;
float time;
float distortionStrength;

float4 SampleMixedColor(float2 coords)
{
    float4 outlineColor = float4(goldColor, 1.0);
    float2 tSize = texelSize * 2.0;
    float levels = 8.0;
    
    float2 leftCoords = coords + float2(-tSize.x, 0.0);
    float2 rightCoords = coords + float2(tSize.x, 0.0);
    float2 upCoords = coords + float2(0.0, -tSize.y);
    float2 downCoords = coords + float2(0.0, tSize.y);
 
    
    float4 left = tex2D(pixelCloudSampler, leftCoords);
    float4 right = tex2D(pixelCloudSampler, rightCoords);
    float4 up = tex2D(pixelCloudSampler, upCoords);
    float4 down = tex2D(pixelCloudSampler, downCoords);
    
    float4 colorToMix = tex2D(pixelCloudSampler, coords);
    
    
    float4 avgColor = (left + right + up + down) / 4.0;
    avgColor = round(avgColor * levels) / levels;
    colorToMix = round(colorToMix * levels) / levels;
    
    
    float r = abs(avgColor.r - colorToMix.r);
    float g = abs(avgColor.g - colorToMix.g);
    float b = abs(avgColor.b - colorToMix.b);
    float diff = r + g + b;
    float d = diff > 0.06;

    colorToMix.rgb = float3(1.0, 1.0, 1.0) - colorToMix.rgb;
    colorToMix.rgb *= colorToMix.a;
    colorToMix.rgb *= 0.1;
    return colorToMix * (1.0 - d) + (outlineColor * d * colorToMix.a * 0.00005);
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float2 p = parallax;
    p.y *= 0.0;
    coords.x *= 2.0;
    coords += p;
    coords = frac(coords);
    
    float2 noiseSampelCoords = coords;
    noiseSampelCoords.x *= 2.0;
    noiseSampelCoords += float2(time * -0.025, time * -0.025);
    noiseSampelCoords = frac(noiseSampelCoords);
    float n = tex2D(noiseSampler, noiseSampelCoords).r;
    float radians = n * 6.28;
    float2 distortionoffset = float2(0.0, sin(radians)) * distortionStrength;
    
    float2 sampleCoords = coords;
    sampleCoords.x -= time * -0.05;
    sampleCoords = frac(sampleCoords);
    sampleCoords += distortionoffset;

  //  float4 col = tex2D(pixelCloudSampler, sampleCoords);
    float4 col = SampleMixedColor(sampleCoords);
    return col * sampleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}