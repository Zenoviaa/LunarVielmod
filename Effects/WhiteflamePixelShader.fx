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

// The X coordinate is the trail completion, the Y coordinate is the elevation on the point of the trail.
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    
    // Read the fade map as a streak.
    float4 fadeMapColor = tex2D(uImage1, float2(frac(coords.y + sin(uTime + 1.57) * 0.01), frac(coords.x - uTime * 1.4 * uSaturation)));
    fadeMapColor.r *= pow(coords.x, 0.2);
    
    float opacity = lerp(1.45, 1.95, fadeMapColor.r) * color.a;
    opacity *= pow(sin(coords.y * 3.141), lerp(1, 4, pow(coords.x, 2)));
    opacity *= pow(sin(coords.x * 3.141), 1.02);
    opacity *= fadeMapColor.r * 1.5 + 1;
    opacity *= lerp(0.4, 0.9, fadeMapColor.r);
    
   // float3 transformColor = lerp(float3(1, 205 / 255.0, 119 / 255.0), float3(1, 76 / 255.0, 79 / 255.0), fadeMapColor.r);
    color.rgb = lerp(color.rgb, uColor, fadeMapColor.r);
    float4 col = color * opacity * 1.6;
    return float4(col.rgb, col.a * uIntensity) * uOpacity;
}

technique Technique1
{
    pass TrailPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
