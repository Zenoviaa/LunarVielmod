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
    float4 baseColor = tex2D(uImage0, coords);
    if(baseColor.a > 0)
        return float4(0.0, 0.0, 0.0, 0.0);

    float2 leftCoords = coords + float2(-texelSize.x, 0.0);
    float2 rightCoords = coords + float2(texelSize.x, 0.0);
    float2 upCoords = coords + float2(0.0, -texelSize.y);
    float2 downCoords = coords + float2(0.0, texelSize.y);
    
    
    float4 left = tex2D(uImage0, leftCoords);
    float4 right = tex2D(uImage0, rightCoords);
    float4 up = tex2D(uImage0, upCoords);
    float4 down = tex2D(uImage0, downCoords);
    float a = max(left.a, max(right.a, max(up.a, down.a)));
    return sampleColor * a;
}





technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
};