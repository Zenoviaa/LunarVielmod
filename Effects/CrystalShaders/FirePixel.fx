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

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)

    //Calculate Distorting Noise
    float s = tex2D(uImage2, coords + float2(uTime * -0.5, 0.0)).r;
    float offset = s * uOpacity;
    float2 oCoords = coords + offset + float2(uTime * -0.25, 0.0);
    
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(uImage1, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, uIntensity);
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   

    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * uSecondaryColor;
    float3 flameCol = lerp(
        col * uColor * scrollingFlamingNoise,
        noiseFlame, coords.x);
    
    float3 finalCol = flameCol;
    
    // Output to screen
    float4 finalColor = float4(finalCol, 1.0);
    float4 sampleColor = tex2D(uImage0, coords + offset);
    return sampleColor * finalColor;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}