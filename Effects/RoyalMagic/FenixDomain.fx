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
float3 gradient[4];
float time;

float quadraticBump(float t)
{
    float factor = 4.0;
    return t * (factor - t * factor);
}
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = coords;
    uv.y += quadraticBump(uv.x) * 0.12;
    uv.y -= 0.45;
    uv.y = 1.0 - uv.y;
    
    float3 backGradient = lerp(float3(1.0, 1.0, 1.0), float3(0.0, 0.0, 0.2), pow(uv.y, 0.25));
    float3 pinks = lerp(float3(1.0, 1.0, 1.0), float3(0.5, 0.0, 0.5), uv.y);
    float3 blues = lerp(float3(1.0, 1.0, 1.0), float3(0.0, 0.0, 0.5), uv.y);
    backGradient = lerp(pinks, backGradient, clamp(uv.y / 0.5, 0.0, 1.0));
    blues = lerp(blues, backGradient, clamp(uv.y / 0.75, 0.0, 1.0));

    float3 pink2 = lerp(float3(1.0, 1.0, 1.0), float3(0.75, 0.35, 1.0), uv.y);
    float3 final = blues + pink2 * 0.5;
    final *= backGradient;
    final *= 0.5;
    

    return float4(final, 1.0) * sampleColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}