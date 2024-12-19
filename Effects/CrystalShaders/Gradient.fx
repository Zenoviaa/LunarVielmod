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
float4 StartGradientColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
float4 EndGradientColor = float4(1.0f, 1.0f, 1.0f, 1.0f);

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 dist = coords - float2(0.5f, 1.0f);
    float magnitude = length(dist);
    return lerp(StartGradientColor, EndGradientColor, magnitude);
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}