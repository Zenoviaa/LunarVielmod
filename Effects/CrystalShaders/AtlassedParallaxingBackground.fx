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
float time;
float heatDistortion;
float4 PixelShaderFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    
    float yTiling = 1.0 / tiling.y;
    int parallaxIndex = coords.y / yTiling;
    
    //Since frac takes the number after decimal point we need to convert the coords to 0-1 to wrap it and then set it back
    //It would not work otherwise
    float2 parallaxingCoords = coords;
    parallaxingCoords *= tiling;
    parallaxingCoords += parallax[parallaxIndex];
    parallaxingCoords = frac(parallaxingCoords);
    parallaxingCoords /= tiling;
    
    float2 finalCoords = offsets[parallaxIndex] + parallaxingCoords;
    float4 spriteColor = tex2D(uImage0, finalCoords) ;
    spriteColor.rgb = lerp(spriteColor.rgb, fadeToColor.rgb, finalCoords.y * fadeToColor.a);
    return spriteColor * sampleColor;
}

float4 HeatDistortionFunction(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    
    float yTiling = 1.0 / tiling.y;
    int parallaxIndex = coords.y / yTiling;
    
    //Since frac takes the number after decimal point we need to convert the coords to 0-1 to wrap it and then set it back
    //It would not work otherwise
    float2 parallaxingCoords = coords;
    parallaxingCoords *= tiling;
    parallaxingCoords += parallax[parallaxIndex];
    parallaxingCoords = frac(parallaxingCoords);
    parallaxingCoords /= tiling;
    
    float2 finalCoords = offsets[parallaxIndex] + parallaxingCoords;
    
    float2 normalCoords = coords + float2(0.0, time * 0.05 + finalCoords.y);
   // normalCoords = frac(normalCoords);
    
    float3 normalVec = tex2D(uImage1, normalCoords).rgb;
    normalVec *= 2.0;
    normalVec -= 1.0;
    finalCoords.x += normalVec.x * heatDistortion;
    float4 spriteColor = tex2D(uImage0, finalCoords);
    spriteColor.rgb = lerp(spriteColor.rgb, fadeToColor.rgb, finalCoords.y * fadeToColor.a);
    return spriteColor * sampleColor;
}

technique SpriteDrawing
{
    pass PixelPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
};

technique HeatDrawing
{
    pass HeatPass
    {
        PixelShader = compile ps_3_0 HeatDistortionFunction();
    }
};