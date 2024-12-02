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

matrix transformMatrix;
texture primaryTexture;
texture noiseTexture;
texture outlineTexture;

float3 primaryColor;
float3 noiseColor;
float3 outlineColor;

float time;
float distortion;
float power;
float blend;

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
    float4 mainColor = tex2D(uImage0, coords);
    
    //Calculate Distorting Noise
    float4 s = tex2D(noiseTex, coords + float2(time * -0.5, 0.0)).r;
    float offset = s * distortion;
    float2 oCoords = coords + offset + float2(time * -0.25, 0.0);
    
    
    //Sample the main noise
    float4 sample = tex2D(primaryTex, oCoords);
    float primaryAlpha = sample.a;
    float scrollingFlamingNoise = sample.r;
    scrollingFlamingNoise = pow(scrollingFlamingNoise, power);
    
    
    //Sample the outline noise
    float4 outlineSample = tex2D(outlineTex, oCoords);
    float outlineAlpha = outlineSample.a;
    float osn = outlineSample.r;
    float3 col = float3(scrollingFlamingNoise, scrollingFlamingNoise, scrollingFlamingNoise);
   
    //Lerp Colors
    float3 noiseFlame = scrollingFlamingNoise * noiseColor;
    float3 flameCol = lerp(
        col * primaryColor * scrollingFlamingNoise,
        noiseFlame, coords.x);
    
    //Add the outline on top
    //Calculate outline color
    float3 outlineFlameCol = lerp(
        osn * outlineColor,
        noiseFlame, coords.x);
    
    float4 trailCol;
    if (outlineAlpha > 0)
    {
        trailCol = float4(outlineFlameCol, outlineAlpha);
    }
    else
    {
        trailCol = float4(flameCol, primaryAlpha);
    }
   
    return mainColor * trailCol;

}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}