sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;
float time;
float alpha;
float distortion;
float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    //Calculate Distortion
    float d = tex2D(uImage1, coords + float2(time * -0.1, time * -0.05)).r;
    float offset = d * distortion;
    float distortionNoise = tex2D(uImage0, coords + offset + float2(time * -0.05, 0.0)).r;
    float distortion = 0.1;
    
    //Scrolling Noise
    float rot = distortionNoise * 4.142;
    float2 scrollingNoise = coords + float2(sin(rot) * distortion, time * 0.3);
    
    //Scroll multiple directions and lerp together to make a cool effect
    float3 col = tex2D(uImage1, coords + scrollingNoise).rgb * 0.5;
    for (float f = 0.0; f < 1.0; f += 0.25)
    {
        float rot = f * 3.14;
        float speed = 0.2;
        float2 offsetCoords = coords + float2(
            sin(rot) * time * speed,
            cos(rot) * time * speed);
                           
        float colorMap = tex2D(uImage1, offsetCoords + scrollingNoise).r;
        float3 targetCol = lerp(primaryColor, noiseColor, colorMap);
        col = lerp(col, targetCol, 0.25);
    }
  

    //Outline it
    float3 outlineCol = float3(0.0, 0.0, 0.0);
    float2 diff = coords - float2(0.5, 0.5);
    float l = pow(length(diff), 2.5);
    float factor = l / 0.5;
    float3 finalCol = lerp(col, outlineCol, factor);
    
    float3 trailCol = d * finalCol;
    float p = tex2D(uImage0, coords + float2(time * -0.05, 0.0)).r;
    p *= d;
    
    if (p > alpha)
    {
        return float4(trailCol, 1.0);

    }
    else
    {
        return float4(0.0, 0.0, 0.0, 0.0);
    }
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}