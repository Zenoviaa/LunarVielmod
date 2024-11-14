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
float2 uImageSize0;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect;
float2 uZoom;

//Our Variables
float time;
texture distortingNoiseTexture;
float distortion;
sampler2D distortingNoiseTex = sampler_state
{
    texture = <distortingNoiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Calculate Distortion
    float2 sampleNoiseCoords = coords + float2(time * -0.05, time * -0.025);
    float sampleNoise = tex2D(distortingNoiseTex, sampleNoiseCoords).r;
    float distortionOffset = sampleNoise * distortion;
    
    //Apply offset
    coords += distortionOffset;
    float4 sampleColor = tex2D(uImage0, coords);
    return sampleColor;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}