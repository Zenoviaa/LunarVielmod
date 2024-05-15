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
matrix uWorldViewProjection;
float4 uShaderSpecificData;
float uStretchReverseFactor;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.y + sin(uTime + 1.57) * 0.01), frac(coords.x - uTime * 1.4 * uSaturation)));
    fadeMapColor.rgb = lerp(fadeMapColor.rgb, uColor, sin(uTime * uSaturation));
    return lerp(color, fadeMapColor, sin(uTime * uSaturation));
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}