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
float time;
float distortion;
float3 primaryColor;
float3 noiseColor;
float3 outlineColor;
float2 noiseTextureSize;
texture noiseTexture;
sampler2D noiseTex = sampler_state
{
    texture = <noiseTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Calculate Distortion
    float4 sampleColor = tex2D(uImage0, coords);
    //Tint
    sampleColor.rgb *= primaryColor;
    
    float2 noiseCoords = (coords * uImageSize0 - uSourceRect.xy) / noiseTextureSize;
    float noise = tex2D(noiseTex, noiseCoords + float2(time * -0.05, time * -0.025));
    return sampleColor * noise;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}