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
float threshold;


float3 palette(float t)
{
    const float3 a = float3(0.5, 0.5, 0.5);
    const float3 b = float3(0.5, 0.5, 0.5);
    const float3 c = float3(1.000, 1.000, 1.000);
    const float3 d = float3(0.522, 0.761, 1.000);

    return d + d * cos(6.28318 * (c * t + d));
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 sampleColor = tex2D(uImage0, coords);
    float2 uv = coords;
    float2 uv0 = uv;
    float3 finalColor = float3(0.0, 0.0, 0.0);
    float4 outputColor = float4(0.0, 0.0, 0.0, 0.0);

    for (float i = 0.0; i < 4.0; i++)
    {
        uv = frac(uv * 100.0) - time * 0.03;
        float d = length(uv) * exp(-length(uv0));
        float3 col = palette(length(uv0) + i * .4 + time * .4);

        d = sin(d * 8.0 + time) / 8.0;
        d = abs(d);
        d = pow(0.01 / d, 1.2);
        finalColor += col * d;
    
        //Scrolling Noise
        float2 scrollingNoise = uv + float2(sin(4.142) * distortion, time * 0.3);
    
        //Scroll multiple directions and lerp together to make a cool effect
        float3 coll = float3(0.0, 0.0, 0.0);
        for (float f = 0.0; f < 1.0; f += 0.25)
        {
            float rot = f * 3.14;
            float speed = 0.2;
            float2 offsetCoords = uv + float2(
                sin(rot) * time * speed,
                cos(rot) * time * speed);
                           
            float colorMap = tex2D(uImage1, offsetCoords + scrollingNoise).r;
            float3 targetCol = lerp(float3(0.263, 0.416, 0.557), float3(0.0, 0.0, 0.0), colorMap);
            coll = lerp(finalColor, targetCol, 0.3);
        }  
        outputColor = float4(coll, 1.0);
    }
    float4 sparkleWaterColor = outputColor;
      
    
    uv = coords;
    //Calculate Distorting Noise
    float s = tex2D(uImage1, uv + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = uv + offset + float2(time * -0.25, 0.0);
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(uImage0, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
    
    //Scroll the outline noise
    float osn = tex2D(uImage2, oCoords).r;
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * scrollingFlamingNoise,
        noiseFlame, uv.x);
    
    //Add the outline on top
    float3 outlineFlameCol = lerp(
        osn * outlineColor,
        noiseFlame, uv.x);
    float3 finalCol = flameCol + outlineFlameCol;
    float avg = (finalCol.r + finalCol.g + finalCol.b)/3.0;
    if (avg > threshold)
    {
        finalCol = sparkleWaterColor;
    }
    
    // Output to screen
    float4 screenColor = float4(finalCol, 1.0) * sampleColor.w;
    return screenColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}