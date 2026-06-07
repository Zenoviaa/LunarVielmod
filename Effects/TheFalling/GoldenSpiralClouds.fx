sampler pixelCloudSampler : register(s0);
sampler shadingCloudSampler : register(s1);
float4 glowColor;
float threshold;
float time;

//Simple hash function
float Hash(in float2 x)
{
    float xhash = cos(x.x * 37.0);
    float yhash = cos(x.y * 57.0);
    return frac(415.92653 * (xhash + yhash));
}

float4 Stars(float2 coords)
{
    float n = Hash(coords);
    float threshold = 0.985;
    
    float l = length(coords);
    float h = sin(l * 50.0) + time * -0.03;
    h = frac(h);
    
    if (n >= threshold)
        n = pow((n - threshold) / (1.0 - threshold), 6.0) * h;
    else
        n = 0.0;
    return float4(n, n, n, n);
}


float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float4 aboveCloudColor = tex2D(pixelCloudSampler, coords - float2(0.0, 0.001));
    float4 cloudColor = tex2D(pixelCloudSampler, coords);
    float4 belowCloudColor = tex2D(pixelCloudSampler, coords + float2(0.0, 0.01));
    
    float diff = abs(aboveCloudColor.r - cloudColor.r);
    float diff2 = abs(belowCloudColor.r - cloudColor.r);
    float d =  aboveCloudColor.r < cloudColor.r;
    float4 finalColor = cloudColor + cloudColor.r * glowColor * d;
    finalColor *= sampleColor;
    finalColor *= 0.5;
    
    float3 gradientAdd = lerp(float3(0.0, 0.0, 0.0), glowColor.rgb, coords.y * coords.y * coords.y);
    gradientAdd *= 0.25;
    

   // finalColor *= 0.36;
    finalColor = pow(finalColor, 2.0);
  
    finalColor.rgb += cloudColor.r * gradientAdd * 3.0;
   // finalColor -= diff < threshold * 0.05 * cloudColor.r;

    
    float4 stars = Stars(coords);
//    brightness *= 1.0 - finalColor.r;

    finalColor.rg += stars.r;
    // finalColor.rgb = lerp(finalColor.rgb, float3(1.0, 1.0, 1.0), n * finalColor.r);
    // finalColor.rgb += pow(tex2D(shadingCloudSampler, coords), 2.0).rgb;
    //finalColor = 1.0 - finalColor;
    return finalColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}