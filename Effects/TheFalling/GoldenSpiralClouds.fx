sampler pixelCloudSampler : register(s0);
sampler shadingCloudSampler : register(s1);
float4 glowColor;
float2 parallax;
float threshold;
float time;


float4 Stars(float2 coords, float2 noiseCoords)
{
    float2 starCoords = coords * 6.0;
    starCoords = frac(starCoords);
    
    float4 stars = tex2D(shadingCloudSampler, starCoords);
    float d = stars.r > 0.95;
    stars *= d;
    stars = pow(stars, 0.8);
    
    float2 noiseChannelCoords = float2(time * 0.015 + noiseCoords.x, noiseCoords.y);
    noiseChannelCoords *= 0.5;
    noiseChannelCoords = frac(noiseChannelCoords);

    float n = tex2D(shadingCloudSampler, noiseChannelCoords).r;
    n = pow(n, 2.0);
    stars *= sin( n * 1.5);
    stars *= (1.0 - coords.y);
    return stars;
}


float SampleAverage(float2 coords)
{
    float2 texelSize = float2(0.01, 0.01);
    float2 leftCoords = coords + float2(-texelSize.x, 0.0);
    float2 rightCoords = coords + float2(texelSize.x, 0.0);
    float2 topCoords = coords + float2(0.0, -texelSize.y);
    float2 bottomCoords = coords + float2(0.0, texelSize.y);
    
    float left = tex2D(pixelCloudSampler, leftCoords).r;
    float right = tex2D(pixelCloudSampler, rightCoords).r;
    float top = tex2D(pixelCloudSampler, topCoords).r;
    float bottom = tex2D(pixelCloudSampler, bottomCoords).r;
    
    float avg = left + right + top + bottom;
    avg *= 0.25;
    return avg;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{   
    float4 cloudColor = tex2D(pixelCloudSampler, coords);
    float d = cloudColor.r > 0.5;

    float avgSample = SampleAverage(coords);
    
    float4 shineColor = lerp(float4(0.02, 0.02, 0.08, 1.0), glowColor, avgSample);
    float4 finalColor = cloudColor + avgSample * shineColor * d;
    
    finalColor *= sampleColor;

    finalColor *= 0.75;
    
    float3 gradientAdd = lerp(float3(0.0, 0.0, 0.0), glowColor.rgb, coords.y);
    gradientAdd *= 0.25;
    finalColor = pow(finalColor, 2.5); 
    finalColor.rgb += cloudColor.r * gradientAdd * 3.0;

    
    float2 starCoords = coords;
    starCoords += float2(time * -0.005, time * 0.005);
    starCoords = frac(starCoords);
    float4 stars = Stars(starCoords, coords);
    
    float2 stars2Coords = starCoords;
    stars2Coords += float2(time * -0.005, time * 0.005);
    stars2Coords.x = 1.0 - stars2Coords.x;
    stars2Coords += float2(0.2, 0.4);
    stars2Coords = frac(stars2Coords);
    
    float4 stars2 = Stars(stars2Coords, coords);
    finalColor.rgb += stars.rgb;
    finalColor.rgb += stars2.rgb;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}