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
float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;
float power;



float3 palette(float t)
{
    float3 a = float3(0.5, 0.5, 0.5);
    float3 b = float3(0.5, 0.5, 0.5);
    float3 c = float3(0.302, 0.859, 1.000);
    float3 d = float3(1.000, 1.000, 1.000);

    return a + c * cos(2.28318 * (d * t + a));
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 sampleColor = tex2D(uImage0, coords);
    float2 uv = coords;
    float2 uv0 = uv;
    float3 finalColor = float3(0.0, 0.0, 0.0);
    
    for (float i = 0.0; i < 6.0; i++)
    {
        uv = frac(uv / 0.5) - 0.5;

        float d = length(uv) * exp(-length(uv0));

        float3 col = palette(length(uv) + i * .4 + time * .4);

        d = cos(d * 10. + time) / 3.;
        d = abs(d);

        d = pow(0.01 / d, 1.1);

        finalColor += col * d;
    }
        
    float4 duutColor = float4(finalColor, 1.0);
    
    // Normalized pixel coordinates (from 0 to 1)

    //Calculate Distorting Noise
    float s = tex2D(uImage1, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(uImage0, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
    
    
    //Scroll the outline noise
    float osn = tex2D(uImage2, oCoords).r;
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * duutColor.rgb * scrollingFlamingNoise,
        noiseFlame, coords.x);
    
    //Add the outline on top
    float3 outlineFlameCol = lerp(
        osn * outlineColor,
        noiseFlame, coords.x);
    float3 finalCol = flameCol + outlineFlameCol;
    
    // Output to screen
    float4 f = float4(finalCol, 1.0) * sampleColor.w;
    return f;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}