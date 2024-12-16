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

//Vars
float2 velocity;
float blurStrength;


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // Normalized pixel coordinates (from 0 to 1)
    float4 color = tex2D(uImage0, coords);
    const float samples = 12.0f;
    for (float f = 0.0; f < samples; f++)
    {
        float p = f / samples;
        float3 blurColor = tex2D(uImage0, coords + velocity * p).rgb * 0.85;
        color.rgb += blurColor / (samples / 4.0);
    }
    return color;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};