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

texture primaryTexture;
texture noiseTexture;
texture outlineTexture;
float3 outlineColor;

float2 primaryTexSize;
float2 noiseTexSize;
float2 outlineTexSize;
sampler2D primaryTex = sampler_state
{
    texture = <primaryTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
sampler2D outlineTex = sampler_state
{
    texture = <outlineTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    
    //Calculate Distorting Noise
    float s = tex2D(noiseTex, coords * 0.1 + float2(uTime * -0.5, 0.0)).r;
    float offset = s * uOpacity;
    float2 oCoords = coords * 0.1 + offset + float2(uTime * -0.25, 0.0);
    
    
    //Scroll the noise
    float scrollingFlamingNoise = tex2D(primaryTex, oCoords).r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, uIntensity);
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   

    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * uSecondaryColor;
    float3 flameCol = lerp(
        col * uColor * scrollingFlamingNoise,
        noiseFlame, 0.5);
    
    float3 outlineFlameCol = lerp(
        col * outlineColor,
        noiseFlame, 0.5);
    float3 finalCol = flameCol + outlineFlameCol;
    
    // Output to screen
    float4 finalColor = float4(finalCol, 0.0);
    float4 sampleColor = tex2D(uImage0, coords + offset);
    float4 baseColor = tex2D(uImage0, coords);
    float4 fireColor = sampleColor * finalColor;
    return baseColor + (baseColor * (finalColor + finalColor));

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}