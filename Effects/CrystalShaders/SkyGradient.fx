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

float4 startColor;
float4 midColor;
float4 endColor;
float h;
float bend;

float QuadraticBump(float t)
{
    const float factor = 4.0;
    return t * (factor - t * factor);
}


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float b = QuadraticBump(coords.x);
    coords.y -= b * bend;
    float4 col = lerp(
        lerp(startColor, midColor, coords.y / h),
        lerp(midColor, endColor, (coords.y - h) / (1.0 - h)),
        step(h, coords.y));
    
    return col * sampleColor;

}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};