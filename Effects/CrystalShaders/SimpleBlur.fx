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
float2 texelSize;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    //VERRRRY simple blur
    float2 left = coords + float2(-texelSize.x, 0.0);
    float2 right = coords + float2(texelSize.x, 0.0);
    float2 up = coords + float2(0.0, -texelSize.y);
    float2 down = coords + float2(0.0, texelSize.y);
    
    float4 sum = tex2D(uImage0, left) + tex2D(uImage0, right) + tex2D(uImage0, up) + tex2D(uImage0, down);
    float4 avgColor = sum / 4.0;
    return avgColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};