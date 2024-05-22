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
float2 scroll;
matrix uWorldViewProjection;
float4 uShaderSpecificData;
float uStretchReverseFactor;


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    //Screen color
    float4 displacement = tex2D(uImage1, scroll + coords);
    float2 distortedCoords = coords + float2(displacement.r * displacement.b, displacement.g * displacement.b) * uProgress;

    float4 color = tex2D(uImage0, distortedCoords);
    return color;
}

technique Technique1
{
    pass ScreenPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}