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

float2 parallax[16];
float2 offsets[16];

float4 fadeToColor;
float2 tiling;

float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float yTiling = 1.0 / tiling.y;
    int parallaxIndex = coords.y / yTiling;
    
    //Here we want to calculate how th egradient would overlay this layer, so we just subtract the offset
    float gradientDepth = (coords.y - offsets[parallaxIndex]) / yTiling;
    float gradientStrength = parallaxIndex * yTiling;
    
    
    float2 parallaxingCoords = coords;
    parallaxingCoords *= tiling;
    parallaxingCoords += parallax[parallaxIndex];
    parallaxingCoords = frac(parallaxingCoords);
    parallaxingCoords /= tiling;
    float2 finalCoords = offsets[parallaxIndex] + parallaxingCoords;
    float4 spriteColor = tex2D(uImage0, finalCoords) ;
    spriteColor.rgb = lerp(spriteColor.rgb, fadeToColor.rgb, gradientDepth * fadeToColor.a);
    return spriteColor * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};