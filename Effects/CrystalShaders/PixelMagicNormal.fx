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
float time;

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 sampleColor = tex2D(uImage0, coords);
    float2 st = float2(coords.x, 0.25 + coords.y * 0.5);

    float3 color = tex2D(uImage0, st + float2(time, 0.0)).xyz;
    float3 color2 = tex2D(uImage0, st + float2(-time * 1.5, 0.0)).xyz * 0.5;

    float sam = tex2D(uImage1, float2(st.y, time)).x;
    float mult = tex2D(uImage1, st).x + (sampleColor.g * -2.0);

    float4 output = float4((color) * sampleColor.rgb * (1.0 + color.x * 2.0), color.x * sampleColor.w);
    return output;
}

technique Technique1
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}