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

float2 parallax[4];
float4 fadeToColor;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 parallaxingCoords = coords;
    int parallaxIndex = coords.y / 0.25;
    float2 parallaxAmount = parallax[parallaxIndex];
    parallaxingCoords += parallaxAmount;
    parallaxingCoords = frac(parallaxingCoords);
    float4 spriteColor = tex2D(uImage0, parallaxingCoords) * sampleColor;
    
    
   
    float depth = parallaxIndex;
    float yDepth = coords.y * fadeToColor.a;
   
    spriteColor.rgb = lerp(spriteColor.rgb, fadeToColor.rgb, yDepth);
    return spriteColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};