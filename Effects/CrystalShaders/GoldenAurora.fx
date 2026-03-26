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

float Time;
float DistortionAmt;
float2 Tiling;
texture DistortionTexture;
sampler2D DistortionTex = sampler_state
{
    texture = <DistortionTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};
float QuadraticBump(float t)
{
    const float factor = 4;
    return t * (factor - t * factor);
}

float2 PolarCoordinates(float2 coords)
{
    #define PI 3.14159
    float2 baseUV = coords;
    baseUV -= 0.5f;
    baseUV *= 2.0;
    
    float angle = atan2(baseUV.y, baseUV.x) / 2.0 * PI;
    float dist = length(float2(baseUV.x, baseUV.y));
    
    float2 polarUV = float2(angle, dist);
    return polarUV;
}


float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 uv = coords;

    float2 polarUV = PolarCoordinates(uv);
    polarUV += float2(0.0, Time * 0.5);
    polarUV = frac(polarUV);
    
    float n = tex2D(DistortionTex, polarUV + float2(0.0, Time * 0.05)).r;
    float radians = n * 3.14;
    float2 offset = float2(cos(radians), sin(radians)) * DistortionAmt;
    polarUV += offset;
    polarUV = frac(polarUV);
    
    polarUV *= Tiling;
  

    float3 col = tex2D(uImage0, polarUV).rgb;
    float dist = length(uv - float2(0.5, 0.5));
    col *= dist / 0.5;
    
    float4 finalColor = float4(col, 1.0);
    finalColor *= sampleColor;
    finalColor *= dist / 0.5;
    return finalColor;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};