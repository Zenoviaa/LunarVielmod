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


texture mixTexture;
sampler2D mixTex = sampler_state
{
    texture = <mixTexture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = wrap;
    AddressV = wrap;
};


float2 texelSize;
float4 outlineColor;
float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 uv = coords;
    float4 colorToMix = tex2D(mixTex, coords);
    float4 mask = tex2D(uImage0, coords);
    float4 mixedColor = colorToMix * sampleColor * mask;

    if (mixedColor.a > 0)
        return mixedColor;

    float2 leftCoords = coords + float2(-texelSize.x, 0.0);
    float2 rightCoords = coords + float2(texelSize.x, 0.0);
    float2 upCoords = coords + float2(0.0, -texelSize.y);
    float2 downCoords = coords + float2(0.0, texelSize.y);
    
    
    float4 left = tex2D(uImage0, leftCoords);
    float4 right = tex2D(uImage0, rightCoords);
    float4 up = tex2D(uImage0, upCoords);
    float4 down = tex2D(uImage0, downCoords);
    
    if (left.a > 0)
        return left.a * outlineColor;
    if (right.a > 0)
        return  right.a * outlineColor;
    if (up.a > 0)
        return up.a * outlineColor;
    return down.a * outlineColor;
}

technique Technique1
{
    pass BlackPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}