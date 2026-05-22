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

float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float2 uv = coords;

    // Time varying pixel color
    float4 col = tex2D(uImage0, uv);
    float fade1 = lerp(0.0, 1.0, QuadraticBump(coords.x));
    float fade2 = lerp(0.0, 1.0, QuadraticBump(coords.y));
    col *= fade1 * fade2;
    return col * sampleColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};